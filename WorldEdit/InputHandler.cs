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
                        Vec3 toInclude = new Vec3(result.BlockPosition.X, result.BlockPosition.Y,
                            result.BlockPosition.Z);

                        if (!Selection.Blocks().Contains(toInclude))
                        {
                            
                            if (toInclude.X > Selection.LargestPoint.X)
                            {
                                Selection.LargestPoint = new Vec3(toInclude.X, Selection.LargestPoint.Y, Selection.LargestPoint.Z);
                            }
                            if (toInclude.X < Selection.SmallestPoint.X)
                            {
                                Selection.SmallestPoint = new Vec3(toInclude.X, Selection.SmallestPoint.Y, Selection.SmallestPoint.Z);
                            }


                            if (toInclude.Y > Selection.LargestPoint.Y)
                            {
                                Selection.LargestPoint = new Vec3(Selection.LargestPoint.X, toInclude.Y, Selection.LargestPoint.Z);
                            }
                            if (toInclude.Y < Selection.SmallestPoint.Y)
                            {
                                Selection.SmallestPoint = new Vec3(Selection.SmallestPoint.X, toInclude.Y, Selection.SmallestPoint.Z);
                            }


                            if (toInclude.Z > Selection.LargestPoint.Z)
                            {
                                Selection.LargestPoint = new Vec3(Selection.LargestPoint.X, Selection.LargestPoint.Y, toInclude.Z);
                            }
                            if (toInclude.Z < Selection.SmallestPoint.Z)
                            {
                                Selection.SmallestPoint = new Vec3(Selection.SmallestPoint.X, Selection.SmallestPoint.Y, toInclude.Z);
                            }

                        }

                        return true;
                    }
                }
            }


        } 
        
        return false;
    }
}
