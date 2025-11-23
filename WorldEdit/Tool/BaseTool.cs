using System.Net.Mime;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Items;
using OnixRuntime.Api.Rendering;

namespace WorldEdit.Tool;

public class BaseTool
{
    public BaseTool(string name, string item)
    {
        Name = name;
        Item = item;
    }
    
    public string Item;
    public virtual bool OnPressed(InputKey key, bool isDown) { return true; }
    public virtual void OnRender (RendererWorld gfx, float delta){} 
    public string Name;
    public TexturePath Texture;
}