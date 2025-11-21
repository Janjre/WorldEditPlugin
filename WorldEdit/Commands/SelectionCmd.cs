using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands {
    

    public class SelectionCmd : OnixCommandBase {
        public SelectionCmd() : base("selection", "provides various tools related to area selection", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        


        [Overload]
        OnixCommandOutput Pos1(OnixCommandOrigin origin, [CommandPath("pos1")]string pos1, BlockPos position)
        {
            if (Selection.SelectionType == Selection.SelectionTypeEnum.Arbitrary)
            {
                Console.WriteLine("Changed selection type to cube");
                Selection.SelectionType = Selection.SelectionTypeEnum.Cuboid;
            }
            Selection.pos1 = new Vec3(position);
            return Success($"Set pos1 to be {position.X} {position.Y} {position.Z}");
        }
        
        [Overload]
        OnixCommandOutput Pos2(OnixCommandOrigin origin, [CommandPath("pos2")]string pos2, BlockPos position)
        {
            if (Selection.SelectionType == Selection.SelectionTypeEnum.Arbitrary)
            {
                Console.WriteLine("Changed selection type to cube");
                Selection.SelectionType = Selection.SelectionTypeEnum.Cuboid;
            }
            Selection.pos2 = new Vec3(position);
            return Success($"Set pos2 to be {position.X} {position.Y} {position.Z}");
        }

        [Overload]
        OnixCommandOutput Include(OnixCommandOrigin origin, [CommandPath("include")] string include, BlockPos position)
        {

            if (Selection.SelectionType == Selection.SelectionTypeEnum.Cuboid)
            {
                int oldSize = Selection.Blocks().Count;
                if (!Selection.Blocks().Contains(new Vec3(position)))
                {

                    if (position.X > Selection.LargestPoint.X)
                    {
                        Selection.LargestPoint = new Vec3(position.X, Selection.LargestPoint.Y, Selection.LargestPoint.Z);
                    }

                    if (position.X < Selection.SmallestPoint.X)
                    {
                        Selection.SmallestPoint =
                            new Vec3(position.X, Selection.SmallestPoint.Y, Selection.SmallestPoint.Z);
                    }


                    if (position.Y > Selection.LargestPoint.Y)
                    {
                        Selection.LargestPoint = new Vec3(Selection.LargestPoint.X, position.Y, Selection.LargestPoint.Z);
                    }

                    if (position.Y < Selection.SmallestPoint.Y)
                    {
                        Selection.SmallestPoint =
                            new Vec3(Selection.SmallestPoint.X, position.Y, Selection.SmallestPoint.Z);
                    }


                    if (position.Z > Selection.LargestPoint.Z)
                    {
                        Selection.LargestPoint = new Vec3(Selection.LargestPoint.X, Selection.LargestPoint.Y, position.Z);
                    }

                    if (position.Z < Selection.SmallestPoint.Z)
                    {
                        Selection.SmallestPoint =
                            new Vec3(Selection.SmallestPoint.X, Selection.SmallestPoint.Y, position.Z);
                    }

                    return Success($"Expanded selection by {Selection.Blocks().Count - oldSize} blocks");
                }
                else
                {
                    return Error("Block is already in selection");
                }
            }
            else
            {
               Selection.ArbitraryBitmap.Add(new Vec3(position));
               return Success("Expanded selection by 1 block");
            }

            
        }
        
        [Overload]
        OnixCommandOutput Get(OnixCommandOrigin origin, [CommandPath("get")]string get)
        {
            if (Selection.SelectionType == Selection.SelectionTypeEnum.Cuboid)
            {
                return Success($"pos1: ({Selection.pos1.X}, {Selection.pos1.Y}, {Selection.pos1.Z}) \n" +
                               $"pos2: ({Selection.pos2.X}, {Selection.pos2.Y}, {Selection.pos2.Z}) \n" +
                               $"largest: ({Selection.LargestPoint.X}, {Selection.LargestPoint.Y}, {Selection.LargestPoint.Z}) \n"+
                               $"smallest: ({Selection.SmallestPoint.X}, {Selection.SmallestPoint.Y}, {Selection.SmallestPoint.Z} \n" +
                               $"count: {Selection.Blocks().Count}"
                );
            }
            else
            {
                foreach (Vec3 block in Selection.ArbitraryBitmap)
                {
                    Console.WriteLine($"{Selection.ArbitraryBitmap.IndexOf(block)}: {block}");
                }

                return Success($"Printed {Selection.ArbitraryBitmap.Count} blocks");
            }
            
        }
        
        [Overload]
        OnixCommandOutput Cube(OnixCommandOrigin origin, [CommandPath("cube")]string cube, int radius)
        {
            if (Selection.SelectionType == Selection.SelectionTypeEnum.Arbitrary)
            {
                Console.WriteLine("Changed selection type to cube");
                Selection.SelectionType = Selection.SelectionTypeEnum.Cuboid;
            }
            Vec3 position = Onix.LocalPlayer.Position.Floor();
            Selection.pos1 = position - radius;
            Selection.pos2 = position + radius;
            return Success("Set selection");
        }
        
        [Overload]
        OnixCommandOutput Sphere(OnixCommandOrigin origin, [CommandPath("sphere")]string sphere, int radius)
        {
            if (Selection.SelectionType == Selection.SelectionTypeEnum.Cuboid)
            {
                Console.WriteLine("Changed selection type to arbitrary");
                Selection.SelectionType = Selection.SelectionTypeEnum.Arbitrary;
            }

            Selection.ArbitraryBitmap = new List<Vec3>();
            
            Vec3 position = Onix.LocalPlayer.Position.Floor();
            
            for (int x = (int)position.X - radius - 1; x <= (int)position.X + radius + 1; x++)
            {
                for (int y = (int)position.X - radius - 1; y <= (int)position.Y + radius + 1; y++)
                {
                    for (int z = (int)position.Z - radius - 1; z <= (int)position.Z + radius + 1; z++)
                    {
                        if ((position - new Vec3(x, y, z)).Length < radius)
                        {
                            Selection.ArbitraryBitmap.Add(new Vec3(x, y, z));
                        }
                    }
                }   
            }
            return Success($"Set selection to a sphere of radius {radius}");
        }
        
        [Overload]
        OnixCommandOutput Move(OnixCommandOrigin origin, [CommandPath("move")]string move, AxisCalculator.DirectionEnum axis, int distance)
        {
            AxisCalculator.Direction direction = new AxisCalculator.Direction(axis);
            if (Selection.SelectionType == Selection.SelectionTypeEnum.Cuboid)
            {
                Selection.pos1 += direction.DirectionVector * distance;
                Selection.pos2 += direction.DirectionVector * distance;
            }
            else
            {
                for (int i = 0; i < Selection.ArbitraryBitmap.Count; i++)
                {
                    
                    Vec3 point = Selection.ArbitraryBitmap[i];

                    point += direction.DirectionVector * distance;
                    Selection.ArbitraryBitmap[i] = point; 
                }

            }

            return Success($"Moved selection {distance} blocks");
        }
        [Overload]
        OnixCommandOutput Mode(OnixCommandOrigin origin, [CommandPath("mode")]string mode, Selection.SelectionTypeEnum target)
        {
            Selection.SelectionType = target;
            String newType = Selection.SelectionType == Selection.SelectionTypeEnum.Cuboid ? "Cuboid" : "Arbitrary";
            return Success($"Selection type is now {newType}");
        }
        
        [Overload]
        OnixCommandOutput Clear(OnixCommandOrigin origin, [CommandPath("clear")]string clear)
        {
            if (Selection.SelectionType == Selection.SelectionTypeEnum.Arbitrary)
            {
                Selection.ArbitraryBitmap = new List<Vec3>();
            }else if (Selection.SelectionType == Selection.SelectionTypeEnum.Cuboid)
            {
                Selection.pos1 = Vec3.Zero;
                Selection.pos2 = Vec3.Zero;
            }
            String typeCleared = Selection.SelectionType == Selection.SelectionTypeEnum.Cuboid ? "Cuboid" : "Arbitrary";
            return Success($"{typeCleared} cleared");
        }
    }
}