using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.World;
using WorldEdit.Maths;
using WorldEdit.Tool;

namespace WorldEdit.Tool.Tools;

public class CircleTool: BaseTool
{
    private enum CircleStage
    {
        NotStarted,
        SelectCenter,
        SelectFirstRadius,
        SelectLastRadius
    }

    private Vec3 _centre;
    private Vec3 _radiusOne;
    private Vec3 _radiusTwo; 
    private CircleStage _stage;

    

    public CircleTool() : base("circle", "wooden_shovel") {}
    public override bool OnPressed(InputKey key, bool isDown)
    {
        if (key != InputKey.Type.LMB || !isDown)
        {
            return false;
        }

        if (_stage == CircleStage.NotStarted)
        {
            RaycastResult result = Onix.LocalPlayer.Raycast;
            _centre= new Vec3(result.BlockPosition);
            _stage = CircleStage.SelectCenter;
        }else if (_stage == CircleStage.SelectCenter)
        {
            RaycastResult result = Onix.LocalPlayer.Raycast;
            _radiusOne= new Vec3(result.BlockPosition);
            _stage = CircleStage.SelectFirstRadius;
        }else if (_stage == CircleStage.SelectFirstRadius)
        {
            Vec3 origin = Onix.LocalPlayer.Position.WithY(Onix.LocalPlayer.Position.Y + 1.5f);
            Angles rotation = Onix.LocalPlayer.Rotation;

            (bool success, Vec3 output) = Maths.CircleCalculator.RaySphereIntersection(rotation, origin, _centre, _radiusOne);

            if (success)
            {
                _radiusTwo = output;
            }
            else
            {
                Console.WriteLine("Point must be on sphere");
            }

            (Vec3 u, Vec3 v, Vec3 centre) = CircleCalculator.GetPlane(_centre, _radiusOne, _radiusTwo);

            List <Vec2> circlePoints = new List<Vec2>();

            for (float theta = 0; theta < 360; theta++)
            {
                circlePoints.Add(new Vec2(
                    (float)(Math.Sin(theta * (Math.PI/180))),
                    (float)(Math.Cos(theta * (Math.PI/180)))
                ));
            }


            float radius = Math.Abs((_centre - _radiusOne).Length);
            foreach (Vec2 point in circlePoints)
            {
                Vec2 toProject = point * radius;

                Vec3 pointIn3dOnPlane = CircleCalculator.ToPlane(toProject, u, v, centre);
                pointIn3dOnPlane = pointIn3dOnPlane.Normalized;
                
                Onix.Game.ExecuteCommand(
                    $"setblock {pointIn3dOnPlane.X} {pointIn3dOnPlane.Y} {pointIn3dOnPlane.Z} diamond_block"
                );

            }

        }
        
        
        return true;
    }

    public override void OnRender(RendererWorld gfx, float delta)
    {
        
    }
}