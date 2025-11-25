using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.World;
using WorldEdit.Tool;

namespace WorldEdit.Tool.Tools;

public class SmoothTool : BaseTool
{
    private float[,,] _kernel5 ={ 
        {
            { 0.000162f, 0.000725f, 0.001195f, 0.000725f, 0.000162f },
            { 0.000725f, 0.003247f, 0.005351f, 0.003247f, 0.000725f },
            { 0.001195f, 0.005351f, 0.008814f, 0.005351f, 0.001195f },
            { 0.000725f, 0.003247f, 0.005351f, 0.003247f, 0.000725f },
            { 0.000162f, 0.000725f, 0.001195f, 0.000725f, 0.000162f }
        },
        {
            { 0.000725f, 0.003247f, 0.005351f, 0.003247f, 0.000725f },
            { 0.003247f, 0.014534f, 0.023951f, 0.014534f, 0.003247f },
            { 0.005351f, 0.023951f, 0.039447f, 0.023951f, 0.005351f },
            { 0.003247f, 0.014534f, 0.023951f, 0.014534f, 0.003247f },
            { 0.000725f, 0.003247f, 0.005351f, 0.003247f, 0.000725f }
        },
        {
            { 0.001195f, 0.005351f, 0.008814f, 0.005351f, 0.001195f },
            { 0.005351f, 0.023951f, 0.039447f, 0.023951f, 0.005351f },
            { 0.008814f, 0.039447f, 0.065010f, 0.039447f, 0.008814f },
            { 0.005351f, 0.023951f, 0.039447f, 0.023951f, 0.005351f },
            { 0.001195f, 0.005351f, 0.008814f, 0.005351f, 0.001195f }
        },
        {
            { 0.000725f, 0.003247f, 0.005351f, 0.003247f, 0.000725f },
            { 0.003247f, 0.014534f, 0.023951f, 0.014534f, 0.003247f },
            { 0.005351f, 0.023951f, 0.039447f, 0.023951f, 0.005351f },
            { 0.003247f, 0.014534f, 0.023951f, 0.014534f, 0.003247f },
            { 0.000725f, 0.003247f, 0.005351f, 0.003247f, 0.000725f }
        },
        {
            { 0.000162f, 0.000725f, 0.001195f, 0.000725f, 0.000162f },
            { 0.000725f, 0.003247f, 0.005351f, 0.003247f, 0.000725f },
            { 0.001195f, 0.005351f, 0.008814f, 0.005351f, 0.001195f },
            { 0.000725f, 0.003247f, 0.005351f, 0.003247f, 0.000725f },
            { 0.000162f, 0.000725f, 0.001195f, 0.000725f, 0.000162f }
        }
    };
    
    private float[,,] _kernel3 = new float[,,]
    {
        { // z = 0
            {0.015625f, 0.03125f, 0.015625f},
            {0.03125f, 0.0625f, 0.03125f},
            {0.015625f, 0.03125f, 0.015625f}
        },
        { // z = 1
            {0.03125f, 0.0625f, 0.03125f},
            {0.0625f, 0.125f, 0.0625f},
            {0.03125f, 0.0625f, 0.03125f}
        },
        { // z = 2
            {0.015625f, 0.03125f, 0.015625f},
            {0.03125f, 0.0625f, 0.03125f},
            {0.015625f, 0.03125f, 0.015625f}
        }
    };


    

    public SmoothTool() : base("smooth", "wooden_sword",false) {}
    
    
    public override bool OnPressed(InputKey key, bool isDown)
    {
        if (key == InputKey.Type.LMB && isDown)
        {
            Vec3 targetPosition = new Vec3();
            bool sucessful = false;
            for (int i = 1; i <= 500; i++)
            {
                Vec3 thisPosition = Onix.LocalPlayer.ForwardPosition(i);
                if (Onix.Region.GetBlock((int)thisPosition.Floor().X, (int)thisPosition.Floor().Y,
                        (int)thisPosition.Floor().Z).Name != "air")
                {
                    targetPosition = thisPosition.Floor();
                    sucessful = true;
                    
                    break;
                }
            }

            if (sucessful)
            {
                long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);
                
                for (int x = (int)targetPosition.X - SmoothToolManager.Size; x <= (int)targetPosition.X + SmoothToolManager.Size; x++)
                {
                    for (int y = (int)targetPosition.Y - SmoothToolManager.Size; y <= (int)targetPosition.Y + SmoothToolManager.Size; y++)
                    {
                        for (int z = (int)targetPosition.Z - SmoothToolManager.Size; z <= (int)targetPosition.Z + SmoothToolManager.Size; z++)
                        {
                            if ((targetPosition - new Vec3(x, y, z)).Length <= SmoothToolManager.Size)
                            {
                                string block;
                                if (SmoothToolManager.LargeKernel)
                                { 
                                    block = Maths.BlurCalculator.ApplyKernel(_kernel5, new Vec3(x, y, z));
                                }
                                else
                                {
                                    block = Maths.BlurCalculator.ApplyKernel(_kernel3, new Vec3(x, y, z));
                                }
                                
                                History.PlaceBlock(block,"[]",new Vec3(x,y,z), actionId);
                            }
                        }
                    }
                }
                
                History.FinishAction(actionId, $"Blurred area");
            }
        }

        if (key.IsMouse)
        {
            return true;
        }

        return false;
    }

    public override void OnRender(RendererWorld gfx, float delta)
    {
        
    }
}