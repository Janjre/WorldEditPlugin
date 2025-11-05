using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;

namespace WorldEdit.UI;

public class MyOnixTextbox
{
    public Rect Area;
    public bool Focused;
    public string Text;
    public Func<string> OnEnter;
    
    private Dictionary<(InputKey, bool), string> myDict;

    

    
    MyOnixTextbox(Rect area, Func<string> onEnter)
    {
        Area = area;
        Focused = false;
        Text = "";
        OnEnter = onEnter;
    }

    bool Input(InputKey key, bool isDown)
    {
        
        
        
        return false;
    }
    
}