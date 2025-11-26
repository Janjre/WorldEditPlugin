using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands {
    
    public class Copy : OnixCommandBase {
        public Copy() : base("copy", "copies the selection using the built-in structure system", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        [Overload]
        OnixCommandOutput CopyExecute(String name = "clipboard")
        {
            Onix.Game.ExecuteCommand(
                $"structure save {name} {Selection.pos1.X} {Selection.pos1.Y} {Selection.pos1.Z} {Selection.pos2.X} {Selection.pos2.Y} {Selection.pos2.Z}");

            return Success($"Successfully saved region");
            
        }
    }
}