using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands
{

    public class Redo : OnixCommandBase
    {
        public Redo() : base("redo", "redoes last action", CommandExecutionTarget.Client, CommandPermissionLevel.Any)
        {
        }

        [Overload]
        OnixCommandOutput RedoExecute()
        {
            History.undoPoint += 1;

            long targetUUID = History.UndoHistory[History.undoPoint].UUID;
            string targetActionDescriptor = History.UndoHistory[History.undoPoint].Text;
            
            foreach (MyBlock block in History.RedoHistoryAsBlocks)
            {
                if (block.Action == targetUUID)
                {
                    Onix.Client.ExecuteCommand("execute setblock " + block.Position.X + " " + block.Position.Y +
                                               " " + block.Position.Z + " " + block.Name);
                }
            }

            if (History.undoPoint < 0)
            {
                History.undoPoint = 0;
            }

            if (History.undoPoint > History.UndoHistory.Count)
            {
                History.undoPoint = History.UndoHistory.Count;
            }

            return Success($"Redone {targetActionDescriptor}");
            
        }
        
    }
}