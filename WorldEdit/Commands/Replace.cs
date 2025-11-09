using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands {
    
    public class Replace : OnixCommandBase {
        public Replace() : base("replace", "Replaces blocks in the selection without others", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        [Overload]
        OnixCommandOutput SetExecute(Block replaceFrom, Block setTo)
        {
            int blocksPlaced = 0;
            long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);
            foreach (Vec3 blockPos in Selection.Blocks())
            {
                if (Onix.Region.GetBlock((int)blockPos.X, (int)blockPos.Y, (int)blockPos.Z).Name == replaceFrom.Name)
                {
                    History.PlaceBlock(setTo.Name,"[]"  ,blockPos,actionId);
                    blocksPlaced += 1;
                }
                
            }
            
            History.FinishAction(actionId,$"Replaced {blocksPlaced} blocks with {setTo.Name}");

            return Success($"Successfully filled {blocksPlaced} blocks");
            
        }
    }
}