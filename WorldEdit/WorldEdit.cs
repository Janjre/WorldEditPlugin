using System.ComponentModel;
using System.Runtime.InteropServices.JavaScript;
using System.Xml;
using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Events;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Plugin;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.World;

namespace WorldEdit {

    public static class Globals // no i cannot do things without this and yes i am just learning c#
    {
        public static Vec3 pos1 = new Vec3();
        public static Vec3 pos2 = new Vec3();
        public static bool NotInGui = true;
        public static OnixTextbox CommandTypyBox = new OnixTextbox(128, "", "World edit command here");
        public static List<MyBlock> UndoHistoryAsBlocks = new List<MyBlock>(); // list of blocks associated with actions
        public static List<HistoryActions.HistoryItem> UndoHistory = new List<HistoryActions.HistoryItem>(); // list of action numbers and display text
        
        public static List<MyBlock> RedoHistoryAsBlocks = new List<MyBlock>(); // list of blocks associated with actions
        public static List<HistoryActions.HistoryItem> RedoHistory = new List<HistoryActions.HistoryItem>(); // list of action numbers and display text

        
        public static TexturePath UndoIcon = TexturePath.Assets("undoIcon");
        public static TexturePath RedoIcon = TexturePath.Assets("redoIcon");
        public static TexturePath ClearIcon = TexturePath.Assets("clear");

        public static Random MyRandom = new Random();

        public static int undoPoint = 0; 

        public static bool myContains(Rect rect, Vec2 point)
        {
            if (point.Y > rect.TopLeft.Y && point.Y < rect.BottomLeft.Y && point.X > rect.BottomLeft.X &&
                point.X < rect.BottomRight.X)
            {
                return true;
            }

            return false;
        }
        


        public static (Vec3, Vec3) FindExtremes (Vec3 a, Vec3 b)
        {
            Vec3 posMin = new Vec3(
                Math.Min(a.X, b.X),
                Math.Min(a.Y, b.Y),
                Math.Min(a.Z, b.Z)
            );

            Vec3 posMax = new Vec3(
                Math.Max(a.X, b.X),
                Math.Max(a.Y, b.Y),
                Math.Max(a.Z, b.Z)
            );

            return (posMin, posMax);
        }
        
    }

    public class MyBlock
    {
        public String Name;
        public String Data;
        public Vec3 Position;
        public long Action;

        public MyBlock(String name, String data, long action, Vec3 position)
        {
            Name = name;
            Data = data;
            Action = action;
            Position = position;

        }
    }
    
    

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
                Console.WriteLine("Doing thjings");
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

    public static class Autocomplete
    {

        public static List<commandObject> commands = new List<commandObject>();

        public static void registerCommand(commandObject command)
        {
            commands.Add(command);
        }
        
        
        

        public class commandObject
        {
            public string Name;
            public string Description;
            public List<String> Arguments;
            public List<List<string>> CompleteOptions;
            public Func<string> OnRan;
            
            
            
            
            public commandObject(string name, string description, List<String> argumentHelp,
                List<List<string>> options, Func<string> onRan)
            {
                Name = name;
                Description = description;
                Arguments = argumentHelp;
                CompleteOptions = options;
                OnRan = onRan;
            }
        }
        
        
    }
    
    public class WorldEdit : OnixPluginBase {
        public static WorldEdit Instance { get; private set; } = null!;
        public static WorldEditConfig Config { get; private set; } = null!;

        public WorldEdit(OnixPluginInitInfo initInfo) : base(initInfo) {
            Instance = this;
            // If you can clean up what the plugin leaves behind manually, please do not unload the plugin when disabling.
            base.DisablingShouldUnloadPlugin = false;
#if DEBUG
           // base.WaitForDebuggerToBeAttached();
#endif
        }

    
        protected override void OnLoaded() {
            Console.WriteLine($"Plugin {CurrentPluginManifest.Name} loaded!");
            Config = new WorldEditConfig(PluginDisplayModule);
            Onix.Events.Common.Tick += OnTick;
            // Onix.Events.Common.HudRender += OnHudRender;
            Onix.Events.Common.WorldRender += OnWorldRender;
            Onix.Events.Common.HudRenderDirect2D += OnHudRenderDirect2D;
            Onix.Events.Input.Input += OnInput;
            // Onix.Events.LocalServer.PlayerChatEvent += MyChatHandler;
            Globals.NotInGui = true;

            Globals.UndoHistory.Add( new HistoryActions.HistoryItem(0, "Start", false));
            Globals.RedoHistory.Add (new HistoryActions.HistoryItem(0, "Start", false));


        }
        
        

        protected override void OnEnabled() {

        }

        protected override void OnDisabled() {

        }

