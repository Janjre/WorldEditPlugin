using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.World;
using WorldEdit.Tool;

namespace WorldEdit.Tool.Tools;

public class CircleTool: BaseTool
{
    private enum Stage
    {
        SelectCenter,
        SelectFirstRadius,
        SelectLastRadius
    }

    public Vec3 Center;
    public Vec3 RadiusOne;
    public Vec3 RadiusTwo;

    public CircleTool() : base("circle", "wooden_shovel") {}
    public override bool OnPressed(InputKey key, bool isDown)
    {
        if (key != InputKey.Type.LMB || !isDown)
        {
            return false;
        }
        
        
        return true;
    }

    public override void OnRender(RendererWorld gfx, float delta)
    {
        
    }
}