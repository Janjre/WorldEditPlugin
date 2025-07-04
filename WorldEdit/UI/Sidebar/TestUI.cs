using OnixRuntime.Api;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.Rendering.Helpers;

namespace WorldEdit.UI.Sidebar;



public static class TestUI
{
   
    
    public static bool render(Rect area, float screenHeight, float screenWidth, float dt)
    {
        Console.WriteLine("RENDERING TEST");
        return true;
    }

    public static Tab tab = new (render, 3, "Test", History.RedoIcon,1);

}