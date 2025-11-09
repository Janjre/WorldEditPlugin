using System.Collections.ObjectModel;
using System.Text;
using OnixRuntime.Api.Maths;
using System.Security.Cryptography;


namespace WorldEdit;

public class NoiseCalculator
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

    public static float FlattenPerlin(float v)
    {
        return (float)(0.5 + 0.5 * Math.Sin((v - 0.5) * Math.PI));
    }


    public static float perlinNoise (int x, int y,int z, float noiseScale, long action)
    {
        
        

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

        // string path = Path.Combine(Globals.assetsPath, "logged.txt");
        //
        // string textToAppend = $"{normalised},";
        //
        // using (StreamWriter writer = new StreamWriter(path, append: true))
        // {
        //     writer.WriteLine(textToAppend);
        // }
        //
        
        return FlattenPerlin(normalised);
    }

    public static float layerPerlin(int x, int y, int z, long actionNumber, float scale, int octanes = 4)
    {
        float value = 0f;
        float frequency = 1.0f;
        float amplitude = 1.0f;
        float max_amplitude = 0f;
        for (int n = 0; n <= octanes; n++)
        {
            value += perlinNoise((int)(x * frequency), (int)(y * frequency), (int)(z*frequency), scale, actionNumber) * amplitude;
            max_amplitude += amplitude;
            frequency *= 2;
            amplitude *= 0.5f;
        }

        return value / max_amplitude;
    }

    public static Tuple<List<Tuple<string, float>>,bool> BlockPatternParser(string blockPattern)
    {
        List<Tuple<string, float>> output = [];
        float runningPercentage = 0f;
        
        String[] args = blockPattern.Split('/');
        foreach (string arg in args)
        {
            var parts = arg.Split("%");
            output.Add( new Tuple<string, float> ( parts[1], runningPercentage) );
            runningPercentage += (float)int.Parse(parts[0])/100;
        }
        
        if (Math.Abs(runningPercentage - 1f) > 0.0000001f)
        {
            return new Tuple<List<Tuple<string, float>>, bool>(output,false);
        }

        return new Tuple<List<Tuple<string, float>>, bool>(output,true);
    }
    
    public static Tuple<bool, string> Noise(int x, int y, int z, string blockPattern, float zoom, string noiseType, long action)
    {
        float noiseOutput = 0f;

        if (noiseType == "perlin")
            noiseOutput = perlinNoise(x, y, z, zoom, action);
        else if (noiseType == "rough")
            noiseOutput = layerPerlin(x, y, z, action, zoom);
        else if (noiseType == "white")
            noiseOutput = (float)Globals.MyRandom.NextDouble();

        

        List<Tuple<string, float>> parsedBlockPattern = new List<Tuple<string, float>>();
        bool success = false;

        (parsedBlockPattern, success) = BlockPatternParser(blockPattern);

        if (!success)
            return new Tuple<bool, string>(false, "");

        parsedBlockPattern.Sort((a, b) => a.Item2.CompareTo(b.Item2)); // sort by threshold

        string output = "";

        foreach (var item in parsedBlockPattern)
        {
            if (noiseOutput >= item.Item2)
                output = item.Item1;
        }

        return new Tuple<bool, string>(output != "", output);
    }

}