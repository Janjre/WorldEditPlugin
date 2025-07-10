using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;

namespace WorldEdit.UI;

public class Tab
{
    public Func<Rect,float,float,float, bool> Render;
    public int TabNumber;
    public string Name;
    public TexturePath Icon;
    public int Type; // 1 = sidebar, 0 = main 
    public Rect Button;
    public Func<InputKey, bool, bool> OnInput;

    public Tab (Func<Rect,float,float,float,bool> render, Func<InputKey, bool, bool> inputHandler, int tabNumber, string name, TexturePath icon, int type)
    {
        Render = render;
        TabNumber = tabNumber;
        Name = name;
        Icon = icon;
        Type = type;
        OnInput = inputHandler;
    }
}

