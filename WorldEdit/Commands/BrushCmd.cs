using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Items;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;
using WorldEdit.Tool;
using WorldEdit.Tool.Tools;

namespace WorldEdit.Commands {
    
    public class BrushCmd : OnixCommandBase {
        public BrushCmd() : base("brush", "Registers brush tools", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        
        [Overload]
        OnixCommandOutput Register(Item item, Block block, int size, string name = "", string mask = "")
        {
            ToolManager.AddTool(new BrushTool(item,block,size,name,mask));
            return Success($"Added tool {name}");
        }
    }
}