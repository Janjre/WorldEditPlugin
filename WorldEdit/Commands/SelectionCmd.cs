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

        private enum Axis
        {
            X,
            Y,
            Z
        }


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
        OnixCommandOutput Near(OnixCommandOrigin origin, [CommandPath("near")]string near, int radius)
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
        OnixCommandOutput Move(OnixCommandOrigin origin, [CommandPath("move")]string move, Axis axis, int distance)
        {
            if (Selection.SelectionType == Selection.SelectionTypeEnum.Cuboid)
            {
                switch (axis)
                {
                    case Axis.X:
                        Selection.pos1.X += distance;
                        Selection.pos2.X += distance;
                        break;
                    case Axis.Y:
                        Selection.pos1.Y += distance;
                        Selection.pos2.Y += distance;
                        break;
                    case Axis.Z:
                        Selection.pos1.Z += distance;
                        Selection.pos2.Z += distance;
                        break;
                }
            }
            else
            {
                for (int i = 0; i < Selection.ArbitraryBitmap.Count; i++)
                {
                    
                    Vec3 point = Selection.ArbitraryBitmap[i];
                    switch (axis)
                    {
                        case Axis.X:
                            point.X += distance;
                            break;
                        case Axis.Y:
                            point.Y += distance;
                            break;
                        case Axis.Z:
                            point.Z += distance;
                            break;
                    }
                    Selection.ArbitraryBitmap[i] = point; 
                }

            }

            return Success($"Moved selection {distance} blocks");
        }
    }
}