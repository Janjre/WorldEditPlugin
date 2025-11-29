using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands {
    
    public class Move : OnixCommandBase {
        public Move() : base("move", "Moved the blocks within your selection. Does NOT copy NBT data or block states. Use structure block or copy and paste on large or complex builds", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        [Overload]
        OnixCommandOutput MoveExecute(int x, int y, int z)
        {

            long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);

            List<(string, Vec3)> blocks = new List<(string, Vec3)>();

            foreach (Vec3 blockPos in Selection.Blocks())
            {
                blocks.Add((Onix.Region.GetBlock(new BlockPos(blockPos)).Name,blockPos));
                History.PlaceBlock("air","[]",blockPos,actionId);
            }

            foreach ((string,Vec3) block in blocks)
            {
                Vec3 newPlace = block.Item2 + new Vec3(x, y, z);
                History.PlaceBlock(block.Item1, "[]", newPlace, actionId);
            }
            
            History.FinishAction(actionId,$"Moved selection");

            return Success($"Successfully moved {Selection.Blocks().Count} blocks");
            
        }
    }
}