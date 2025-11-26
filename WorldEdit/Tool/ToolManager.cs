using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Items;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.Rendering;
using WorldEdit.Tool.Tools;

namespace WorldEdit.Tool;

public static class ToolManager
{
    public static List<BaseTool> RegisteredTools = new List<BaseTool>();
    public static BaseTool SelectedTool = new SelectionTool();
    public static bool SelectingTool = false;
    public static BaseTool WhenSelectingToolSelectedTool = SelectedTool;
    
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
    
    public static void RenderTenthSlot(RendererGame gfx, float delta)
    {
        
        TexturePath unselectedTexture = TexturePath.Game("textures/ui/hotbar_0.png");
        TexturePath selectedTexture = TexturePath.Game("textures/ui/selected_hotbar_slot.png");
        TexturePath startCap = TexturePath.Game("textures/ui/hotbar_start_cap.png");
        TexturePath endCap = TexturePath.Game("textures/ui/hotbar_end_cap.png");
        
        if (SelectingTool)
        {
            int n = 0;
            
            foreach (BaseTool tool in RegisteredTools)
            {
                
                SlotRects thisSlot = SlotDrawer.GetSlotRects(n);
                gfx.RenderTexture(thisSlot.FullSlot,unselectedTexture);
                gfx.RenderTexture(thisSlot.LeftSixteenth,startCap);
                gfx.RenderTexture(thisSlot.RightSixteenth,endCap);
                
                gfx.RenderItem(thisSlot.ItemPos,new ItemStack(tool.Item,1),true,1.2f);
                
                gfx.RenderText(thisSlot.TextSlot,ColorF.White, tool.Name,TextAlignment.Left,TextAlignment.Center);

                if (tool == WhenSelectingToolSelectedTool)
                {
                    gfx.RenderTexture(thisSlot.ExpandedSlot,selectedTexture);
                    
                }
                
                n++;
            }
            
        }
        else
        {
            SlotRects slotRects = SlotDrawer.GetSlotRects(0);
        
            
        
            gfx.RenderTexture(slotRects.FullSlot,unselectedTexture);
            gfx.RenderTexture(slotRects.LeftSixteenth,startCap);
            gfx.RenderTexture(slotRects.RightSixteenth,endCap);
        
        
        
            gfx.RenderItem(slotRects.ItemPos,new ItemStack(SelectedTool.Item,1),true,1.2f);

            if (ToolSlotSelected)
            {
                gfx.RenderTexture(slotRects.ExpandedSlot, selectedTexture);
            }
            
        }
    }
    
    private static bool IsKeyNumber(InputKey key)
    {
        if (key == WorldEdit.Config.HotbarKey1.Value) { return true; }
        if (key == WorldEdit.Config.HotbarKey2.Value) { return true; }
        if (key == WorldEdit.Config.HotbarKey3.Value) { return true; }
        if (key == WorldEdit.Config.HotbarKey4.Value) { return true; }
        if (key == WorldEdit.Config.HotbarKey5.Value) { return true; }
        if (key == WorldEdit.Config.HotbarKey6.Value) { return true; }
        if (key == WorldEdit.Config.HotbarKey7.Value) { return true; }
        if (key == WorldEdit.Config.HotbarKey8.Value) { return true; }
        if (key == WorldEdit.Config.HotbarKey9.Value) { return true; }
    
        return false;
    }
    
    public static bool OnKeyPressed(InputKey key, bool isDown)
    {
        if (key == WorldEdit.Config.HotbarKey10.Value && isDown)
        {
            ToolSlotSelected = true;
            return true;
        }
    
        if (IsKeyNumber(key))
        {
            ToolSlotSelected = false;
            return false;
        }
    
        if (key.ClickInput == InputKey.ClickType.Scroll)
        {
            if (!SelectingTool)
            {
                ToolSlotSelected = false;
                return false; 
            }
            else
            {
                int index = RegisteredTools.IndexOf(WhenSelectingToolSelectedTool);
                if (!isDown)
                {
                    index += 1;
                    if (index > RegisteredTools.Count - 1)
                    {
                        index = 0;
                    }
                }
                else
                {
                    index -= 1;
                    if (index < 0)
                    {
                        index = RegisteredTools.Count-1;
                    }
                }

                WhenSelectingToolSelectedTool = RegisteredTools[index];
            }
        }

        if (key == InputKey.Type.Alt)
        {
            if (ToolSlotSelected)
            {
                if (isDown)
                {
                    SelectingTool = true;
                }
                else
                {
                    SelectingTool = false;
                    SelectedTool = WhenSelectingToolSelectedTool;
                }
            }else if (isDown == false) 
            {
                SelectingTool = false;
            }

        }

        if (ToolSlotSelected)
        {
            if (key.IsMouse)
            {
                return SelectedTool.OnPressed(key,isDown);
            }
        }
        
        return false;
    }

    public static void OnWorldRender (RendererWorld gfx, float delta)
    {
        if (WorldEdit.Config.Do10ThSlot)
        {
            if (ToolSlotSelected)
            {
                SelectedTool.OnRender(gfx,delta);
            }
        }
        else
        {
            foreach (Tool.BaseTool tool in Tool.ToolManager.RegisteredTools ?? Enumerable.Empty<Tool.BaseTool>())
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
        
    }
    
    
}