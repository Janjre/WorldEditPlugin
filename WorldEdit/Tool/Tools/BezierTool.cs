using System.Drawing;
using System.Threading.Channels;
using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.World;
using WorldEdit.Maths;
using WorldEdit.Tool;

namespace WorldEdit.Tool.Tools;

public class BezierTool: BaseTool
{

    public static List<Vec3> Points = new List<Vec3>();
    public static int SelectedPointIndex = -1;
    public static float SelectionPointDist = 0f;

    public BezierTool() : base("bezier", "wooden_hoe",true) {}
    public override bool OnPressed(InputKey key, bool isDown)
    {
        if (isDown == false)
        {
            return false;
        }
        RaycastResult result = Onix.LocalPlayer.Raycast;
        
        if (key.Value == InputKey.Type.LMB)
        {
            if (Selection.SelectionType == Selection.SelectionTypeEnum.Cuboid)
            {
                Console.WriteLine("Switched to arbitrary selection type");
                Selection.SelectionType = Selection.SelectionTypeEnum.Arbitrary;
            }
            
            Points.Add(new Vec3(result.HitPosition.X, result.HitPosition.Y,
                result.HitPosition.Z));

            Console.WriteLine($"Added point, {Points.Count} in total");
            return true;
        }

        if (key.Value == InputKey.Type.RMB)
        {
            
            if (Points.Count > 1)
            {
                SelectedPointIndex = -1;
                Selection.ArbitraryBitmap = new List<Vec3>();

                float length = 0;
                for (int i = 0; i < Points.Count - 1; i++)
                {
                    length += (Points[i + 1] - Points[i]).Length;
                }


                List<Vec3> curvePoints = new List<Vec3>();
                float steps = length * 10;
                for (int n = 0; n <= steps; n++)
                {
                    float t = n / steps;
                    Vec3 output = (BezierCalculator.BezierPoint(Points, t)).Floor();
                    
                    if (!Selection.ArbitraryBitmap.Contains(output))
                    {
                        Selection.ArbitraryBitmap.Add(output);
                    }                
                }

                Console.WriteLine("Finished curve - you can now do commands like .set on this area");
                Points = new List<Vec3>();
            }
            else
            {
                Console.WriteLine("Cleared arbitrary selection");
                Selection.ArbitraryBitmap = new List<Vec3>();
            }

            return true;
        }

        if (key == InputKey.Type.MMB)
        {
            if (SelectedPointIndex != -1)
            {
                SelectedPointIndex = -1;
                return true;
            }
            
            Angles rotation = Onix.LocalPlayer.Rotation;
            float x = (float)(Math.Cos(rotation.Pitch * Math.PI/180) * Math.Sin(rotation.Yaw * Math.PI/180));
            float y = (float)Math.Sin(rotation.Pitch * Math.PI/180);
            float z = (float)(Math.Cos(rotation.Pitch * Math.PI/180) * Math.Cos(rotation.Yaw * Math.PI/180));
            x /= 10;
            y /= 10;
            z /= 10;
            x *= -1;
            y *= -1;
            
            Vec3 headPos = Onix.LocalPlayer.Position.WithY(Onix.LocalPlayer.Position.Y+1.5f);
            for (int i = 0; i < 500;i++)
            {
                headPos.X += x;
                headPos.Y += y;
                headPos.Z += z;

                foreach (Vec3 point in Points)
                {
                    BoundingBox box = new BoundingBox(point- 0.5f, point + 0.5f);
                    
                    if (box.Contains(headPos))
                    {
                        SelectedPointIndex = Points.IndexOf(point);
                        SelectionPointDist = Points[SelectedPointIndex].Distance(Onix.LocalPlayer.Position);
                    }
                }
            }
            
            return true;
        }
        return false;
    }
    

    public override void OnRender(RendererWorld gfx, float delta)
    {
        
        
        foreach (Vec3 point in Points)
        {
            BoundingBox box = new BoundingBox(point - 0.2f, point + 0.2f);
            if (SelectedPointIndex != -1)
            {
                if (Points.IndexOf(point) == SelectedPointIndex)
                {
                    Onix.Render.World.RenderBoundingBoxFill(box,ColorF.Red);
                }
                else
                {
                    Onix.Render.World.RenderBoundingBoxFill(box,ColorF.White);
                }
            }
            else
            {
                Onix.Render.World.RenderBoundingBoxFill(box,ColorF.White);
            }
            
            
            
        }

        for (int i = 0; i < Points.Count - 1; i++)
        {
            Onix.Render.World.DrawLine(Points[i],Points[i+1], ColorF.Red);
        }



        if (Points.Count > 2)
        {
            float length = (Points[0] - Points[Points.Count - 1]).Length;

            List<Vec3> curvePoints = new List<Vec3>();
            float steps = length * 10;
            for (int n = 0; n <= steps; n++)
            {
                float t = n / steps;
                Vec3 output = BezierCalculator.BezierPoint(Points, t);
                curvePoints.Add(output);
            }
            
            for (int i = 0; i < curvePoints.Count - 1; i++)
            {
                Onix.Render.World.DrawLine(curvePoints[i],curvePoints[i+1], ColorF.Green);
            }
        }

        if (SelectedPointIndex != -1)
        {
            Points[SelectedPointIndex] = Onix.LocalPlayer.ForwardPosition(SelectionPointDist);
        }
        
    }
}