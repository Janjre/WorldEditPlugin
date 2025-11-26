using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands {

    
    public class Paste : OnixCommandBase {
        enum PastePosition
        {
            Selection,
            Player
        }
        
        public Paste() : base("paste", "pastes the selection - cannot be undone", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        [Overload]
        OnixCommandOutput PasteExecute(String name = "clipboard", PastePosition placeToPaste = PastePosition.Selection)
        {
            if (placeToPaste == PastePosition.Selection)
            {
                Onix.Game.ExecuteCommand(
                                $"structure load {name} {Selection.pos1.X} {Selection.pos1.Y} {Selection.pos1.Z}");
            }
            else
            {
                Onix.Game.ExecuteCommand(
                    $"structure load {name} {(int)Onix.LocalPlayer.Position.X} {(int)Onix.LocalPlayer.Position.Y} {(int)Onix.LocalPlayer.Position.Z}");
            }
            

            return Success($"Successfully pasted '{name}' at {(placeToPaste == PastePosition.Player ? "Player" : "Selection")}");
            
        }
    }
}