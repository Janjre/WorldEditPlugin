using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.OnixClient;
using OnixRuntime.Api.Rendering;
using WorldEdit.UI.Main;

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
        ConsoleUI.CommandBox.IsFocused = true;
        Console.WriteLine("Opened it");
    }

    public override bool OnClosed()
    {
        ConsoleUI.CommandBox.IsFocused = false;
        return false;
    }
}