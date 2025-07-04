using OnixRuntime.Api;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.Rendering.Helpers;

namespace WorldEdit.UI.Sidebar;



public static class NoiseUI
{
    public static ScrollbarPanelLogic ScrollBar = new ();

    public static List<string> testData = Autocomplete.blocks;

    public static float textHeight = 5f;
    public static float paddingHeight = 2f;
    
    public static bool render(Rect area, float screenHeight, float screenWidth, float dt)
    {
        
        ScrollBar.MarginFromLeft = 0f;
        ScrollBar.MarginFromRight = 0f;
        ScrollBar.PanelRect = area;
        
        var clickInput = InputHandler.InputTrackers.ClickInput;
        var scrollCount = InputHandler.InputTrackers.ScrollCount;
        var mousePos = Onix.Gui.MousePosition;

        if (clickInput != null && mousePos != null)
        {
            ScrollBar.Update(clickInput, mousePos, (int)scrollCount, dt);
        }
        
        float currentScrollY = ScrollBar.GetScrollOffset().Y;
        using (_ = Onix.Render.Direct2D.PushClippingRectangleWithin(ScrollBar.ContentRect))
        {
            for (int x = 0; x <= 100; x++)
            {
                int index = x;

                float height = index * (textHeight + paddingHeight);

                height -= currentScrollY;
                Rect cardRect = new Rect(new Vec2(area.TopLeft.X,height),
                    new Vec2(area.TopRight.X,height+textHeight));
                if (cardRect.Y > area.W || cardRect.W < area.Y) {
                    continue; // Skip rendering if the card is outside the visible area
                }
                Onix.Render.Direct2D.RenderText(cardRect,ColorF.White,"hi there", TextAlignment.Left,TextAlignment.Bottom);
                
            }
        }
        ScrollBar.RenderScrollbarsV3Themed(Onix.Render.Common);
        return true;
    }

    public static Tab tab = new (render, 2, "Noise", History.RedoIcon,1);

}