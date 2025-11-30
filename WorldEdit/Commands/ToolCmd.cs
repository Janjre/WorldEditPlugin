using System.Text.Json;
using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Items;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.Utils;
using OnixRuntime.Api.World;
using WorldEdit.Tool;

namespace WorldEdit.Commands {
    

    public class ToolCmd : OnixCommandBase {
        public ToolCmd() : base("tool", "provides various commands related to the tool system", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }
        


        [Overload]
        OnixCommandOutput Item(OnixCommandOrigin origin, [CommandPath("item")]string item, [SoftEnumName("tools")] string tool, Item newItem)
        {
            
            BaseTool selectedTool = ToolManager.RegisteredTools.First(x => x.Name == tool);
            
            selectedTool.Item = newItem.Name;

            return Success($"Changed {tool} to {selectedTool.Item}");

        }
        
        [Overload]
        OnixCommandOutput Remove(OnixCommandOrigin origin, [CommandPath("remove")]string remove, [SoftEnumName("tools")] string tool)
        {
            
            BaseTool selectedTool = ToolManager.RegisteredTools.First(x => x.Name == tool);

            if (selectedTool.Removable)
            {
                ToolManager.RegisteredTools.Remove(selectedTool);
                CommandEnumRegistry.RemoveSoftEnumValue("tools",selectedTool.Name);

                if (ToolManager.SelectedTool == selectedTool)
                {
                    ToolManager.SelectedTool = ToolManager.RegisteredTools.First(x => x.Name == "selection");
                }
                if (ToolManager.WhenSelectingToolSelectedTool == selectedTool)
                {
                    ToolManager.WhenSelectingToolSelectedTool = ToolManager.RegisteredTools.First(x => x.Name == "selection");
                }
                return Success("Removed tool");
            }
            else
            {
                return Error("You cannot remove this tool");
            }

        }
        
        [Overload]
        OnixCommandOutput SmoothRadius(OnixCommandOrigin origin, [CommandPath("smooth_radius")]string smoothRadius, int radius)
        {

            SmoothToolManager.Size = radius;

            return Success("Changed tool radius");

        }
        
        [Overload]
        OnixCommandOutput SmoothKernel(OnixCommandOrigin origin, [CommandPath("large_kernel")]string smoothRadius, bool isLarge)
        {
            SmoothToolManager.LargeKernel = isLarge;

            return Success("Changed kernel size");
        }
        
        [Overload]
        OnixCommandOutput ShowAllTools(OnixCommandOrigin origin, [CommandPath("show_all_tools")]string showAllTools, bool show)
        {
            ToolManager.showAllTools = show;

            return Success(show ? "Shown all tools" : "Shown only selected tool");
        }
        
        [Overload]
        OnixCommandOutput CopyTenthSlotInfo(OnixCommandOrigin origin, [CommandPath("copy_info")]string copyInfo)
        {
            Clipboard.SetText($"Screen size: X {Onix.Gui.ScreenSize.X}, Y {Onix.Gui.ScreenSize.Y}" +
                              $"Tenth slot X: {SlotDrawer.StartX}" +
                              $"Tenth slot Y {SlotDrawer.StartY}" +
                              $"Tenth slot size {SlotDrawer.Size}");

            return Success("Copied info to clipboard");
        }
        
        [Overload]
        OnixCommandOutput AutoFit(OnixCommandOrigin origin, [CommandPath("auto_fit")]string autoFit)
        {
            Console.WriteLine(Onix.Gui.ScreenSize);
            WorldEdit.Config.TenthSlotX = SlotDrawer.GetTenthSlot(Onix.Gui.ScreenSize.X, Onix.Gui.ScreenSize.Y).pos.X;
            WorldEdit.Config.TenthSlotY = SlotDrawer.GetTenthSlot(Onix.Gui.ScreenSize.X, Onix.Gui.ScreenSize.Y).pos.Y;
            WorldEdit.Config.TenthSlotSize = SlotDrawer.GetTenthSlot(Onix.Gui.ScreenSize.X, Onix.Gui.ScreenSize.Y).size;
            return Success("Fitted tenth slot");
        }

        
        
        

        
    }
}