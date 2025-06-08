using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Events;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Plugin;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.World;

namespace WorldEdit {

    public static class Globals
    {
        public static Vec3 pos1 = new Vec3();
        public static Vec3 pos2 = new Vec3();
        public static bool InGui = true;
        public static OnixTextbox CommandTypyBox = new OnixTextbox(128, "", "World edit command here");
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
            Onix.Events.LocalServer.PlayerChatEvent += MyChatHandler;
            
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
            Onix.Events.LocalServer.PlayerChatEvent -= MyChatHandler;

            

        }
        private void OnTick() {
            
             
            if (Globals.CommandTypyBox.HasConfirmedText)
            {
                
                Console.WriteLine(Globals.CommandTypyBox.Text);
                
                
                
                Globals.CommandTypyBox.IsEmpty = true;
            }

            Console.WriteLine(Onix.Gui.ScreenName);
            // Console.WriteLine(Globals.InGui);
        }

        // private void OnHudRender(RendererGame gfx, float delta) {
        //     
        // }

        private void OnWorldRender(RendererWorld gfx, float delta) {
            Vec3 posMin = new Vec3(
                Math.Min(Globals.pos1.X, Globals.pos2.X),
                Math.Min(Globals.pos1.Y, Globals.pos2.Y),
                Math.Min(Globals.pos1.Z, Globals.pos2.Z)
            );

            Vec3 posMax = new Vec3(
                Math.Max(Globals.pos1.X, Globals.pos2.X) + 1,
                Math.Max(Globals.pos1.Y, Globals.pos2.Y) + 1,
                Math.Max(Globals.pos1.Z, Globals.pos2.Z) + 1
            );
            
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
            if (!Globals.InGui)
            {
                Rect myRect = new Rect(new Vec2(90,30), new Vec2(585,245));
                // Onix.Render.Direct2D.DrawRoundedRectangle(myRect, ColorF.Gray, 10,10);
                
                ColorF darkGray = new ColorF(0.1f, 0.1f, 0.1f,0.95f);
                Onix.Render.Direct2D.FillRoundedRectangle(myRect, darkGray , 10, 10);
                Onix.Render.Direct2D.DrawRoundedRectangle(myRect, ColorF.White , 0.25f, 10);

                ColorF lightGray = new ColorF(0.34f, 0.34f, 0.34f, 1f);
                Rect commandLine = new Rect(new Vec2(100,215), new Vec2(575, 235));
                Onix.Render.Direct2D.FillRoundedRectangle(commandLine, lightGray , 5f, 10);
                Globals.CommandTypyBox.Render(commandLine);
                
                

            } 
            Globals.CommandTypyBox.IsFocused = !Globals.InGui;
        }
        
        private bool OnInput(InputKey key, bool isDown) {
            if (Onix.Gui.MouseGrabbed == false)
            {
                return false;
            }
            if (isDown)
            {
                RaycastResult result = Onix.LocalPlayer.Raycast;
                if (Onix.LocalPlayer.MainHandItem.IsEmpty == false)
                {
                    if (Onix.LocalPlayer.MainHandItem.Item.Name == "wooden_axe")
                    {
                        if (key.Value == InputKey.Type.LMB)
                        {
                            Globals.pos1 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y, result.BlockPosition.Z);
                            return true;
                            
                        }  if (key.Value == InputKey.Type.RMB)
                        {
                            Globals.pos2 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y, result.BlockPosition.Z);    
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
                if (key.Value == InputKey.Type.Y && isDown && Globals.InGui)
                {
                    Globals.InGui = !Globals.InGui;
                    Onix.Gui.MouseGrabbed = false;
                }

                if (key.Value == InputKey.Type.Escape && isDown)
                {
                    // return true;
                    Globals.InGui = true;
                    Onix.Gui.MouseGrabbed = true;
                }
                if (Globals.InGui == false)
                {
                    // return true;
                }
            }
            
            
            return false;  
            
        }
        
        
        bool MyChatHandler(ServerPlayer player, string message)
        {
            if (Onix.LocalPlayer != null)
            {
                // Console.WriteLine($"{player} said: {message}");
                
                if (message[0] == '$')
                {
                    String[] splitMessage = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    
                    switch (splitMessage[0])
                    {
                        
                        case "$fill":
                            
                            Onix.Game.ExecuteCommand("fill "+Globals.pos1.X + " " + Globals.pos1.Y + " " + Globals.pos1.Z + " " + Globals.pos2.X + " " + Globals.pos2.Y + " " + Globals.pos2.Z +" " + splitMessage[1]);
                            return true;
                            break;
                    }
                    return true;    
                }
            } 
            return false;
            
        }
        
    } 
}