using OnixRuntime.Api;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.World;

namespace WorldEdit;

public static class HistoryActions
    {
        public static void PlaceBlock(String blockName, String data, Vec3 position, long actionNumber, bool undo = true)
        {
            
            
            Block block = Onix.LocalPlayer.Region.GetBlock((int)position.X, (int)position.Y, (int)position.Z);
            MyBlock blockReplaced = new MyBlock(block.NameFull, block.States.ToString(), actionNumber, position);
            Globals.UndoHistoryAsBlocks.Add(blockReplaced);
        
            MyBlock blockPlaced = new MyBlock(blockName, data, actionNumber, position);
            Globals.RedoHistoryAsBlocks.Add(blockPlaced);
            
            
            Onix.Client.ExecuteCommand("execute setblock " + position.X + " " + position.Y + " " + position.Z + " " + blockName + " " + data, true);
        }
        
        

        public static void FinishAction(long actionNumber,string displayText) // logic around removing redo options after an action are here
        {
            
            
            
            if (Globals.undoPoint != Globals.UndoHistory.Count)
            {
                
                for (int i = Globals.UndoHistory.Count-1; i >  Globals.undoPoint; i--)
                {
                    Globals.UndoHistory.RemoveAt(i);
                }
            }

            Globals.undoPoint += 1;
            
            
            Globals.UndoHistory.Add(new HistoryItem(actionNumber,displayText,true));
            Globals.RedoHistory.Add(new HistoryItem(actionNumber, displayText, true));


        }

        public class HistoryItem
        {
            public long UUID;
            public string Text;
            public bool Selected;

            public HistoryItem(long uuid, string text, bool selected)
            {
                if (selected)
                {
                    foreach (HistoryActions.HistoryItem item in Globals.UndoHistory)
                    {
                        item.Selected = false;
                    }
                    foreach (HistoryActions.HistoryItem item in Globals.RedoHistory)
                    {
                        item.Selected = false;
                    }
                }
                UUID = uuid;
                Text = text;
                Selected = selected;

            }
        }

        
        
        
            
    }