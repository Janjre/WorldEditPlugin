using OnixRuntime.Api.Maths;

namespace WorldEdit;

public class Selection
{
    public static Vec3 pos1 = new Vec3();
    public static Vec3 pos2 = new Vec3();

    public static List<Vec3> Blocks() // returns a List of all the block positions in your selections so you can for (block in Selection.Blocks){set(block,air)}
    {
        List<Vec3> blocks = [];

        Vec3 smallest = SmallestPoint();
        Vec3 largest = LargestPoint();
        for (int x= (int)smallest.X;x<=largest.X; x++)
        {
            for (int y= (int)smallest.Y;y<=largest.Y; y++)
            {
                for (int z= (int)smallest.X;z<=largest.Z; z++)
                {
                    blocks.Add(new Vec3(x,y,z));
                }
            }
        }

        return blocks;
    }

    public static Vec3 SmallestPoint()
    {
        return new Vec3(
            Math.Min(pos1.X, pos2.X),
            Math.Min(pos1.Y, pos2.Y),
            Math.Min(pos1.Z, pos2.Z)
        );
    }

    public static Vec3 LargestPoint()
    {
        return new Vec3(
            Math.Max(pos1.X, pos2.X),
            Math.Max(pos1.Y, pos2.Y),
            Math.Max(pos1.Z, pos2.Z)
        );
    }
}

