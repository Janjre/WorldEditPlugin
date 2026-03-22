using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands {
    
    public class Set : OnixCommandBase {
        public Set() : base("set", "Sets the selected area with a block", CommandExecutionTarget.Client) { }

        [Overload]
        OnixCommandOutput SetExecute(Block block)
        {
            long actionId = History.GenerateId();
            foreach (Vec3 blockPos in Selection.Blocks())
            {
                History.PlaceBlock(block.Name,"[]"  ,blockPos,actionId);
            }
            
            History.FinishAction(actionId,$"Filled {Selection.Blocks().Count} blocks with {block.Name}");

            return Success($"Successfully filled {Selection.Blocks().Count} blocks");
            
        }
    }
}