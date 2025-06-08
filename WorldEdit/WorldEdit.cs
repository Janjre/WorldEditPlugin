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

    public static class Globals // no i cannot do things without this
    {
        public static Vec3 pos1 = new Vec3();
        public static Vec3 pos2 = new Vec3();
        public static bool NotInGui = true;
        public static OnixTextbox CommandTypyBox = new OnixTextbox(128, "", "World edit command here");
        public static List<MyBlock> HistoryAsBlocks = new List<MyBlock>(); // list of blocks associated with actions
        public static List<(long Id, string Name)> History = new List<(long Id, string Name)>(); // list of action numbers and display text


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
        public static void PlaceBlock(String blockName, String data, Vec3 position, long actionNumber)
        {
            Block block = Onix.LocalPlayer.Region.GetBlock((int)position.X, (int)position.Y, (int)position.Z);
            MyBlock blockReplaced = new MyBlock(block.NameFull, block.States.ToString(), actionNumber, position);
            Globals.HistoryAsBlocks.Add(blockReplaced);
            Onix.Client.ExecuteCommand("execute setblock " + position.X + " " + position.Y + " " + position.Z + " " + blockName + " " + data, true);
        }

        public static void FinishAction(long actionNumber,string displayText)
        {
            Globals.History.Add((actionNumber,displayText));
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
                        Console.WriteLine("Got this far");

                        long actionId = new Random().NextInt64(1, 1_000_000_001);
                        
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
            if (!Globals.NotInGui)
            {
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
                
                for (int i = 0; i <= 1000; i++)
                {
                    
                    Vec2 tlTextPos = new Vec2 (screenWidth*0.67f, startIterationsPosition + characterHeight * i);
                    Vec2 brTextPos = new Vec2 (screenWidth*0.83f, startIterationsPosition +(characterHeight*i) + characterHeight);
                    
                    if (brTextPos.Y >= endPoint)
                    {
                        break;
                    }
                    if (i >= 0 && i < Globals.History.Count)
                    {
                        Onix.Render.Direct2D.RenderText(tlTextPos,ColorF.White,Globals.History[i].Name,TextAlignment.Left,TextAlignment.Top,characterHeight/6);
                    }
                    
                }
                



            } 
            Globals.CommandTypyBox.IsFocused = !Globals.NotInGui;
        }
        
        private bool OnInput(InputKey key, bool isDown) {
            
            if (isDown && Onix.Gui.MouseGrabbed)
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