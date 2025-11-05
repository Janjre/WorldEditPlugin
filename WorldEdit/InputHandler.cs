using System.Globalization;
using OnixRuntime.Api.OnixClient;

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
        
        if (isDown && Onix.Gui.MouseGrabbed)
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
        
        return false;
    }
}
