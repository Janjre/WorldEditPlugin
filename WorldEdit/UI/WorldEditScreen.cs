using OnixRuntime.Api;
using OnixRuntime.Api.OnixClient;
using OnixRuntime.Api.Rendering;

namespace WorldEdit.UI;

public class MyCustomScreen : OnixClientScreen
{
    public MyCustomScreen() : base("WorldEdit", true, true)
    {
        // Initialization code here
    }

    // Override the methods you need, such as rendering or input handling
    public override void OnRender(RendererCommon2D gfx)
    {
        // Draw your custom UI here
        gfx.RenderText(new OnixRuntime.Api.Maths.Vec2(100, 100), Onix.Client.ThemeV3.Text, "Hello from MyCustomScreen!", 1.0f);
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