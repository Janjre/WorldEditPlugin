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
                    TabManager.selectedTabSide.OnOpened();
                }
            }
            foreach (Tab tab in TabManager.MainTabs)
            {
                if (Globals.myContains(tab.Button, mouseCursor))
                {
                    TabManager.selectedTabMain = tab;
                    TabManager.selectedTabMain.OnOpened();
                    Console.WriteLine($"Ran {TabManager.selectedTabMain.Name}.OnOpened()");
                }
            }
        }

        TabManager.selectedTabMain.OnInput(key, isDown);
        TabManager.selectedTabSide.OnInput(key, isDown); 


        

        if (key == InputKey.Type.Escape)
        {
            Globals.Screen.CloseScreen();
        }
        

        return false;
    }
}
