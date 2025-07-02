using OnixRuntime.Api;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.World;

namespace WorldEdit;

public static class History
    {
        public static List<MyBlock> UndoHistoryAsBlocks = new List<MyBlock>(); // list of blocks associated with actions
        public static List<History.HistoryItem> UndoHistory = new List<History.HistoryItem>(); // list of action numbers and display text
        
        public static List<MyBlock> RedoHistoryAsBlocks = new List<MyBlock>(); // list of blocks associated with actions
        public static List<HistoryItem> RedoHistory = new List<History.HistoryItem>(); // list of action numbers and display text

        
        public static TexturePath UndoIcon = TexturePath.Assets("undoIcon");
        public static TexturePath RedoIcon = TexturePath.Assets("redoIcon");
        public static TexturePath ClearIcon = TexturePath.Assets("clear");
        public static int undoPoint = 0;
        
        public static List<string> commandHistory = new List<string>();
        public static int commandHistoryPoint;
        
        
        public static void PlaceBlock(String blockName, String data, Vec3 position, long actionNumber, bool undo = true)
        {
            
            
            Block block = Onix.LocalPlayer.Region.GetBlock((int)position.X, (int)position.Y, (int)position.Z);
            MyBlock blockReplaced = new MyBlock(block.NameFull, block.States.ToString(), actionNumber, position);
            UndoHistoryAsBlocks.Add(blockReplaced);
        
            MyBlock blockPlaced = new MyBlock(blockName, data, actionNumber, position);
            RedoHistoryAsBlocks.Add(blockPlaced);
            
            
            Onix.Client.ExecuteCommand("execute setblock " + position.X + " " + position.Y + " " + position.Z + " " + blockName + " " + data, true);
        }
        
        

        public static void FinishAction(long actionNumber,string displayText) // logic around removing redo options after an action are here
        {
            
            
            
            if (undoPoint != UndoHistory.Count)
            {
                
                for (int i = UndoHistory.Count-1; i >  undoPoint; i--)
                {
                    UndoHistory.RemoveAt(i);
                }
            }

            undoPoint += 1;
            
            
            UndoHistory.Add(new HistoryItem(actionNumber,displayText,true));
            RedoHistory.Add(new HistoryItem(actionNumber, displayText, true));


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
                    foreach (History.HistoryItem item in UndoHistory)
                    {
                        item.Selected = false;
                    }
                    foreach (History.HistoryItem item in RedoHistory)
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