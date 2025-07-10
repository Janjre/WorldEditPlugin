using System.Globalization;
using OnixRuntime.Api.OnixClient;
using WorldEdit.UI;
using WorldEdit.UI.Main;
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

    public static bool
        OnInput(InputKey key, bool isDown) // split up into relevant parts in different files and functions
    {
        
        Console.WriteLine($"Hit key {key}");
        if (isDown && Onix.Gui.MouseGrabbed && Gui.NotInGui)
        {
            RaycastResult result = Onix.LocalPlayer.Raycast;
            if (Onix.LocalPlayer.MainHandItem.Item != null)
            {
                if (Onix.LocalPlayer.MainHandItem.Item.Name == "wooden_axe")
                {
                    if (key.Value == InputKey.Type.LMB)
                    {
                        Selection.pos1 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y,
                            result.BlockPosition.Z);
                        return true;

                    }

                    if (key.Value == InputKey.Type.RMB)
                    {
                        Selection.pos2 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y,
                            result.BlockPosition.Z);
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
            if (key.Value == InputKey.Type.Y && isDown)
            {
                Globals.Screen.OpenScreen();
            }
        }
        
        if (Onix.Gui.ScreenName == Globals.Screen.ScreenName)
        {
            Console.WriteLine("Getting here");
            if (key.Value == InputKey.Type.Escape && isDown)
            {
                Globals.Screen.CloseScreen();
            }
        }

        return false;
    }

    public static bool screenInput(InputKey key,bool isDown)
    {
        if (key.Value == InputKey.Type.LMB && isDown)
        {
            Vec2 mouseCursor = Onix.Gui.MousePosition;
            float screenWidth = Onix.Gui.ScreenSize.X;
            float screenHeight = Onix.Gui.ScreenSize.Y;

            

            foreach (Tab tab in TabManager.SideTabs)
            {
                if (Globals.myContains(tab.Button, mouseCursor))
                {
                    TabManager.selectedTabSide = tab;
                }
            }
            foreach (Tab tab in TabManager.MainTabs)
            {
                if (Globals.myContains(tab.Button, mouseCursor))
                {
                    TabManager.selectedTabMain = tab;
                }
            }
        }



        if (key.Value == InputKey.Type.Tab && isDown)  // increment thgrough options
        {


            var (args, argCount) = Globals.SimpleSplit(ConsoleUI.CommandBox.Text); // argCount is not 0-based !!

            if (Autocomplete.currentOptions.Count == 0)
            {
                return false;
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

        if (isDown && key.Value == InputKey.Type.Space) // ACtually filling in the thing
        {
            Autocomplete.Complete();
        }


        if (isDown)
        {
            switch (key.Value)
            {
                case InputKey.Type.Up:
                    if (History.commandHistory.Count != 0)
                    {
                        History.commandHistoryPoint -= 1;

                        if (History.commandHistoryPoint > History.commandHistory.Count - 1)
                        {
                            History.commandHistoryPoint = History.commandHistory.Count - 1;
                        }

                        if (History.commandHistoryPoint < 0)
                        {
                            History.commandHistoryPoint = 0;
                        }

                        ConsoleUI.CommandBox.Text = History.commandHistory[History.commandHistoryPoint];
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

                        ConsoleUI.CommandBox.Text = History.commandHistory[History.commandHistoryPoint];

                    }

                    break;
            }

        }

        if (key == InputKey.Type.Escape)
        {
            Globals.Screen.CloseScreen();
        }
        

        return false;
    }
}
