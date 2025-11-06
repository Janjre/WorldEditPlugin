using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands
{

    public class Undo : OnixCommandBase
    {
        public Undo() : base("undo", "undoes last action", CommandExecutionTarget.Client, CommandPermissionLevel.Any)
        {
        }

        [Overload]
        OnixCommandOutput UndoExecute()
        {

            long targetUuid = History.UndoHistory[History.undoPoint].UUID;
            string targetActionDescriptor = History.UndoHistory[History.undoPoint].Text;

            foreach (MyBlock block in History.UndoHistoryAsBlocks)
            {
                if (block.Action == targetUuid)
                {
                    Onix.Client.ExecuteCommand("execute setblock " + block.Position.X + " " + block.Position.Y +
                                               " " + block.Position.Z + " " + block.Name);
                }
            }

            History.undoPoint -= 1;
            if (History.undoPoint < 0)
            {
                History.undoPoint = 0;
            }

            if (History.undoPoint > History.UndoHistory.Count)
            {
                History.undoPoint = History.UndoHistory.Count;
            }

            return Success($"Undone {targetActionDescriptor}");
        }
    }
}