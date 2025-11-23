using System.Globalization;
using OnixRuntime.Api.OnixClient;
using OnixRuntime.Api.OnixClient.Settings;
using WorldEdit.Tool;
using WorldEdit.Tool.Tools;

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
    // public OnixSettingKeybind HotBarSlot1 = new OnixSettingKeybind()
    
    public static bool OnInput(InputKey key, bool isDown) // split up into relevant parts in different files and functions
    {
        if (Onix.Gui.MouseGrabbed)
        {
            return ToolManager.OnKeyPressed(key, isDown); 
        }
        
        if (isDown && Onix.Gui.MouseGrabbed)
        {
            
            
            
            // if (Onix.LocalPlayer.MainHandItem.Item != null)
            // {
            //     foreach (Tool.BaseTool tool in Tool.ToolManager.RegisteredTools ?? Enumerable.Empty<Tool.BaseTool>())
            //     {
            //         if (tool == null)
            //         {
            //             Console.WriteLine("Null tool found in RegisteredTools");
            //             continue;
            //         }
            //
            //         var heldItemName = Onix.LocalPlayer?.MainHandItem?.Item?.Name;
            //         if (heldItemName == null)
            //         {
            //             // Probably between worlds or no item in hand
            //             continue;
            //         }
            //
            //         // Console.WriteLine("Got here");
            //         if (tool.Item == heldItemName)
            //         {
            //             
            //             try
            //             {
            //                 return tool.OnPressed(key, isDown);
            //             }
            //             catch (Exception ex)
            //             {
            //                 Console.WriteLine($"Error rendering tool {tool.Item}: {ex}");
            //             }
            //         }
            //     }
            // }

        } 
        
        return false;
    }
}
