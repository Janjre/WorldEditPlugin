using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;
using WorldEdit.Maths;

namespace WorldEdit.Commands {
    
    public class Extrude : OnixCommandBase {
        public Extrude() : base("extrude", "Extrudes certain blocks in your selection", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        [Overload]
        OnixCommandOutput ExtrudeExecute(Block block, AxisCalculator.DirectionEnum direction,int distance)
        {
            long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);

            AxisCalculator.Direction directionAsClass = new AxisCalculator.Direction(direction);
            
            foreach (Vec3 blockPos in Selection.Blocks())
            {
                if (Onix.Region.GetBlock((int)blockPos.X, (int)blockPos.Y, (int)blockPos.Z).Name == block.Name)
                {
                    for (int n = 1; n <= distance; n++)
                    {
                        Vec3 placementPosition = (directionAsClass.DirectionVector * n) + blockPos.Floor();
                        History.PlaceBlock(block.Name,"[]",placementPosition.Floor(),actionId);
                    }
                }
            }
            
            History.FinishAction(actionId,$"Filled {Selection.Blocks().Count} blocks with {block.Name}");

            return Success($"Successfully filled {Selection.Blocks().Count} blocks");
            
        }
    }
}