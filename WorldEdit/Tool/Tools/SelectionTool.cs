using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.World;
using WorldEdit.Tool;

namespace WorldEdit.Tool.Tools;

public class SelectionTool: BaseTool
{

    public SelectionTool() : base("selection", "wooden_axe",false) {}
    public override bool OnPressed(InputKey key, bool isDown)
    {
        if ((key == InputKey.Type.LMB || key == InputKey.Type.RMB || key == InputKey.Type.MMB) && !isDown)
        {
            return true;
        }
        RaycastResult result = Onix.LocalPlayer.Raycast;
         if (key.Value == InputKey.Type.LMB)
        {
            if (Selection.SelectionType == Selection.SelectionTypeEnum.Arbitrary)
            {
                Console.WriteLine("Selection type is arbitrary - changing to cuboid");
                Selection.SelectionType = Selection.SelectionTypeEnum.Cuboid;
            }
            Selection.pos1 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y,
                result.BlockPosition.Z);
            return true;

        }

        if (key.Value == InputKey.Type.RMB)
        {
            if (Selection.SelectionType == Selection.SelectionTypeEnum.Arbitrary)
            {
                Console.WriteLine("Selection type is arbitrary - changing to cuboid");
                Selection.SelectionType = Selection.SelectionTypeEnum.Cuboid;
            }
            Selection.pos2 = new Vec3(result.BlockPosition.X, result.BlockPosition.Y,
                result.BlockPosition.Z);
            return true;
        }

        if (key.Value == InputKey.Type.MMB)
        {
            if (Selection.SelectionType == Selection.SelectionTypeEnum.Arbitrary)
            {
                Console.WriteLine("Selection type is arbitrary - changing to cuboid");
                Selection.SelectionType = Selection.SelectionTypeEnum.Cuboid;
            }
            
            Vec3 toInclude = new Vec3(result.BlockPosition.X, result.BlockPosition.Y,
                result.BlockPosition.Z);

            if (!Selection.Blocks().Contains(toInclude))
            {
                
                if (toInclude.X > Selection.LargestPoint.X)
                {
                    Selection.LargestPoint = new Vec3(toInclude.X, Selection.LargestPoint.Y, Selection.LargestPoint.Z);
                }
                if (toInclude.X < Selection.SmallestPoint.X)
                {
                    Selection.SmallestPoint = new Vec3(toInclude.X, Selection.SmallestPoint.Y, Selection.SmallestPoint.Z);
                }


                if (toInclude.Y > Selection.LargestPoint.Y)
                {
                    Selection.LargestPoint = new Vec3(Selection.LargestPoint.X, toInclude.Y, Selection.LargestPoint.Z);
                }
                if (toInclude.Y < Selection.SmallestPoint.Y)
                {
                    Selection.SmallestPoint = new Vec3(Selection.SmallestPoint.X, toInclude.Y, Selection.SmallestPoint.Z);
                }


                if (toInclude.Z > Selection.LargestPoint.Z)
                {
                    Selection.LargestPoint = new Vec3(Selection.LargestPoint.X, Selection.LargestPoint.Y, toInclude.Z);
                }
                if (toInclude.Z < Selection.SmallestPoint.Z)
                {
                    Selection.SmallestPoint = new Vec3(Selection.SmallestPoint.X, Selection.SmallestPoint.Y, toInclude.Z);
                }

            }

            return true;
        }
        
        

        return false;
    }

    public override void OnRender(RendererWorld gfx, float delta)
    {
        
    }
}