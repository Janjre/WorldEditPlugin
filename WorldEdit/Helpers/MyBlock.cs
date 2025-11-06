using OnixRuntime.Api.Maths;

public class MyBlock
{
    public String Name;
    public String Data;
    public Vec3 Position;
    public long Action;

    public MyBlock(String name, String data, long action, Vec3 position)
    {
        Name = name;
        Data = data;
        Action = action;
        Position = position;

    }
}