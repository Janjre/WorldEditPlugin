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
    public static bool OnInput(InputKey key, bool isDown) // split up into relevant parts in different files and functions
    {
        if (isDown && Onix.Gui.MouseGrabbed && Globals.NotInGui)
        {
            RaycastResult result = Onix.LocalPlayer.Raycast;
            if (Onix.LocalPlayer.MainHandItem.Item != null)
            {
                if (Onix.LocalPlayer.MainHandItem.Item.Name == "wooden_axe")
                {
                    if (key.Value == InputKey.Type.LMB)
                    {
                        Globals.pos1 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y, result.BlockPosition.Z);
                        // Onix.Client.Notify("HAHA BLOCKED INPUT1!!!");
                        return true;

                    }

                    if (key.Value == InputKey.Type.RMB)
                    {
                        Globals.pos2 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y, result.BlockPosition.Z);
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
            if (key.Value == InputKey.Type.Y && isDown && Globals.NotInGui)
            {
                Globals.NotInGui = false;
                Onix.Gui.MouseGrabbed = false;
            }

            if (key.Value == InputKey.Type.Escape && isDown && Globals.NotInGui == false)
            {

                Globals.NotInGui = true;
                Onix.Gui.MouseGrabbed = true;
                return true;
            }

            if (Globals.NotInGui == false && key.Value == InputKey.Type.LMB && isDown)
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
                    long targetUUID = Globals.UndoHistory[Globals.undoPoint].UUID;

                    foreach (MyBlock block in Globals.UndoHistoryAsBlocks)
                    {
                        if (block.Action == targetUUID)
                        {
                            Onix.Client.ExecuteCommand("execute setblock " + block.Position.X + " " + block.Position.Y +
                                                       " " + block.Position.Z + " " + block.Name);
                        }
                    }

                    Globals.undoPoint -= 1;
                    if (Globals.undoPoint < 0)
                    {
                        Globals.undoPoint = 0;
                    }

                    if (Globals.undoPoint > Globals.UndoHistory.Count)
                    {
                        Globals.undoPoint = Globals.UndoHistory.Count;
                    }
                }

                if (Globals.myContains(redo, mouseCursor))
                {

                    Globals.undoPoint += 1;

                    long targetUUID = Globals.UndoHistory[Globals.undoPoint].UUID;
                    foreach (MyBlock block in Globals.RedoHistoryAsBlocks)
                    {
                        if (block.Action == targetUUID)
                        {
                            Onix.Client.ExecuteCommand("execute setblock " + block.Position.X + " " + block.Position.Y +
                                                       " " + block.Position.Z + " " + block.Name);
                        }
                    }

                    if (Globals.undoPoint < 0)
                    {
                        Globals.undoPoint = 0;
                    }

                    if (Globals.undoPoint > Globals.UndoHistory.Count)
                    {
                        Globals.undoPoint = Globals.UndoHistory.Count;
                    }

                }

                if (Globals.myContains(clear, mouseCursor))
                {
                    Globals.undoPoint = 0;
                    for (int i = Globals.UndoHistory.Count - 1; i >= 1; i--)
                    {
                        Globals.UndoHistory.RemoveAt(i);
                    }
                }

                return true;


            }



            if (Onix.Gui.ScreenName == "hud_screen" && key.Value == InputKey.Type.Tab && isDown && // increment thgrough options
                Globals.NotInGui == false)
            {


                var (args, argCount) = Globals.SimpleSplit(Globals.CommandBox.Text); // argCount is not 0-based !!

                if (Autocomplete.currentOptions.Count == 0)
                {
                    return true;
                }

                int increment = 1;
                
                if (Globals.Shifting)
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

            if (Globals.NotInGui == false && isDown && key.Value == InputKey.Type.Space) // ACtually filling in the thing
            {
                Autocomplete.Complete();
            }
            

            if (Globals.NotInGui == false && isDown) 
            {
                switch (key.Value)
                {
                    case InputKey.Type.Up:
                        if (Globals.commandHistory.Count != 0){ 
                            Globals.commandHistoryPoint -= 1;

                            if (Globals.commandHistoryPoint > Globals.commandHistory.Count - 1)
                            {
                                Globals.commandHistoryPoint = Globals.commandHistory.Count - 1;
                            }
                            if (Globals.commandHistoryPoint < 0)
                            {
                                Globals.commandHistoryPoint = 0;
                            }
                            
                            Globals.CommandBox.Text = Globals.commandHistory[Globals.commandHistoryPoint];
                        }
                        break;
                    
                    case InputKey.Type.Down:

                        if (Globals.commandHistory.Count != 0)
                        {
                            Globals.commandHistoryPoint += 1;
                                                    
                            if (Globals.commandHistoryPoint > Globals.commandHistory.Count - 1)
                            {
                                Globals.commandHistoryPoint = Globals.commandHistory.Count - 1;
                            }
                            if (Globals.commandHistoryPoint < 0)
                            {
                                Globals.commandHistoryPoint = 0;
                            }
    
                            Globals.CommandBox.Text = Globals.commandHistory[Globals.commandHistoryPoint];
                            
                        }
                        break;
                }
                
            }
            
            if (Globals.NotInGui == false && key.Value == InputKey.Type.Shift)
            {
                Globals.Shifting = isDown;
            }

            if (Globals.NotInGui == false)
            {
                return true;
            }
        }


        return false;

    }
}
