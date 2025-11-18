using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;
using WorldEdit.Maths;

namespace WorldEdit.Commands {
    
    public class Line : OnixCommandBase {
        public Line() : base("line", "Creates a line between pos1 and pos2", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        [Overload]
        OnixCommandOutput LineExecute(Block block)
        {
            long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);

            float distance = (Selection.pos1 - Selection.pos2).Length;

            for (float t = 0; t <= distance * 100; t++)
            {
                float usingT = t / 100;
                Vec3 point = BezierCalculator.Interpolate(Selection.pos1, Selection.pos2, usingT);
                History.PlaceBlock(block.Name,"[]",point.Floor(),actionId);
            }
            
            History.FinishAction(actionId,$"Filled {Selection.Blocks().Count} blocks with {block.Name}");

            return Success($"Successfully filled {Selection.Blocks().Count} blocks");
            
        }
    }
}