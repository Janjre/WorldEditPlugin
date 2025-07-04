using OnixRuntime.Api;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.Rendering.Helpers;

namespace WorldEdit.UI.Sidebar;



public static class HistoryUI
{
    
    
    public static bool render(Rect area, float screenHeight, float screenWidth,float deltaTime)
    {
      

        float startIterationsPosition = screenHeight * 0.17f;
        float characterHeight = 7;
        float endPoint = screenHeight * 0.83f;

        
        
        for (int i = 0; i <= History.UndoHistory.Count; i++)
        {
            Vec2 tlTextPos = new Vec2(screenWidth * 0.67f, startIterationsPosition + characterHeight * i);
            Vec2 brTextPos = new Vec2(screenWidth * 0.83f,
                startIterationsPosition + (characterHeight * i) + characterHeight);

            if (brTextPos.Y >= endPoint)
            {
                break;
            }

            if (i >= 0 && i < History.UndoHistory.Count)
            {
                ColorF colour = ColorF.White;
                if (i == History.undoPoint)
                {
                    colour = ColorF.Red;
                }

                Onix.Render.Direct2D.RenderText(tlTextPos, colour, History.UndoHistory[i].Text, TextAlignment.Left,
                    TextAlignment.Top, characterHeight / 6);
            }


            Rect undo = new Rect(new Vec2(screenWidth * 0.80f, screenHeight * 0.16f),
                new Vec2((screenWidth * 0.80f) + 10, (screenHeight * 0.16f) + 10));

            Onix.Render.Direct2D.RenderTexture(undo, History.UndoIcon, 1f);

            Rect redo = new Rect(new Vec2(screenWidth * 0.815f, screenHeight * 0.16f),
                new Vec2((screenWidth * 0.815f) + 10, (screenHeight * 0.16f) + 10));

            Onix.Render.Direct2D.RenderTexture(redo, History.RedoIcon, 1f);

            Rect clear = new Rect(new Vec2(screenWidth * 0.785f, screenHeight * 0.16f),
                new Vec2((screenWidth * 0.785f) + 9, (screenHeight * 0.16f) + 9));

            Onix.Render.Direct2D.RenderTexture(clear, History.ClearIcon, 1f);
        }

        return true;
    }

    public static Tab tab = new (render, 1, "History", History.UndoIcon,1);

}