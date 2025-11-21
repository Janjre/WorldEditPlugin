using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.Rendering;

namespace WorldEdit.Tools;

public static class ToolManager
{
    public static List<BaseTool> RegisteredTools = new List<BaseTool>();
    
    public static bool ToolSlotSelected = false;
    
    
    public static void AddTool(BaseTool baseTool)
    {
        RegisteredTools.Add(baseTool);
        CommandEnumRegistry.AddSoftEnumValue("tools",baseTool.Name);
    }

    public static void RemoveTool(BaseTool baseTool)
    {
        RegisteredTools.Remove(baseTool);
        CommandEnumRegistry.RemoveSoftEnumValue("tools",baseTool.Name);
    }

    // public static bool RenderTenthSlot(RendererCommon2D gfx)
    // {
    //     float screenWidth = Onix.Gui.ScreenSize.X;
    //     float screenHeight = Onix.Gui.ScreenSize.Y;
    //     
    //     Vec2 topLeft = new Vec2(screenWidth * 0.63794f, screenHeight * 0.9230f);
    //     Vec2 sizeVec = new Vec2(screenWidth * 0.02920f, screenWidth * 0.02920f);
    //
    //     Rect tenthSlot = new Rect(topLeft, topLeft + sizeVec);
    //     
    //     Vec2 expandFactor   = new Vec2(sizeVec.X / 16f, sizeVec.Y / 16f);
    //     Vec2 expandedTopLeft = topLeft - expandFactor;
    //     Vec2 expandedSize    = sizeVec + expandFactor * 2f;
    //
    //     Rect tenthSlotExpanded = new Rect(expandedTopLeft, expandedTopLeft + expandedSize);
    //
    //     TexturePath unselectedTexture = TexturePath.Game("textures/ui/hotbar_0.png");
    //     TexturePath selectedTexture = TexturePath.Game("textures/ui/selected_hotbar_slot.png");
    //
    //     gfx.RenderTexture(tenthSlot,unselectedTexture);
    //     
    //     if (ToolSlotSelected)
    //     {
    //         gfx.RenderTexture(tenthSlotExpanded,selectedTexture);
    //     }
    //     
    //     
    //     
    //     
    //     // gfx.DrawRectangle(tenthSlot,ColorF,1);
    //     return true;
    // }
    //
    // private static bool IsKeyNumber(InputKey key)
    // {
    //     // if (key == Module.HotbarSlot1.Value) { return true; }
    //     // if (key == Module.HotbarSlot2.Value) { return true; }
    //     // if (key == Module.HotbarSlot3.Value) { return true; }
    //     // if (key == Module.HotbarSlot4.Value) { return true; }
    //     // if (key == Module.HotbarSlot5.Value) { return true; }
    //     // if (key == Module.HotbarSlot6.Value) { return true; }
    //     // if (key == Module.HotbarSlot7.Value) { return true; }
    //     // if (key == Module.HotbarSlot8.Value) { return true; }
    //     // if (key == Module.HotbarSlot9.Value) { return true; }
    //
    //     return false;
    // }
    //
    // public static bool OnKeyPressed(InputKey key, bool isDown)
    // {
    //     // if (key == Module.HotbarSlot10.Value && isDown)
    //     // {
    //         // ToolSlotSelected = true;
    //         // return true;
    //     // }
    //
    //     if (IsKeyNumber(key))
    //     {
    //         ToolSlotSelected = false;
    //         return false;
    //     }
    //
    //     if (key.ClickInput == InputKey.ClickType.Scroll)
    //     {
    //         ToolSlotSelected = false;
    //         return false;
    //     }
    //     
    //     return false;
    // }
}