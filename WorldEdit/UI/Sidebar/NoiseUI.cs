using OnixRuntime.Api;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;

namespace WorldEdit.UI.Sidebar;



public static class NoiseUI
{
    public static bool render(Rect area, float screenHeight, float screenWidth)
    {
        return true;
    }

    public static Tab tab = new (render, 2, "Noise", History.RedoIcon,1);

}