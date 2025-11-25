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
    private float snapThreshold = 1 - (1f/36f); 

    

    public CircleTool() : base("circle", "wooden_shovel",true) {}
    public override bool OnPressed(InputKey key, bool isDown)
    {
        
        if (!isDown) { return false; }
        
        if (key != InputKey.Type.LMB)
        {
            if (key == InputKey.Type.RMB)
            {
                Selection.ArbitraryBitmap = new List<Vec3>();
                Console.WriteLine("Cleared arbitrary selection");
            }

            return false;
        }
        

        if (_stage == CircleStage.NotStarted)
        {
            RaycastResult result = Onix.LocalPlayer.Raycast;
            _centre= result.HitPosition;
            _centre = _centre.Round();
            _stage = CircleStage.SelectCenter;
        }else if (_stage == CircleStage.SelectCenter)
        {
            RaycastResult result = Onix.LocalPlayer.Raycast;
            _radiusOne= result.HitPosition;
            _radiusOne = _radiusOne.Round();
            _stage = CircleStage.SelectFirstRadius;
        }else if (_stage == CircleStage.SelectFirstRadius)
        {
            Vec3 origin = Onix.LocalPlayer.Position.WithY(Onix.LocalPlayer.Position.Y + 1.62f);
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
            
            Vec3 normal = u.Cross(v).Normalized;

            float dotXY = Math.Abs(normal.Dot(new Vec3(0,0,1)));
            float dotXZ = Math.Abs(normal.Dot(new Vec3(0,1,0)));
            float dotYZ = Math.Abs(normal.Dot(new Vec3(1,0,0)));
            
            
            
            if (dotXY > snapThreshold)
            {
                // Snap to XY plane
                u = new Vec3(1,0,0);
                v = new Vec3(0,1,0);
            }
            else if (dotXZ > snapThreshold)
            {
                // Snap to XZ plane
                u = new Vec3(1,0,0);
                v = new Vec3(0,0,1);
            }
            else if (dotYZ > snapThreshold)
            {
                // Snap to YZ plane
                u = new Vec3(0,1,0);
                v = new Vec3(0,0,1);
            }


            
            List <Vec2> circlePoints = new List<Vec2>();

            float radiusHere = (_radiusOne - _centre).Length;

            for (float theta = 0; theta < 360; theta+=20/radiusHere)
            {
                circlePoints.Add(new Vec2(
                    (float)(Math.Sin(theta * (Math.PI/180))),
                    (float)(Math.Cos(theta * (Math.PI/180)))
                ));
            }


            float radius = Math.Abs((_centre - _radiusOne).Length);

            if (Selection.SelectionType == Selection.SelectionTypeEnum.Cuboid)
            {
                Console.WriteLine("Selection type set to arbitrary");
                Selection.SelectionType = Selection.SelectionTypeEnum.Arbitrary;
            }

            Selection.ArbitraryBitmap = new List<Vec3>();
            
            foreach (Vec2 point in circlePoints)
            {
                Vec2 toProject = point * radius;

                Vec3 pointIn3dOnPlane = CircleCalculator.ToPlane(toProject, u, v, centre);
                pointIn3dOnPlane = pointIn3dOnPlane.Round();

                if (!Selection.ArbitraryBitmap.Contains(pointIn3dOnPlane))
                {
                    Selection.ArbitraryBitmap.Add(pointIn3dOnPlane);
                }
                

            }

            _stage = CircleStage.NotStarted;

        }
        
        
        return true;
    }

    public override void OnRender(RendererWorld gfx, float delta)
    {
        if (_stage != CircleStage.NotStarted)
        {
            BoundingBox centreBox = new BoundingBox(_centre - 0.2f, _centre + 0.2f);
            
            gfx.RenderBoundingBoxFill(centreBox,ColorF.White);
        }

        if (_stage == CircleStage.SelectFirstRadius)
        {
            Vec3 c = _centre;
            Vec3 v1 = _radiusOne - c;
            float radius = v1.Length;
            Vec3 axis = v1.Normalized;

            (Vec3 u, Vec3 w) = CircleCalculator.getUvFromNormal(axis);

            for (int n = 0; n < 360; n += 10)
            {
                float t = MathF.PI * n / 180f;

                Vec3 normal = u * MathF.Cos(t) + w * MathF.Sin(t);
                normal = normal.Normalized;

                
                Vec3 uT = normal.Cross(axis).Normalized; 
                Vec3 vT = axis;                             

                
                List<Vec3> points = new List<Vec3>();
                for (int m = 0; m <= 360; m += 3)
                {
                    float φ = MathF.PI * m / 180f;

                    Vec3 p =
                        c
                        + uT * (radius * MathF.Cos(φ))
                        + vT * (radius * MathF.Sin(φ));

                    points.Add(p);
                }

                for (int k = 0; k < points.Count - 1; k++)
                    gfx.DrawLine(points[k], points[k + 1], ColorF.Red);
            }
            
            
            
            Vec3 origin = Onix.LocalPlayer.Position.WithY(Onix.LocalPlayer.Position.Y + 1.62f);
            Angles rotation = Onix.LocalPlayer.Rotation;

            
            // Draw looking circle
            
            (bool success, Vec3 output) = Maths.CircleCalculator.RaySphereIntersection(rotation, origin, _centre, _radiusOne);

            
            
            if (success)
            {
                BoundingBox lookingBox = new BoundingBox(output - 0.2f, output + 0.2f);
                gfx.RenderBoundingBoxFill(lookingBox,ColorF.Green);
                (Vec3 u2, Vec3 v2, Vec3 centre2) = CircleCalculator.GetPlane(_centre, _radiusOne, output);
                
                Vec3 normal2 = u2.Cross(v2).Normalized;

                float dotXY2 = Math.Abs(normal2.Dot(new Vec3(0,0,1)));
                float dotXZ2 = Math.Abs(normal2.Dot(new Vec3(0,1,0)));
                float dotYZ2 = Math.Abs(normal2.Dot(new Vec3(1,0,0)));


                if (dotXY2 > snapThreshold)
                {
                    u2 = new Vec3(1,0,0);
                    v2 = new Vec3(0,1,0);
                }
                else if (dotXZ2 > snapThreshold)
                {
                    u2 = new Vec3(1,0,0);
                    v2 = new Vec3(0,0,1);
                }
                else if (dotYZ2 > snapThreshold)
                {
                    u2 = new Vec3(0,1,0);
                    v2 = new Vec3(0,0,1);
                }

                // Circle points generation (same)
                List<Vec2> circlePoints = new List<Vec2>();
                for (float theta = 0; theta < 360; theta++)
                    circlePoints.Add(new Vec2(
                        (float)Math.Sin(theta * Math.PI / 180),
                        (float)Math.Cos(theta * Math.PI / 180)
                    ));

                // Project to 3D
                List<Vec3> toDrawPreviewThing = new List<Vec3>();
                float radius2 = (_centre - _radiusOne).Length;
                foreach (Vec2 point in circlePoints)
                {
                    Vec3 pointIn3dOnPlane = CircleCalculator.ToPlane(point * radius2, u2, v2, centre2);
                    toDrawPreviewThing.Add(pointIn3dOnPlane);
                }

                // Draw circle
                for (int k = 0; k < toDrawPreviewThing.Count; k++)
                    gfx.DrawLine(toDrawPreviewThing[k], toDrawPreviewThing[(k + 1) % toDrawPreviewThing.Count], ColorF.Green);

            }

        }
    }
}