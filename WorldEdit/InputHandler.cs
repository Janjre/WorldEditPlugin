using System.Globalization;
using WorldEdit.UI;
using WorldEdit.UI.Sidebar;

namespace WorldEdit;
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

public static class InputHandler
{
    public static bool Shifting;
    public static class InputTrackers
    {
        public static InputKey ClickInput = InputKey.None;
        public static float ScrollCount = 0f;
    }
    public static bool OnInput(InputKey key, bool isDown) // split up into relevant parts in different files and functions
    {
        if (isDown && Onix.Gui.MouseGrabbed && Gui.NotInGui)
        {
            RaycastResult result = Onix.LocalPlayer.Raycast;
            if (Onix.LocalPlayer.MainHandItem.Item != null)
            {
                if (Onix.LocalPlayer.MainHandItem.Item.Name == "wooden_axe")
                {
                    if (key.Value == InputKey.Type.LMB)
                    {
                        Selection.pos1 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y, result.BlockPosition.Z);
                        return true;

                    }

                    if (key.Value == InputKey.Type.RMB)
                    {
                        Selection.pos2 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y, result.BlockPosition.Z);
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
            if (key.Value == InputKey.Type.Y && isDown && Gui.NotInGui)
            {
                Gui.NotInGui = false;
                Onix.Gui.MouseGrabbed = false;
            }

            if (key.Value == InputKey.Type.Escape && isDown && Gui.NotInGui == false)
            {

                Gui.NotInGui = true;
                Onix.Gui.MouseGrabbed = true;
                return true;
            }

            if (Gui.NotInGui == false && key.Value == InputKey.Type.LMB && isDown)
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

                foreach (Tab tab in TabManager.tabs)
                {
                    if (Globals.myContains(tab.Button,mouseCursor))
                    {
                        TabManager.selectedTab = tab;
                    }
                }

                return true;


            }



            if (Onix.Gui.ScreenName == "hud_screen" && key.Value == InputKey.Type.Tab && isDown && // increment thgrough options
                Gui.NotInGui == false)
            {


                var (args, argCount) = Globals.SimpleSplit(Gui.CommandBox.Text); // argCount is not 0-based !!

                if (Autocomplete.currentOptions.Count == 0)
                {
                    return true;
                }

                int increment = 1;
                
                if (Shifting)
                {
                    increment = -1;
                }
                
                
                int pointInList = Autocomplete.currentOptions.IndexOf(Autocomplete.Selected) + increment;
                
                if (pointInList == 0)
                {
                }

                if (!Globals.IndexExists(Autocomplete.currentOptions, pointInList))
                {
                    pointInList = 0;
                }
                Autocomplete.Selected = Autocomplete.currentOptions[pointInList];

                Autocomplete.isPreviewing = true;

                

            }

            if (Gui.NotInGui == false && isDown && key.Value == InputKey.Type.Space) // ACtually filling in the thing
            {
                Autocomplete.Complete();
            }
            

            if (Gui.NotInGui == false && isDown) 
            {
                switch (key.Value)
                {
                    case InputKey.Type.Up:
                        if (History.commandHistory.Count != 0){ 
                            History.commandHistoryPoint -= 1;

                            if (History.commandHistoryPoint > History.commandHistory.Count - 1)
                            {
                                History.commandHistoryPoint = History.commandHistory.Count - 1;
                            }
                            if (History.commandHistoryPoint < 0)
                            {
                                History.commandHistoryPoint = 0;
                            }
                            
                            Gui.CommandBox.Text = History.commandHistory[History.commandHistoryPoint];
                        }
                        break;
                    
                    case InputKey.Type.Down:

                        if (History.commandHistory.Count != 0)
                        {
                            History.commandHistoryPoint += 1;
                                                    
                            if (History.commandHistoryPoint > History.commandHistory.Count - 1)
                            {
                                History.commandHistoryPoint = History.commandHistory.Count - 1;
                            }
                            if (History.commandHistoryPoint < 0)
                            {
                                History.commandHistoryPoint = 0;
                            }
    
                            Gui.CommandBox.Text = History.commandHistory[History.commandHistoryPoint];
                            
                        }
                        break;
                }
                
            }
            
            if (Gui.NotInGui == false && key.Value == InputKey.Type.Shift)
            {
                Shifting = isDown;
            }

            if (!Gui.NotInGui && key.IsMouse)
            {
                InputTrackers.ClickInput = key;

                if (key.Value == InputKey.Type.Scroll)
                {
                    if (isDown)
                    {
                        InputTrackers.ScrollCount += 1;
                    }
                    else
                    {
                        InputTrackers.ScrollCount -= 1;
                    }
                }

                
            }

            if (Gui.NotInGui == false)
            {
                return true;
            }
        }


        return false;

    }
}
