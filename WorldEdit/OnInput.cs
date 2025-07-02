using System.Globalization;

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
                        // Onix.Client.Notify("HAHA BLOCKED INPUT1!!!");
                        return true;

                    }

                    if (key.Value == InputKey.Type.RMB)
                    {
                        Selection.pos2 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y, result.BlockPosition.Z);
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
                    long targetUUID = HistoryActions.UndoHistory[HistoryActions.undoPoint].UUID;

                    foreach (MyBlock block in HistoryActions.UndoHistoryAsBlocks)
                    {
                        if (block.Action == targetUUID)
                        {
                            Onix.Client.ExecuteCommand("execute setblock " + block.Position.X + " " + block.Position.Y +
                                                       " " + block.Position.Z + " " + block.Name);
                        }
                    }

                    HistoryActions.undoPoint -= 1;
                    if (HistoryActions.undoPoint < 0)
                    {
                        HistoryActions.undoPoint = 0;
                    }

                    if (HistoryActions.undoPoint > HistoryActions.UndoHistory.Count)
                    {
                        HistoryActions.undoPoint = HistoryActions.UndoHistory.Count;
                    }
                }

                if (Globals.myContains(redo, mouseCursor))
                {

                    HistoryActions.undoPoint += 1;

                    long targetUUID = HistoryActions.UndoHistory[HistoryActions.undoPoint].UUID;
                    foreach (MyBlock block in HistoryActions.RedoHistoryAsBlocks)
                    {
                        if (block.Action == targetUUID)
                        {
                            Onix.Client.ExecuteCommand("execute setblock " + block.Position.X + " " + block.Position.Y +
                                                       " " + block.Position.Z + " " + block.Name);
                        }
                    }

                    if (HistoryActions.undoPoint < 0)
                    {
                        HistoryActions.undoPoint = 0;
                    }

                    if (HistoryActions.undoPoint > HistoryActions.UndoHistory.Count)
                    {
                        HistoryActions.undoPoint = HistoryActions.UndoHistory.Count;
                    }

                }

                if (Globals.myContains(clear, mouseCursor))
                {
                    HistoryActions.undoPoint = 0;
                    for (int i = HistoryActions.UndoHistory.Count - 1; i >= 1; i--)
                    {
                        HistoryActions.UndoHistory.RemoveAt(i);
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
                        if (HistoryActions.commandHistory.Count != 0){ 
                            HistoryActions.commandHistoryPoint -= 1;

                            if (HistoryActions.commandHistoryPoint > HistoryActions.commandHistory.Count - 1)
                            {
                                HistoryActions.commandHistoryPoint = HistoryActions.commandHistory.Count - 1;
                            }
                            if (HistoryActions.commandHistoryPoint < 0)
                            {
                                HistoryActions.commandHistoryPoint = 0;
                            }
                            
                            Gui.CommandBox.Text = HistoryActions.commandHistory[HistoryActions.commandHistoryPoint];
                        }
                        break;
                    
                    case InputKey.Type.Down:

                        if (HistoryActions.commandHistory.Count != 0)
                        {
                            HistoryActions.commandHistoryPoint += 1;
                                                    
                            if (HistoryActions.commandHistoryPoint > HistoryActions.commandHistory.Count - 1)
                            {
                                HistoryActions.commandHistoryPoint = HistoryActions.commandHistory.Count - 1;
                            }
                            if (HistoryActions.commandHistoryPoint < 0)
                            {
                                HistoryActions.commandHistoryPoint = 0;
                            }
    
                            Gui.CommandBox.Text = HistoryActions.commandHistory[HistoryActions.commandHistoryPoint];
                            
                        }
                        break;
                }
                
            }
            
            if (Gui.NotInGui == false && key.Value == InputKey.Type.Shift)
            {
                Shifting = isDown;
            }

            if (Gui.NotInGui == false)
            {
                return true;
            }
        }


        return false;

    }
}
