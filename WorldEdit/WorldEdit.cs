using System.ComponentModel;
using System.Runtime.InteropServices.JavaScript;
using System.Xml;
using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Events;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Plugin;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.UI;
using OnixRuntime.Api.World;
using WorldEdit.Tools.Tools;


namespace WorldEdit {

    

    
    
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
            Config = new WorldEditConfig();
            // Onix.Events.Common.Tick += OnTick;
            // Onix.Events.Common.HudRender += OnHudRender;
            Onix.Events.Common.WorldRender += OnWorldRender;
            Onix.Events.Input.Input += InputHandler.OnInput;
            Onix.Events.LocalServer.PlayerChatEvent += MyChatHandler;
            
            History.UndoHistory.Add( new History.HistoryItem(0, "Start", false));
            History.RedoHistory.Add (new History.HistoryItem(0, "Start", false));
            
            
            Onix.Client.CommandRegistry.RegisterCommand(new Commands.Set());
            Onix.Client.CommandRegistry.RegisterCommand(new Commands.Noise());
            Onix.Client.CommandRegistry.RegisterCommand(new Commands.Replace());
            Onix.Client.CommandRegistry.RegisterCommand(new Commands.Undo());
            Onix.Client.CommandRegistry.RegisterCommand(new Commands.Redo());
            Onix.Client.CommandRegistry.RegisterCommand(new Commands.HistoryCmd());
            Onix.Client.CommandRegistry.RegisterCommand(new Commands.Up());
            Onix.Client.CommandRegistry.RegisterCommand(new Commands.SelectionCmd());

            
            CommandEnumRegistry.RegisterSoftEnum("actions", new List<string>());
            CommandEnumRegistry.RegisterSoftEnum("tools", new List<string>());
            
            Tools.ToolManager.AddTool(new SelectionTool());
            Tools.ToolManager.AddTool(new BezierTool());
        }
        
        

        protected override void OnEnabled() {

        }

        protected override void OnDisabled() {

        }

        protected override void OnUnloaded() {
            // Ensure every task or thread is stopped when this function returns.
            // You can give them base.PluginEjectionCancellationToken which will be cancelled when this function returns. 
            Console.WriteLine($"Plugin {CurrentPluginManifest.Name} unloaded!");
            // Onix.Events.Common.Tick -= OnTick;
            // Onix.Events.Common.HudRender -= OnHudRender;
            Onix.Events.Common.WorldRender -= OnWorldRender;
            Onix.Events.Input.Input -= InputHandler.OnInput;
            // Onix.Events.LocalServer.PlayerChatEvent -= MyChatHandler;

            

        }

        private void OnWorldRender(RendererWorld gfx, float delta)
        {

            if (Selection.SelectionType == Selection.SelectionTypeEnum.Cuboid)
            {
                (Vec3 posMin, Vec3 posMax) = Globals.FindExtremes(Selection.pos1, Selection.pos2);
            
                posMax.X += 1;
                posMax.Y += 1;
                posMax.Z += 1;
            
                ColorF colour = ColorF.Black;
                BoundingBox boundingBox = new BoundingBox(posMin, posMax);
                
                Onix.Render.World.RenderBoundingBoxOutline(boundingBox,colour);

            }

            if (Selection.SelectionType == Selection.SelectionTypeEnum.Arbitrary)
            {
                // Easy example: Render a specific block that is not really in the world.
                // Gets the water and fence block we want to render. (the ?? is because the fence changed its name at some point, so this handles both cases)
                Block previewBlock = Onix.World!.BlockRegistry.GetBlock("red_concrete",0);

                // Get the coordinates of where we want to render, in my case it'll be at 5, 5, 5.
                BlockPos fakeBlockRenderPosition = BlockPos.FromString("0 0 0");
                // Start a new mesh builder session with quads.
                using (var session = gfx.NewMeshBuilderSession(MeshBuilderPrimitiveType.Quad, ColorF.White, TexturePath.TerrainAtlas)) {
                    // Get our wonderful block renderer.
                    var blockRenderer = gfx.BlockRenderer;
                    

                    // We'll need to cheat the cache to get the result we want, first we have to reset it.
                    // In this case, we could probably get away with not resetting it, but because of the water
                    // We have to reset it and lie, otherwise it becomes a flat plane.
                    foreach (Vec3 blockPosVec in Selection.ArbitraryBitmap)
                    {
                        BlockPos blockPosition = new(blockPosVec);
                        blockRenderer.ResetCache(blockPosition);
                        blockRenderer.ResetBiomeCache(blockPosition);
                        blockRenderer.Cache.SetBlock(blockPosition, previewBlock);
                        blockRenderer.SetupRenderingForBlock(blockPosition, previewBlock);
                        blockRenderer.UseOccluder = false;
                        blockRenderer.RenderBlockTinted(session.Builder, blockPosition, previewBlock, ColorF.White.WithOpacity(0.1f));
                    }

                    // For lighting, it usually puts UVs from the light texture into the UV1 slot of the vertices.
                    // Since we can't provide it, we make sure it's not rendering from whatever was last bound there.
                    session.Builder.DisableUv1();
                } // Now, if we had anything to render, when that } calls Dispose() it should render our block with the material we picked.
                
                   
            } 

            
            
            

            
            foreach (Tools.BaseTool tool in Tools.ToolManager.RegisteredTools ?? Enumerable.Empty<Tools.BaseTool>())
            {
                if (tool == null)
                {
                    Console.WriteLine("Null tool found in RegisteredTools");
                    continue;
                }

                var heldItemName = Onix.LocalPlayer?.MainHandItem?.Item?.Name;
                if (heldItemName == null)
                {
                    // Probably between worlds or no item in hand
                    continue;
                }

                if (tool.Item == heldItemName)
                {
                    try
                    {
                        tool.OnRender(gfx, delta);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error rendering tool {tool.Item}: {ex}");
                    }
                }
            }


        }

        
        
        
        
        
        bool MyChatHandler(ServerPlayer player, string message)
        {
            // if (message.Contains("Error"))
            // {
            //     return true;
            // }
            // else
            // {
            return false;
            // }
        }
        
    } 
}