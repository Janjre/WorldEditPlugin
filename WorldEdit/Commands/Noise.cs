using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands {
    

    public class Noise : OnixCommandBase {
        public Noise() : base("noise", "Sets the selected area with a noise pattern", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }
        


        [Overload]
        OnixCommandOutput Perlin(OnixCommandOrigin origin, [CommandPath("perlin")]string perlin, string pattern, float zoom) {
            long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);
            foreach (Vec3 blockPos in Selection.Blocks())
            {
                string output = "";
                bool sucess = false;
                (sucess,output) = NoiseCalculator.Noise((int)blockPos.X, (int)blockPos.Y, (int)blockPos.Z, pattern,zoom,"perlin", actionId);
                if (sucess == false)
                {
                    return Error("Invalid noise pattern");
                }
                
                History.PlaceBlock(output,"[]",blockPos,actionId);
            }

            return Success($"Successfully filled {Selection.Blocks().Count} blocks");
        }
        
        [Overload]
        OnixCommandOutput White(OnixCommandOrigin origin, [CommandPath("white")]string white, string pattern) {
            long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);
            foreach (Vec3 blockPos in Selection.Blocks())
            {
                string output = "";
                bool sucess = false;
                (sucess,output) = NoiseCalculator.Noise((int)blockPos.X, (int)blockPos.Y, (int)blockPos.Z, pattern,0,"white", actionId);
                if (sucess == false)
                {
                    return Error("Invalid noise pattern");
                }
                
                History.PlaceBlock(output,"[]",blockPos,actionId);
            }
        
            return Success($"Successfully filled {Selection.Blocks().Count} blocks");
        }
        
        [Overload]
        OnixCommandOutput Rough(OnixCommandOrigin origin, [CommandPath("rough")]string rough, string pattern, float zoom) {
            long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);
            foreach (Vec3 blockPos in Selection.Blocks())
            {
                string output = "";
                bool sucess = false;
                (sucess,output) = NoiseCalculator.Noise((int)blockPos.X, (int)blockPos.Y, (int)blockPos.Z, pattern,zoom,"rough", actionId);
                if (sucess == false)
                {
                    return Error("Invalid noise pattern");
                }
                
                History.PlaceBlock(output,"[]",blockPos,actionId);
            }
        
            return Success($"Successfully filled {Selection.Blocks().Count} blocks");
        }
    }
}