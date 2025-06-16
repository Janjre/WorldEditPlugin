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
using OnixRuntime.Api.UI;
using OnixRuntime.Api.World;

namespace WorldEdit {

    public static class Globals // no i cannot do things without this and yes i am just learning c#
    {
        public static Vec3 pos1 = new Vec3();
        public static Vec3 pos2 = new Vec3();
        public static bool NotInGui = true;
        public static OnixTextbox CommandBox = new OnixTextbox(128, "", "World edit command here");
        public static List<MyBlock> UndoHistoryAsBlocks = new List<MyBlock>(); // list of blocks associated with actions
        public static List<HistoryActions.HistoryItem> UndoHistory = new List<HistoryActions.HistoryItem>(); // list of action numbers and display text
        
        public static List<MyBlock> RedoHistoryAsBlocks = new List<MyBlock>(); // list of blocks associated with actions
        public static List<HistoryActions.HistoryItem> RedoHistory = new List<HistoryActions.HistoryItem>(); // list of action numbers and display text

        
        public static TexturePath UndoIcon = TexturePath.Assets("undoIcon");
        public static TexturePath RedoIcon = TexturePath.Assets("redoIcon");
        public static TexturePath ClearIcon = TexturePath.Assets("clear");

        public static Random MyRandom = new Random();

        public static int undoPoint = 0;

        

        public static string assetsPath;

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
        
        public static (List<string> args, int argCount) SimpleSplit(string input)
        {
            List<string> args = new();
            if (string.IsNullOrWhiteSpace(input))
                return (args, 0);

            int length = input.Length;
            int i = 0;
            int count = 0;

            while (i < length)
            {
               
                while (i < length && char.IsWhiteSpace(input[i]))
                    i++;

                if (i >= length)
                    break;

                
                int start = i;
                while (i < length && !char.IsWhiteSpace(input[i]))
                    i++;

                args.Add(input.Substring(start, i - start));
                count++;  
            }
            
            args.Add("");
            args.Add("");
            args.Add(""); // No more IndexOutOfBounds errors!!1!!!!!

            return (args, count);
        }
        
        public static bool IndexExists<T>(List<T> list, int index)
        {
            return index >= 0 && index < list.Count;
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
    
    

    

    
    
    public class WorldEdit : OnixPluginBase {
        public static WorldEdit Instance { get; private set; } = null!;
        public static WorldEditConfig Config { get; private set; } = null!;
        
        

        public WorldEdit(OnixPluginInitInfo initInfo) : base(initInfo) {
            Instance = this;
            // If you can clean up what the plugin leaves behind manually, please do not unload the plugin when disabling.
            base.DisablingShouldUnloadPlugin = false;
            Globals.assetsPath = Instance.PluginAssetsPath;
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

            Autocomplete.RegisterCommand(Commands.FillInit());
            
            


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
            
            
             
            if (Globals.CommandBox.HasConfirmedText)
            {
                String message = Globals.CommandBox.Text;
                
                String[] splitMessage = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    
                

                foreach (Autocomplete.commandObject command in Autocomplete.commands)
                {
                    if (command.Name == splitMessage[0])
                    {
                        command.OnRan(message);
                    }
                }
                
                Globals.CommandBox.IsEmpty = true;
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
                Globals.CommandBox.Render(commandLine);

                //console area

                string[] splitMessage = Globals.CommandBox.Text.Split(' ');

                
                
                List<String> completionOptions = new List<string>();
                string text = "";
                bool foundOne = false;
                
                foreach (Autocomplete.commandObject command in Autocomplete.commands)
                {
                    if (Globals.CommandBox.Text.StartsWith(command.Name))
                    {
                        foundOne = true;
                        text = command.Name;
                        foreach (string arg in command.Arguments)
                        {
                            text += " " + arg;

                        }

                        // come up with all potential options
                        var (args,argCount) = Globals.SimpleSplit(Globals.CommandBox.Text); // argCount is not 0-based !!

                        if (Globals.IndexExists(command.CompleteOptions, argCount - 1-1))
                        {
                            foreach (string option in command.CompleteOptions[argCount - 1-1])
                            {
                                if (Globals.IndexExists(args, argCount - 1))
                                {
                                    if (option.Contains(args[argCount - 1]))
                                    {
                                        completionOptions.Add(option);
                                    }    
                                }
                                
                            }
                        }
                        else
                        {
                            if (Globals.CommandBox.Text.EndsWith(" "))
                            {
                                if (Globals.IndexExists(command.CompleteOptions, argCount - 1))
                                {
                                    completionOptions = command.CompleteOptions[argCount - 1];
                                    Autocomplete.AutoCompleteSelection = completionOptions[0];
                                }
                            }
                        }
                        
                        



                    }
                }
                
                

                if (foundOne == false)
                {
                    
                    //display all of the commands with splitMessage[0].Contains
                    foreach (Autocomplete.commandObject command in Autocomplete.commands)
                    {
                        
                        if (splitMessage.Length > 0)
                        {
                            if (command.Name.Contains(splitMessage[0]))
                            {
                                completionOptions.Add(command.Name);
                            }
                            
                        }
                    }
                }
                
                Rect syntaxLine = new Rect(new Vec2(screenWidth * 0.15f, screenHeight * 0.71f), new Vec2(screenWidth * 0.58f, screenHeight * 0.78f));
                Onix.Render.Direct2D.RenderText(syntaxLine,ColorF.White, text,TextAlignment.Left,TextAlignment.Top,1f);


                
                
                //create lots of textboxes for autocomplete/previous commands to go

                Vec2 tlTextArea = new Vec2(screenWidth * 0.15f, screenHeight * 0.12f);
                Vec2 brTextArea = new Vec2(screenWidth * 0.58f, screenHeight * 0.69f);
                Rect textArea = new Rect(tlTextArea, brTextArea);

                

                float lineHeight = 7f; 
                float startY = screenHeight * 0.68f;

                for (int i = 0; i < completionOptions.Count; i++)
                {
                    string option = completionOptions[i];
    
                    Vec2 topLeft = new Vec2(screenWidth * 0.15f, startY - i * lineHeight);
                    Vec2 bottomRight = new Vec2(screenWidth * 0.58f, topLeft.Y + lineHeight);
                    Rect textBox = new Rect(topLeft, bottomRight);

                    if (textBox.TopLeft.Y < textArea.TopLeft.Y)
                    {
                        break;
                    }

                    Onix.Render.Direct2D.RenderText(textBox, ColorF.White, option, TextAlignment.Left, TextAlignment.Top, 1f);
                }










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
            Globals.CommandBox.IsFocused = !Globals.NotInGui;
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

                if (Onix.Gui.ScreenName == "hud_screen" && key.Value == InputKey.Type.Tab && isDown &&
                    Globals.NotInGui == false)
                {
                    var (args,argCount) = Globals.SimpleSplit(Globals.CommandBox.Text); // argCount is not 0-based !!
                    
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