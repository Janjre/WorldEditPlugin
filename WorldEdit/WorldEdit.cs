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

    public static class Globals // no i cannot do things without this
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

        public static bool Shifting;

        

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
            Onix.Events.Common.HudRenderDirect2D += Gui.OnHudRenderDirect2D;
            Onix.Events.Input.Input += InputHandler.OnInput;
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
            Onix.Events.Common.HudRenderDirect2D -= Gui.OnHudRenderDirect2D;
            Onix.Events.Input.Input -= InputHandler.OnInput;
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

        
        
        
        
        
        // bool MyChatHandler(ServerPlayer player, string message)
        // {
        //     
        // }
        
    } 
}