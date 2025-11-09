using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands {
    
    public class Up : OnixCommandBase {
        public Up() : base("up", "teleports you upward and places a glass block below you", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        [Overload]
        OnixCommandOutput UpExecute(int distance)
        {
            
            Onix.Game.ExecuteCommand($"tp ~ ~{distance} ~");
            Onix.Game.ExecuteCommand($"setblock ~ ~-1 ~ glass");
            Onix.Game.ExecuteCommand($"setblock ~ ~ ~ air");
            Onix.Game.ExecuteCommand($"setblock ~ ~1 ~ air");
            

            return Success($"Successfully teleported {distance} blocks");
            
        }
    }
}