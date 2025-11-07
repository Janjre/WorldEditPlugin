using System.Text.Json;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands {
    

    public class HistoryCmd : OnixCommandBase {
        public HistoryCmd() : base("history", "provides various tools related to the undo/redo commands", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }
        


        [Overload]
        OnixCommandOutput Clear(OnixCommandOrigin origin, [CommandPath("clear")]string clear) {
            History.undoPoint = 0;
            for (int i = History.UndoHistory.Count - 1; i >= 1; i--)
            {
                History.UndoHistory.RemoveAt(i);
                CommandEnumRegistry.RemoveSoftEnumValue("actions",History.UndoHistory[i].UUID.ToString());
            }

            return Success("Cleared all history");
        }
        
        [Overload]
        OnixCommandOutput List(OnixCommandOrigin origin, [CommandPath("list")]string list, int depth = 20) {


            if (depth >= History.UndoHistory.Count)
            {
                depth = History.UndoHistory.Count - 1;
            }
            for (int i = 0; i < depth; i++)
            {
                Console.WriteLine($"{(History.UndoHistory[i].Selected ? "-> §e" : "")} {i+1}: {History.UndoHistory[i].Text}, uuid: {History.UndoHistory[i].UUID}");
            }

            return Success("");
        }
        
        [Overload]
        OnixCommandOutput Info(OnixCommandOrigin origin, [CommandPath("info")]string list, [SoftEnumName("actions")] string action) {
            var targetItem = History.UndoHistory.FirstOrDefault(x => x.UUID.ToString() == action);
            
            List<MyBlock> targetBlocks = History.UndoHistoryAsBlocks.Where(x => x.Action == targetItem.UUID).ToList();
            
            return Success($"{targetItem.UUID}: desc: {targetItem.Text}, numberOfBlocks: {targetBlocks.Count}");
            // return Success("");
        }
        
        // [Overload]
        // OnixCommandOutput Export(OnixCommandOrigin origin, [CommandPath("export")]string list,string fileName)
        // {
        //
        //     var all = (History.UndoHistory, History.UndoHistoryAsBlocks);
        //     String json = JsonSerializer.Serialize(all);
        //     File.WriteAllText(fileName,json);
        //
        //     return Success($"Sucessfully put history into {fileName}");
        // }
        
    }
}