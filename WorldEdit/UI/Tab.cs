using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;

namespace WorldEdit.UI;

public class Tab
{
    public Func<Rect,float,float,bool> Render;
    public int TabNumber;
    public string Name;
    public TexturePath Icon;
    public int Type; // 1 = sidebar, 0 = main 

    public Tab (Func<Rect,float,float,bool> render, int tabNumber, string name, TexturePath icon, int type)
    {
        Render = render;
        TabNumber = tabNumber;
        Name = name;
        Icon = icon;
        Type = type;
    }
}

