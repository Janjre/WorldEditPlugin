using System.Collections.ObjectModel;
using System.Text;
using OnixRuntime.Api.Maths;
using System.Security.Cryptography;


namespace WorldEdit;

public class Noise
{
    public static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
    
    public static Vec3 RandomVectorFromSeed(Vec3 position, long seed)
    {
        int x = (int)position.X;
        int y = (int)position.Y;
        int z = (int)position.Z;
        
        string input = $"{x},{y},{z},{seed}";

        // Hash it using SHA256
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

            
            uint aRaw = BitConverter.ToUInt32(hash, 0); // Azimuth angle
            uint bRaw = BitConverter.ToUInt32(hash, 4); // Elevation angle

            
            double azimuth = (aRaw / (double)uint.MaxValue) * 2 * Math.PI;
            double elevation = (bRaw / (double)uint.MaxValue) * Math.PI;

            
            float xDir = (float)(Math.Cos(azimuth) * Math.Sin(elevation));
            float yDir = (float)(Math.Sin(azimuth) * Math.Sin(elevation));
            float zDir = (float)(Math.Cos(elevation));

            return new Vec3(xDir, yDir, zDir);
        }
    }

    public static float Lerp(float a, float b, float t)
    {
        return a + t * (b - a);
    }

    

    public static float perlinNoise (int x, int y,int z, long action)
    {
        
        float noiseScale = 0.1f;

        Vec3 point = new Vec3(x, y, z);
        point *= noiseScale;

        int ix = (int)Math.Floor(point.X);
        int iy = (int)Math.Floor(point.Y);
        int iz = (int)Math.Floor(point.Z);

        float fx = point.X - ix;
        float fy = point.Y - iy;
        float fz = point.Z - iz;

        float u = Fade(fx);
        float v = Fade(fy);
        float w = Fade(fz);

        float[,,] dots = new float[2, 2, 2];

        for (int dx = 0; dx <= 1; dx++)
        {
            for (int dy = 0; dy <= 1; dy++)
            {
                for (int dz = 0; dz <= 1; dz++)
                {
                    Vec3 corner = new Vec3(ix + dx, iy + dy, iz + dz);
                    Vec3 gradient = RandomVectorFromSeed(corner, action);
                    Vec3 offset = point - corner;

                    dots[dx, dy, dz] = gradient.Dot(offset);
                }
            }
        }

        // Trilinear interpolation
        float x00 = Lerp(dots[0, 0, 0], dots[1, 0, 0], u);
        float x10 = Lerp(dots[0, 1, 0], dots[1, 1, 0], u);
        float x01 = Lerp(dots[0, 0, 1], dots[1, 0, 1], u);
        float x11 = Lerp(dots[0, 1, 1], dots[1, 1, 1], u);

        float y0 = Lerp(x00, x10, v);
        float y1 = Lerp(x01, x11, v);

        float final = Lerp(y0, y1, w);


        float normalised = (final + 1f) / 2f;

        return normalised;
    }
    
    public static string BlockFigure(int x, int y, int z, string fill, long action)
    {
        List<(float, string)> tablifiedFill = new List<(float probability, string name)>();
        float totalCount = 0f;
        
        var args = fill.Split(',');
        foreach (string arg in args)
        {
            var parts = arg.Split("%");
            tablifiedFill.Add((   totalCount ,  parts[1]  ));
            totalCount += (float)int.Parse(parts[0])/100;
        }

        foreach ((float a, string b) in tablifiedFill)
        {
            Console.WriteLine($"A: {a} B: {b}");
        }

        if (Math.Abs(totalCount - 1f) > 0.0001f)
        {
            Console.WriteLine("Not good, your arguments are bad :(");
        }

        float perlinPicker = perlinNoise(x, y, z, action);

        tablifiedFill.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        string output = "FOUND NOTHING";

        foreach ((float b, string label) in tablifiedFill)
        {
            if (perlinPicker >= b)
            {
                output = label;
            }
        }
        

        return output;

    }
}