using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;
using WorldEdit.Maths;

namespace WorldEdit.Commands {
    
    public class Wall : OnixCommandBase {
        public Wall() : base("wall", "Sets the vertical edges of the selected area with a block to make walls", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        [Overload]
        OnixCommandOutput WallExecute(Block block)
        {
            
            
            long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);

            float distance = (Selection.pos1 - Selection.pos2.WithY(Selection.pos1.Y)).Length;

            for (float y = Selection.SmallestPoint.Y; y <= Selection.LargestPoint.Y; y++)
            {
                for (float t = 0; t <= distance * 10; t++)
                {
                    float usingT = t / 100;
                    Vec3 point = BezierCalculator.Interpolate(Selection.pos1.WithY(y), Selection.pos2.WithY(y), usingT);
                    History.PlaceBlock(block.Name,"[]",point.Floor(),actionId);
                }
            }
            
            
            History.FinishAction(actionId,$"Built a wall of {block.Name}");

            return Success($"Successfully built wall");
            
        }
    }
}