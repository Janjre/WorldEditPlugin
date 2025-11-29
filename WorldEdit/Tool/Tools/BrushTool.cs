using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Items;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.World;
using WorldEdit.Tool;

namespace WorldEdit.Tool.Tools;

public class BrushTool: BaseTool
{
    private Block _block;
    private int _size;
    private string _mask;

    public BrushTool(Item item, Block block, int size, string name = "", string mask = "") : base(name == "" ? item.Name : name, item.Name,true)
    {
        _block = block;
        _size = size;
        _mask = mask;
    }
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
                
                for (int x = (int)targetPosition.X - _size; x <= (int)targetPosition.X + _size; x++)
                {
                    for (int y = (int)targetPosition.Y - _size; y <= (int)targetPosition.Y + _size; y++)
                    {
                        for (int z = (int)targetPosition.Z - _size; z <= (int)targetPosition.Z + _size; z++)
                        {
                            if ((targetPosition - new Vec3(x, y, z)).Length <= _size)
                            {
                                if (Onix.Region.GetBlock(x, y, z).Name == _mask || _mask == "")
                                {
                                    History.PlaceBlock(_block.Name,"[]",new Vec3(x,y,z),actionId);  
                                }
                            }
                        }
                    }
                }
                
                History.FinishAction(actionId, $"Filled sphere with {_block.Name} from brush");
            }
            
               
            return true;
        }

        return false;
    }

    public override void OnRender(RendererWorld gfx, float delta)
    {
        
    }
}