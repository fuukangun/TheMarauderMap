using Microsoft.Xna.Framework;
using TheMarauderMap.Rendering;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class NameRendererTests
{
    [Fact]
    public void CalculateHeartLayout_MatchesTextHeight()
    {
        var textPosition = new Vector2(100, 80);
        var textSize = new Vector2(48, 18);

        HeartLayout layout = NameRenderer.CalculateHeartLayout(textPosition, textSize);

        Assert.Equal(18f, layout.Size);
        Assert.Equal(new Vector2(137, 80), layout.Position);
    }

    [Fact]
    public void GetHeartAssetPath_ResolvesUnderModDirectory()
    {
        string path = NameRenderer.GetHeartAssetPath("/tmp/TheMarauderMap");

        Assert.Equal(Path.Combine("/tmp/TheMarauderMap", "assets/heart.png"), path);
    }

    [Fact]
    public void HeartAsset_IsTransparentPng()
    {
        string assetPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../assets/heart.png"));

        (int width, int height, int[] cornerAlphas) = ReadPngHeaderAndCornerAlphas(assetPath);

        Assert.Equal(64, width);
        Assert.Equal(64, height);
        Assert.All(cornerAlphas, alpha => Assert.Equal(0, alpha));
    }

    private static (int Width, int Height, int[] CornerAlphas) ReadPngHeaderAndCornerAlphas(string path)
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

        return (
            width,
            height,
            new[]
            {
                (int)rows[0][3],
                (int)rows[0][(width - 1) * 4 + 3],
                (int)rows[height - 1][3],
                (int)rows[height - 1][(width - 1) * 4 + 3]
            }
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
}
