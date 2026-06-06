using Microsoft.Xna.Framework;
using TheMarauderMap.Rendering;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class FootprintRendererTests
{
    [Fact]
    public void FootprintAsset_IsMaraudersMapSpriteSheet()
    {
        string assetPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../assets/footprints.png"));

        PngInfo png = ReadPngInfo(assetPath);

        Assert.Equal(200, png.Width);
        Assert.Equal(220, png.Height);
        Assert.Equal(0, png.CornerAlphas[0]);
        Assert.Equal(0, png.CornerAlphas[1]);
        Assert.Equal(0, png.CornerAlphas[2]);
        Assert.Equal(0, png.CornerAlphas[3]);
        Assert.True(png.VisiblePixels > 1000);
        Assert.True(png.InteriorTransparentPixels > 1000);
    }

    [Fact]
    public void FootprintCloudAsset_IsMaraudersMapCloudSpriteSheet()
    {
        string assetPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../assets/footprints-cloud.png"));

        PngInfo png = ReadPngInfo(assetPath);

        Assert.Equal(100, png.Width);
        Assert.Equal(110, png.Height);
        Assert.True(png.VisiblePixels > 1000);
    }

    [Fact]
    public void GetFootprintAssetPath_ResolvesUnderModDirectory()
    {
        string path = FootprintRenderer.GetFootprintAssetPath("/tmp/TheMarauderMap");

        Assert.Equal(Path.Combine("/tmp/TheMarauderMap", "assets/footprints.png"), path);
    }

    [Fact]
    public void PlanFootSprites_ReturnsOffsetLeftAndRightFeetFromSpriteSheet()
    {
        IReadOnlyList<FootprintSprite> sprites = FootprintSpritePlanner.PlanFootSprites(footstepIndex: 0);

        Assert.Equal(2, sprites.Count);
        Assert.Equal(new Rectangle(150, 0, 50, 110), sprites[0].SourceRectangle);
        Assert.Equal(new Vector2(-5f, 7f), sprites[0].LocalOffset);
        Assert.Equal(new Rectangle(150, 110, 50, 110), sprites[1].SourceRectangle);
        Assert.Equal(new Vector2(5f, -7f), sprites[1].LocalOffset);
    }

    [Fact]
    public void PlanFootSprites_AlwaysUsesCompleteFootprintFrame()
    {
        Assert.Equal(new Rectangle(150, 0, 50, 110), FootprintSpritePlanner.PlanFootSprites(0)[0].SourceRectangle);
        Assert.Equal(new Rectangle(150, 0, 50, 110), FootprintSpritePlanner.PlanFootSprites(1)[0].SourceRectangle);
        Assert.Equal(new Rectangle(150, 0, 50, 110), FootprintSpritePlanner.PlanFootSprites(2)[0].SourceRectangle);
        Assert.Equal(new Rectangle(150, 0, 50, 110), FootprintSpritePlanner.PlanFootSprites(3)[0].SourceRectangle);
        Assert.Equal(new Rectangle(150, 0, 50, 110), FootprintSpritePlanner.PlanFootSprites(4)[0].SourceRectangle);
    }

    [Fact]
    public void GetScale_ReturnsFootprintsTenPercentSmallerThanPreviousScale()
    {
        Assert.Equal(0.198f, FootprintSpritePlanner.GetScale(1f), precision: 3);
        Assert.Equal(0.18f, FootprintSpritePlanner.GetScale(0.5f), precision: 3);
        Assert.Equal(0.378f, FootprintSpritePlanner.GetScale(3f), precision: 3);
    }

    [Fact]
    public void CreateFootprintMask_ReturnsReadableTwinFootprintShape()
    {
        FootprintMask mask = FootprintRenderer.CreateFootprintMask();

        Assert.Equal(18, mask.Width);
        Assert.Equal(22, mask.Height);
        Assert.True(CountOpaquePixels(mask, 0, 8) >= 20);
        Assert.True(CountOpaquePixels(mask, 10, 17) >= 20);
        Assert.Equal(Color.Transparent, mask.Data[(mask.Height / 2) * mask.Width + (mask.Width / 2)]);
    }

    private static int CountOpaquePixels(FootprintMask mask, int minX, int maxX)
    {
        int count = 0;

        for (int y = 0; y < mask.Height; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                if (mask.Data[y * mask.Width + x].A > 0)
                    count++;
            }
        }

        return count;
    }

    private static PngInfo ReadPngInfo(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        Assert.Equal(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, bytes[..8]);

        int position = 8;
        int width = 0;
        int height = 0;
        byte colorType = 0;
        byte[] idat = Array.Empty<byte>();

        while (position < bytes.Length)
        {
            int length = ReadBigEndianInt32(bytes, position);
            position += 4;
            string type = System.Text.Encoding.ASCII.GetString(bytes, position, 4);
            position += 4;
            byte[] chunk = bytes[position..(position + length)];
            position += length + 4;

            if (type == "IHDR")
            {
                width = ReadBigEndianInt32(chunk, 0);
                height = ReadBigEndianInt32(chunk, 4);
                colorType = chunk[9];
            }
            else if (type == "IDAT")
            {
                byte[] combined = new byte[idat.Length + chunk.Length];
                Buffer.BlockCopy(idat, 0, combined, 0, idat.Length);
                Buffer.BlockCopy(chunk, 0, combined, idat.Length, chunk.Length);
                idat = combined;
            }
            else if (type == "IEND")
            {
                break;
            }
        }

        Assert.Equal(6, colorType);

        using var compressed = new MemoryStream(idat);
        using var zlib = new System.IO.Compression.ZLibStream(compressed, System.IO.Compression.CompressionMode.Decompress);
        using var decompressed = new MemoryStream();
        zlib.CopyTo(decompressed);
        byte[] inflated = decompressed.ToArray();
        int stride = width * 4;
        var rows = new byte[height][];
        byte[] previous = new byte[stride];
        int offset = 0;

        for (int y = 0; y < height; y++)
        {
            byte filter = inflated[offset++];
            byte[] scan = inflated[offset..(offset + stride)];
            offset += stride;
            byte[] row = ReconstructPngScanline(filter, scan, previous, bytesPerPixel: 4);
            rows[y] = row;
            previous = row;
        }

        int visiblePixels = 0;
        int minX = width;
        int minY = height;
        int maxX = -1;
        int maxY = -1;
        for (int y = 0; y < rows.Length; y++)
        {
            byte[] row = rows[y];
            for (int x = 0; x < width; x++)
            {
                if (row[x * 4 + 3] > 0)
                {
                    visiblePixels++;
                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, x);
                    maxY = Math.Max(maxY, y);
                }
            }
        }

        int interiorTransparentPixels = 0;
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                if (rows[y][x * 4 + 3] == 0)
                    interiorTransparentPixels++;
            }
        }

        return new PngInfo(
            width,
            height,
            new[]
            {
                (int)rows[0][3],
                (int)rows[0][(width - 1) * 4 + 3],
                (int)rows[height - 1][3],
                (int)rows[height - 1][(width - 1) * 4 + 3]
            },
            visiblePixels,
            interiorTransparentPixels
        );
    }

    private static byte[] ReconstructPngScanline(byte filter, byte[] scan, byte[] previous, int bytesPerPixel)
    {
        var row = new byte[scan.Length];

        for (int i = 0; i < scan.Length; i++)
        {
            int left = i >= bytesPerPixel ? row[i - bytesPerPixel] : 0;
            int up = previous[i];
            int upperLeft = i >= bytesPerPixel ? previous[i - bytesPerPixel] : 0;
            int predictor = filter switch
            {
                0 => 0,
                1 => left,
                2 => up,
                3 => (left + up) / 2,
                4 => Paeth(left, up, upperLeft),
                _ => throw new InvalidDataException($"Unsupported PNG filter {filter}.")
            };

            row[i] = (byte)((scan[i] + predictor) & 0xff);
        }

        return row;
    }

    private static int Paeth(int left, int up, int upperLeft)
    {
        int p = left + up - upperLeft;
        int pa = Math.Abs(p - left);
        int pb = Math.Abs(p - up);
        int pc = Math.Abs(p - upperLeft);

        if (pa <= pb && pa <= pc)
            return left;

        return pb <= pc ? up : upperLeft;
    }

    private static int ReadBigEndianInt32(byte[] bytes, int offset)
    {
        return (bytes[offset] << 24)
            | (bytes[offset + 1] << 16)
            | (bytes[offset + 2] << 8)
            | bytes[offset + 3];
    }

    private sealed record PngInfo(int Width, int Height, int[] CornerAlphas, int VisiblePixels, int InteriorTransparentPixels);
}
