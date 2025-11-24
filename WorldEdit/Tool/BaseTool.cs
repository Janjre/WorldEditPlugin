using System.Net.Mime;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Items;
using OnixRuntime.Api.Rendering;

namespace WorldEdit.Tool;

public class BaseTool
{
    public BaseTool(string name, string item, bool removable)
    {
        Name = name;
        Item = item;
        Removable = removable;
    }
    
    public string Item;
    public bool Removable;
    public virtual bool OnPressed(InputKey key, bool isDown) { return true; }
    public virtual void OnRender (RendererWorld gfx, float delta){} 
    public string Name;
    public TexturePath Texture;
}