using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands {
    
    public class Sphere : OnixCommandBase {
        public Sphere() : base("sphere", "Creates a sphere with centre pos1 and radius pos2", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        [Overload]
        OnixCommandOutput SphereExecute(Block block, bool fill = true)
        {
            long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);

            int radius = (int)(Selection.pos2 - Selection.pos1).Length;
            for (int x = (int)Selection.pos1.X - radius - 1; x <= (int)Selection.pos1.X + radius + 1; x++)
            {
                for (int y = (int)Selection.pos1.Y - radius - 1; y <= (int)Selection.pos1.Y + radius + 1; y++)
                {
                    for (int z = (int)Selection.pos1.Z - radius - 1; z <= (int)Selection.pos1.Z + radius + 1; z++)
                    {
                        if (fill)
                        {
                            if ((Selection.pos1 - new Vec3(x, y, z)).Length < radius)
                            {
                                History.PlaceBlock(block.Name,"[]",new Vec3(x,y,z),actionId);
                            }
                        }
                        else
                        {
                            if ((int)((Selection.pos1 - new Vec3(x, y, z)).Length) ==radius)
                            {
                                History.PlaceBlock(block.Name,"[]",new Vec3(x,y,z),actionId);
                            }
                        }
                        
                    }
                }   
            }
            
            History.FinishAction(actionId,$"Filled {Selection.Blocks().Count} blocks with {block.Name}");

            return Success($"Successfully filled {Selection.Blocks().Count} blocks");
            
        }
    }
}