        protected override void OnUnloaded() {
            // Ensure every task or thread is stopped when this function returns.
            // You can give them base.PluginEjectionCancellationToken which will be cancelled when this function returns. 
            Console.WriteLine($"Plugin {CurrentPluginManifest.Name} unloaded!");
            Onix.Events.Common.Tick -= OnTick;
            // Onix.Events.Common.HudRender -= OnHudRender;
            Onix.Events.Common.WorldRender -= OnWorldRender;
            Onix.Events.Common.HudRenderDirect2D -= OnHudRenderDirect2D;
            Onix.Events.Input.Input -= OnInput;
            // Onix.Events.LocalServer.PlayerChatEvent -= MyChatHandler;

            

        }
        private void OnTick() {
            
            
             
            if (Globals.CommandTypyBox.HasConfirmedText)
            {

                String message = Globals.CommandTypyBox.Text;
                
                String[] splitMessage = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    
                switch (splitMessage[0])
                {

                    case "fill":

                        long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);
                        
                        (Vec3 posMin, Vec3 posMax) = Globals.FindExtremes(Globals.pos1, Globals.pos2);
                        // loop through area!!!! (this is incredibly unique, i don't think we will do this anywhere else in the plugin!!!!)
                        
                        for (int x = (int)posMin.X; x <= posMax.X; x++)
                        {
                            for (int y = (int)posMin.Y; y <= posMax.Y; y++)
                            {
                                for (int z = (int)posMin.Z; z <= posMax.Z; z++)
                                {
                                    // Console.WriteLine("attempted to do something");
                                    HistoryActions.PlaceBlock(splitMessage[1], "[]",new Vec3(x,y,z),actionId);
                                    
                                }
                            }
                        }
                        HistoryActions.FinishAction(actionId, "Filled area with "+splitMessage[1]);

                        break;
                }

                foreach (Autocomplete.commandObject command in Autocomplete.commands)
                {
                    if (command.Name == splitMessage[0])
                    {
                        command.OnRan(message);
                    }
                }
                
                Globals.CommandTypyBox.IsEmpty = true;
            }

        }



        private void OnWorldRender(RendererWorld gfx, float delta)
        {

            (Vec3 posMin, Vec3 posMax) = Globals.FindExtremes(Globals.pos1, Globals.pos2);
            
            posMax.X += 1;
            posMax.Y += 1;
            posMax.Z += 1;
            
            ColorF colour = ColorF.Black;

            // Vertical edges
            Onix.Render.World.DrawLine(new Vec3(posMin.X, posMin.Y, posMin.Z), new Vec3(posMin.X, posMax.Y, posMin.Z), colour);
            Onix.Render.World.DrawLine(new Vec3(posMax.X, posMin.Y, posMin.Z), new Vec3(posMax.X, posMax.Y, posMin.Z), colour);
            Onix.Render.World.DrawLine(new Vec3(posMin.X, posMin.Y, posMax.Z), new Vec3(posMin.X, posMax.Y, posMax.Z), colour);
            Onix.Render.World.DrawLine(new Vec3(posMax.X, posMin.Y, posMax.Z), new Vec3(posMax.X, posMax.Y, posMax.Z), colour);

            // Bottom face
            Onix.Render.World.DrawLine(new Vec3(posMin.X, posMin.Y, posMin.Z), new Vec3(posMax.X, posMin.Y, posMin.Z), colour);
            Onix.Render.World.DrawLine(new Vec3(posMax.X, posMin.Y, posMin.Z), new Vec3(posMax.X, posMin.Y, posMax.Z), colour);
            Onix.Render.World.DrawLine(new Vec3(posMax.X, posMin.Y, posMax.Z), new Vec3(posMin.X, posMin.Y, posMax.Z), colour);
            Onix.Render.World.DrawLine(new Vec3(posMin.X, posMin.Y, posMax.Z), new Vec3(posMin.X, posMin.Y, posMin.Z), colour);

            // Top face
            Onix.Render.World.DrawLine(new Vec3(posMin.X, posMax.Y, posMin.Z), new Vec3(posMax.X, posMax.Y, posMin.Z), colour);
            Onix.Render.World.DrawLine(new Vec3(posMax.X, posMax.Y, posMin.Z), new Vec3(posMax.X, posMax.Y, posMax.Z), colour);
            Onix.Render.World.DrawLine(new Vec3(posMax.X, posMax.Y, posMax.Z), new Vec3(posMin.X, posMax.Y, posMax.Z), colour);
            Onix.Render.World.DrawLine(new Vec3(posMin.X, posMax.Y, posMax.Z), new Vec3(posMin.X, posMax.Y, posMin.Z), colour);


        }

