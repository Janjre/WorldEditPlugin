using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.Rendering.Helpers;

namespace WorldEdit.UI.Sidebar;



public static class TestUI
{
   
    
    public static bool render(Rect area, float screenHeight, float screenWidth, float dt)
    {
        return true;
    }

    public static bool Input(InputKey key, bool isDown)
    {
        return false;
    }
    
    public static Tab tab = new (render, Input, 3, "Test", History.RedoIcon,1);

}