using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.OnixClient;
using OnixRuntime.Api.Rendering;

namespace WorldEdit.UI;

public class WorldEditScreen : OnixClientScreen
{
    public WorldEditScreen() : base("WorldEdit", true, true)
    {
        
    }

    public override void OnRender(RendererCommon2D gfx)
    {
        Gui.DrawScreen(gfx);
    }

    public override bool OnInput(InputKey key, bool isDown)
    {
        return InputHandler.screenInput(key,isDown);
    }

    public override void OnOpened()
    {
        base.OnOpened();
        // Code to run when the screen is opened
    }

    public override bool OnClosed()
    {
        // Code to run when the screen is closed
        return base.OnClosed();
    }
}