        private void OnHudRenderDirect2D(RendererDirect2D gfx, float delta) {
            // For example, before the loop:
            

            
            if (!Globals.NotInGui)
            {
                Onix.Render.Direct2D.RenderText(new Vec2(0,0),ColorF.White,Globals.undoPoint.ToString(),1f );
                float screenWidth = Onix.Gui.ScreenSize.X;
                float screenHeight = Onix.Gui.ScreenSize.Y;
                
                Rect consoleArea = new Rect(new Vec2(screenWidth * 0.13f, screenHeight * 0.10f), new Vec2(screenWidth * 0.60f, screenHeight * 0.85f));
                // Onix.Render.Direct2D.DrawRoundedRectangle(myRect, ColorF.Gray, 10,10);
                
                ColorF darkGray = new ColorF(0.1f, 0.1f, 0.1f,0.95f);
                Onix.Render.Direct2D.FillRoundedRectangle(consoleArea, darkGray , 10, 10);
                Onix.Render.Direct2D.DrawRoundedRectangle(consoleArea, ColorF.White , 0.25f, 10);

                ColorF lightGray = new ColorF(0.34f, 0.34f, 0.34f, 1f);
                Rect commandLine = new Rect(new Vec2(screenWidth * 0.15f, screenHeight * 0.75f), new Vec2(screenWidth * 0.58f, screenHeight * 0.82f));
                Onix.Render.Direct2D.FillRoundedRectangle(commandLine, lightGray , 5f, 10);
                Globals.CommandTypyBox.Render(commandLine);

                Rect sidebarArea = new Rect(new Vec2(screenWidth * 0.65f, screenHeight * 0.10f), new Vec2(screenWidth * 0.85f, screenHeight * 0.85f));
                Onix.Render.Direct2D.FillRoundedRectangle(sidebarArea,darkGray,10,10);
                Onix.Render.Direct2D.DrawRoundedRectangle(sidebarArea, ColorF.White , 0.25f, 10);

                float startIterationsPosition = screenHeight * 0.14f;
                float characterHeight = 7;
                float endPoint = screenHeight * 0.83f;
                
                for (int i = 0; i <= Globals.UndoHistory.Count; i++)
                {
                    
                    Vec2 tlTextPos = new Vec2 (screenWidth*0.67f, startIterationsPosition + characterHeight * i);
                    Vec2 brTextPos = new Vec2 (screenWidth*0.83f, startIterationsPosition +(characterHeight*i) + characterHeight);
                    
                    if (brTextPos.Y >= endPoint)
                    {
                        break;
                    }
                    if (i >= 0 && i < Globals.UndoHistory.Count)
                    {
                        
                        // if (Globals.UndoHistory[i].Id)
                        ColorF colour = ColorF.White;
                        if (i == Globals.undoPoint)
                        {
                            
                            colour = ColorF.Red;
                        }
                        Onix.Render.Direct2D.RenderText(tlTextPos,colour,Globals.UndoHistory[i].Text,TextAlignment.Left,TextAlignment.Top,characterHeight/6);
                        // Rect button = new Rect(
                        //     new Vec2(tlTextPos.X- 10, tlTextPos.Y+2),
                        //     new Vec2(tlTextPos.X -2, tlTextPos.Y + 10));
                        //
                        // Globals.RevertButtons[i] = new HistoryActions.HistoryItem(i,false,button,Globals.UndoHistory[i].Id);
                        // Onix.Render.Direct2D.RenderTexture(button,Globals.UndoIcon,1f);
                        
                    }

                    Rect undo = new Rect(new Vec2(screenWidth * 0.80f, screenHeight * 0.12f),
                        new Vec2((screenWidth * 0.80f)+10, (screenHeight * 0.12f)+10));
                    
                    Onix.Render.Direct2D.RenderTexture(undo,Globals.UndoIcon,1f);
                    
                    Rect redo = new Rect(new Vec2(screenWidth * 0.815f, screenHeight * 0.12f),
                        new Vec2((screenWidth * 0.815f)+10, (screenHeight * 0.12f)+10));
                    
                    Onix.Render.Direct2D.RenderTexture(redo,Globals.RedoIcon,1f);
                   
                    Rect clear = new Rect(new Vec2(screenWidth * 0.785f, screenHeight * 0.12f),
                        new Vec2((screenWidth * 0.785f)+9, (screenHeight * 0.12f)+9));
                    
                    Onix.Render.Direct2D.RenderTexture(clear,Globals.ClearIcon,1f);
                }
                
                
                



            } 
            Globals.CommandTypyBox.IsFocused = !Globals.NotInGui;
        }
        
