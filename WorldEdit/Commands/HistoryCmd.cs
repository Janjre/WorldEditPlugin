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
            History.RedoHistory = new List<History.HistoryItem>();
            History.RedoHistoryAsBlocks = new List<MyBlock>();
            History.UndoHistory = new List<History.HistoryItem>();
            History.UndoHistoryAsBlocks = new List<MyBlock>();
            
            History.UndoHistory.Add( new History.HistoryItem(0, "Start", false));
            History.RedoHistory.Add (new History.HistoryItem(0, "Start", false));

            return Success("Cleared all history");
        }
        
        [Overload]
        OnixCommandOutput List(OnixCommandOrigin origin, [CommandPath("list")]string list, int depth = 20) {


            if (depth >= History.UndoHistory.Count)
            {
                depth = History.UndoHistory.Count - 1;
            }
            for (int i = 0; i <= depth; i++)
            {
                Console.WriteLine($"{(i == History.undoPoint ? "-> §e" : "")} {i+1}: {History.UndoHistory[i].Text}, uuid: {History.UndoHistory[i].UUID}");
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
        
        [Overload]
        OnixCommandOutput PrintUndoPoint(OnixCommandOrigin origin, [CommandPath("print_undo_point")]string printUndoPoint)
        {
            return Success($"Undo point: {History.undoPoint}");
        }
        
        
        
        
    }
}