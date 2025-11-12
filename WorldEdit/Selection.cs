using OnixRuntime.Api.Maths;

namespace WorldEdit;


public class Selection
{
    public enum SelectionTypeEnum
    {
        Cuboid,
        Arbitrary
    }
    
    public static Vec3 pos1 = new Vec3();
    public static Vec3 pos2 = new Vec3();

    public static SelectionTypeEnum SelectionType = SelectionTypeEnum.Cuboid;
    public static List<Vec3> ArbitraryBitmap = new List<Vec3>();
    

    public static List<Vec3> Blocks() // returns a List of all the block positions in your selections so you can for (block in Selection.Blocks){set(block,air)}
    {
        if (SelectionType == SelectionTypeEnum.Cuboid)
        {
            List<Vec3> blocks = [];

            Vec3 smallest = SmallestPoint;
            Vec3 largest = LargestPoint;

            for (int x = (int)smallest.X; x <= largest.X; x++)
            {
                for (int y = (int)smallest.Y; y <= largest.Y; y++)
                {
                    for (int z = (int)smallest.Z; z <= largest.Z; z++)
                    {
                        blocks.Add(new Vec3(x, y, z));
                    }
                }
            }

            return blocks;
        }
        if (SelectionType == SelectionTypeEnum.Arbitrary)
        {
            return ArbitraryBitmap;
        }

        return new List<Vec3>();
    }
    public static Vec3 SmallestPoint
    {
        get => new Vec3(
            Math.Min(pos1.X, pos2.X),
            Math.Min(pos1.Y, pos2.Y),
            Math.Min(pos1.Z, pos2.Z)
        );
        set
        {
            // when setting smallest, we only change the coords that are currently smaller
            if (pos1.X < pos2.X) pos1.X = value.X; else pos2.X = value.X;
            if (pos1.Y < pos2.Y) pos1.Y = value.Y; else pos2.Y = value.Y;
            if (pos1.Z < pos2.Z) pos1.Z = value.Z; else pos2.Z = value.Z;
        }
    }

    public static Vec3 LargestPoint
    {
        get => new Vec3(
            Math.Max(pos1.X, pos2.X),
            Math.Max(pos1.Y, pos2.Y),
            Math.Max(pos1.Z, pos2.Z)
        );
        set
        {
            // when setting largest, we only change the coords that are currently larger
            if (pos1.X > pos2.X) pos1.X = value.X; else pos2.X = value.X;
            if (pos1.Y > pos2.Y) pos1.Y = value.Y; else pos2.Y = value.Y;
            if (pos1.Z > pos2.Z) pos1.Z = value.Z; else pos2.Z = value.Z;
        }
    }

    public override string ToString()
    {
        return $"Selection(pos1={pos1}, pos2={pos2})";
    }
    

}

