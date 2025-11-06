using OnixRuntime.Api.Maths;

public static class Globals // no i cannot do things without this
{

    public static Random MyRandom = new Random();
        


    public static (Vec3, Vec3) FindExtremes (Vec3 a, Vec3 b)
    {
        Vec3 posMin = new Vec3(
            Math.Min(a.X, b.X),
            Math.Min(a.Y, b.Y),
            Math.Min(a.Z, b.Z)
        );

        Vec3 posMax = new Vec3(
            Math.Max(a.X, b.X),
            Math.Max(a.Y, b.Y),
            Math.Max(a.Z, b.Z)
        );

        return (posMin, posMax);
    }
        
}