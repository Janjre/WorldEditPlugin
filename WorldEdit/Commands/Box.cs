using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands {
    
    public class Box : OnixCommandBase {
        public Box() : base("box", "Sets alls the edges of the selected area with a block to make walls", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        [Overload]
        OnixCommandOutput BoxExecute(Block block)
        {
            long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);
            int count = 0;
            foreach (Vec3 blockPos in Selection.Blocks())
            {
                if (((int)blockPos.X == (int)Selection.SmallestPoint.X || (int)blockPos.X == (int)Selection.LargestPoint.X) ||
                    ((int)blockPos.Z == (int)Selection.SmallestPoint.Z || (int)blockPos.Z == (int)Selection.LargestPoint.Z) ||
                    ((int)blockPos.Y == (int)Selection.SmallestPoint.Y || (int)blockPos.Y == (int)Selection.LargestPoint.Y)){
                    History.PlaceBlock(block.Name,"[]"  ,blockPos,actionId);
                    count++;
                }
            }
            
            History.FinishAction(actionId,$"Created box around selection out of {block.Name}");

            return Success($"Successfully filled {count} blocks");
            
        }
    }
}