        private bool OnInput(InputKey key, bool isDown) {
            
            if (isDown && Onix.Gui.MouseGrabbed && Globals.NotInGui)
            {
                RaycastResult result = Onix.LocalPlayer.Raycast;
                if (Onix.LocalPlayer.MainHandItem.Item != null)
                {
                    if (Onix.LocalPlayer.MainHandItem.Item.Name == "wooden_axe")
                    {
                        if (key.Value == InputKey.Type.LMB)
                        {
                            Globals.pos1 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y, result.BlockPosition.Z);
                            // Onix.Client.Notify("HAHA BLOCKED INPUT1!!!");
                            return true;
                            
                        }  if (key.Value == InputKey.Type.RMB)
                        {
                            Globals.pos2 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y, result.BlockPosition.Z); 
                            // Onix.Client.Notify("HAHA BLOCKED INPUT2!!!");
                            return true;
                        }

                        if (key.Value == InputKey.Type.MMB)
                        {
                            
                        }
                    }
                }
                
                
            }
            
            if (Onix.Gui.ScreenName == "hud_screen")
            {
                if (key.Value == InputKey.Type.Y && isDown && Globals.NotInGui)
                {
                    Globals.NotInGui = false;
                    Onix.Gui.MouseGrabbed = false;
                }

                if (key.Value == InputKey.Type.Escape && isDown && Globals.NotInGui == false)
                {
                    
                    Globals.NotInGui = true;
                    Onix.Gui.MouseGrabbed = true;
                    return true;
                }
                
                if (Globals.NotInGui == false && key.Value == InputKey.Type.LMB && isDown)
                {
                    Vec2 mouseCursor = Onix.Gui.MousePosition;
                    float screenWidth = Onix.Gui.ScreenSize.X;
                    float screenHeight = Onix.Gui.ScreenSize.Y;
                    
                    Rect undo = new Rect(new Vec2(screenWidth * 0.80f, screenHeight * 0.12f),
                        new Vec2((screenWidth * 0.80f)+15, (screenHeight * 0.12f)+15));
                    
                    
                    
                    Rect redo = new Rect(new Vec2(screenWidth * 0.82f, screenHeight * 0.12f),
                        new Vec2((screenWidth * 0.82f)+15, (screenHeight * 0.12f)+15));
                    
                    Rect clear = new Rect(new Vec2(screenWidth * 0.785f, screenHeight * 0.12f),
                        new Vec2((screenWidth * 0.785f)+9, (screenHeight * 0.12f)+9));

                    Console.WriteLine(Globals.undoPoint);
                    
                    
                    if (Globals.myContains(undo,mouseCursor))
                    {
                        long targetUUID = Globals.UndoHistory[Globals.undoPoint].UUID;
                        
                        foreach (MyBlock block in Globals.UndoHistoryAsBlocks)
                        {
                            if (block.Action == targetUUID)
                            {
                                Onix.Client.ExecuteCommand("execute setblock "+ block.Position.X + " " + block.Position.Y + " " + block.Position.Z + " " + block.Name);
                            }
                        }
                        Globals.undoPoint -= 1;
                        if (Globals.undoPoint < 0)
                        {
                            Globals.undoPoint = 0;
                        }
                    
                        if (Globals.undoPoint > Globals.UndoHistory.Count)
                        {
                            Globals.undoPoint = Globals.UndoHistory.Count;
                        }
                    }
                    
                    if (Globals.myContains(redo, mouseCursor))
                    {
                        
                        Globals.undoPoint += 1;
                        
                        long targetUUID = Globals.UndoHistory[Globals.undoPoint].UUID;
                        foreach (MyBlock block in Globals.RedoHistoryAsBlocks)
                        {
                             if (block.Action == targetUUID)
                             {
                                 Onix.Client.ExecuteCommand("execute setblock "+ block.Position.X + " " + block.Position.Y + " " + block.Position.Z + " " + block.Name);
                             }
                        }
                        if (Globals.undoPoint < 0)
                        {
                            Globals.undoPoint = 0;
                        }
                    
                        if (Globals.undoPoint > Globals.UndoHistory.Count)
                        {
                            Globals.undoPoint = Globals.UndoHistory.Count;
                        }
                        
                    }

                    if (Globals.myContains(clear, mouseCursor))
                    {
                        Globals.undoPoint = 0;
                        for (int i = Globals.UndoHistory.Count-1; i >= 1;i--)
                        {
                            Globals.UndoHistory.RemoveAt(i);
                        }
                    }
                    
                    return true;
                } 
                
                if (Globals.NotInGui == false)
                {
                    return true;
                }
                
            }
            
            
            return false;  
            
        }
        
        
        // bool MyChatHandler(ServerPlayer player, string message)
        // {
        //     
        // }
        
    } 
}