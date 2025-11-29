using OnixRuntime.Api.Maths;

namespace WorldEdit.Maths;

public static class CircleCalculator
{
    public static (Vec3,Vec3,Vec3) GetPlane(Vec3 centre, Vec3 radiusOne, Vec3 radiusTwo) 
    {
        Vec3 C = centre;
        Vec3 R1 = radiusOne;
        Vec3 R2 = radiusTwo; 

        // direction vectors
        Vec3 v1 = R1 - C;
        Vec3 v2 = R2 - C;

        Vec3 normal = v1.Cross(v2).Normalized;

        float circleRadius = v1.Length;

        // 2d x and y on plane
        Vec3 u = v1.Normalized;                  
        Vec3 v = normal.Cross(u).Normalized;  // perpendicular to u in the plane
        

        return (u, v,C);

    }

    public static Vec3 ToPlane(Vec2 point, Vec3 u, Vec3 v,Vec3 c)
    {
        return c + (point.X * u) + (point.Y * v);
    }

    public static (bool, Vec3) RaySphereIntersection(Angles rotation, Vec3 origin, Vec3 centre, Vec3 point1)
    {

        float radius = (point1 - centre).Length;
        
        float dx = (float)(Math.Cos(rotation.Pitch * Math.PI / 180) * Math.Sin(rotation.Yaw * Math.PI / 180));
        float dy = (float)Math.Sin(rotation.Pitch * Math.PI / 180);
        float dz = (float)(Math.Cos(rotation.Pitch * Math.PI / 180) * Math.Cos(rotation.Yaw * Math.PI / 180));
        Vec3 dir = new Vec3(-dx, -dy, dz).Normalized;

        Vec3 L = origin - centre;

        float a = dir.Dot(dir);
        float b = 2f * dir.Dot(L);
        float c = L.Dot(L) - radius * radius;

        float discriminant = b * b - 4 * a * c;

        if (discriminant >= 0)
        {
            float sqrtD = MathF.Sqrt(discriminant);
            float t1 = (-b - sqrtD) / (2 * a);
            float t2 = (-b + sqrtD) / (2 * a);

            float t = (t1 > 0) ? t1 : ((t2 > 0) ? t2 : -1);

            if (t > 0)
            {
                Vec3 hit = origin + dir * t;
                return (true, hit);
            }
        }
        
        return (false, Vec3.Zero);

        
    }
    
    public static (Vec3 u,Vec3 v) getUvFromNormal(Vec3 normal)
    {
        // Normalize the normal just to be safe
        Vec3 n = normal.Normalized;

        Vec3 temp;

        // Pick the helper vector
        if (MathF.Abs(n.X) > MathF.Abs(n.Z))
        {
            temp = new Vec3(-n.Y, n.X, 0f);
        }
        else
        {
            temp = new Vec3(0f, -n.Z, n.Y);
        }

        Vec3 u = temp.Normalized;

        Vec3 v = n.Cross(u).Normalized;

        return (u, v);
    }
    
}