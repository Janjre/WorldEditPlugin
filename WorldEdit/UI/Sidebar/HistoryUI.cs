using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
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

    public static bool Input(InputKey key, bool isDown)
    {

        if (key == InputKey.Type.LMB && isDown)
        {
           Vec2 mouseCursor = Onix.Gui.MousePosition;
        float screenWidth = Onix.Gui.ScreenSize.X;
        float screenHeight = Onix.Gui.ScreenSize.Y;
        
        Rect undo = new Rect(new Vec2(screenWidth * 0.80f, screenHeight * 0.12f),
                new Vec2((screenWidth * 0.80f) + 15, (screenHeight * 0.12f) + 15));
        
        Rect redo = new Rect(new Vec2(screenWidth * 0.82f, screenHeight * 0.12f),
            new Vec2((screenWidth * 0.82f) + 15, (screenHeight * 0.12f) + 15));

        Rect clear = new Rect(new Vec2(screenWidth * 0.785f, screenHeight * 0.12f),
            new Vec2((screenWidth * 0.785f) + 9, (screenHeight * 0.12f) + 9));




        if (Globals.myContains(undo, mouseCursor))
        {
            long targetUUID = History.UndoHistory[History.undoPoint].UUID;

            foreach (MyBlock block in History.UndoHistoryAsBlocks)
            {
                if (block.Action == targetUUID)
                {
                    Onix.Client.ExecuteCommand("execute setblock " + block.Position.X + " " + block.Position.Y +
                                               " " + block.Position.Z + " " + block.Name);
                }
            }

            History.undoPoint -= 1;
            if (History.undoPoint < 0)
            {
                History.undoPoint = 0;
            }

            if (History.undoPoint > History.UndoHistory.Count)
            {
                History.undoPoint = History.UndoHistory.Count;
            }
        }

        if (Globals.myContains(redo, mouseCursor))
        {

            History.undoPoint += 1;

            long targetUUID = History.UndoHistory[History.undoPoint].UUID;
            foreach (MyBlock block in History.RedoHistoryAsBlocks)
            {
                if (block.Action == targetUUID)
                {
                    Onix.Client.ExecuteCommand("execute setblock " + block.Position.X + " " + block.Position.Y +
                                               " " + block.Position.Z + " " + block.Name);
                }
            }

            if (History.undoPoint < 0)
            {
                History.undoPoint = 0;
            }

            if (History.undoPoint > History.UndoHistory.Count)
            {
                History.undoPoint = History.UndoHistory.Count;
            }

        }

        if (Globals.myContains(clear, mouseCursor))
        {
            History.undoPoint = 0;
            for (int i = History.UndoHistory.Count - 1; i >= 1; i--)
            {
                History.UndoHistory.RemoveAt(i);
            }
        } 
        }

        return false;

    }

    public static Tab tab = new (render, Input,1, "History", History.UndoIcon,1);

}