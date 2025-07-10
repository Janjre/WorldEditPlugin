using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;

namespace WorldEdit.UI.Main;

public class NoiseMakerUI
{
    public static bool render(Rect renderArea, float screenWidth, float screenHeight, float delta)
    {
        return true;
    }

    public static bool Input(InputKey key, bool isDown)
    {
        return false;
    }
    
    public static Tab tab = new (render, Input, 5, "Test2", History.UndoIcon,0);
}