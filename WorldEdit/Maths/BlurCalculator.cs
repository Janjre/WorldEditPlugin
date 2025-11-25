using OnixRuntime.Api;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.World;

namespace WorldEdit.Maths;

public static class BlurCalculator
{
    
    public static string ApplyKernel(float[,,] kernel, Vec3 position)
    {

        List<(string, float)> blockWeights = new();
        
        int kernelCenter = kernel.GetLength(0) / 2;
        for (int x = 0; x < kernel.GetLength(0); x++)
        {
            for (int y = 0; y < kernel.GetLength(1); y++)
            {
                for (int z = 0; z < kernel.GetLength(2); z++)
                {
                    float weight = kernel[x,y,z];
                    Vec3 worldPosition = position + new Vec3(x - kernelCenter, y - kernelCenter, z - kernelCenter);

                    
                    string block = Onix.Region.GetBlock((int)worldPosition.Floor().X, (int)worldPosition.Floor().Y,
                    (int)worldPosition.Floor().Z).Name;
                    
                    blockWeights.Add((block,weight));
                }
            }
        }

        List<string> uniqueBlocks = blockWeights
            .Select(x => x.Item1)
            .Distinct()
            .ToList();
        
        List<(string, float)> uniqueBlockWeights = new();
        
        foreach (string block in uniqueBlocks)
        {
            List<(string, float)> blocksOfThisType = blockWeights
                .Where(x => x.Item1 == block)
                .ToList();
        
            float total = 0f;
            foreach ((string,float) blockToAdd in blocksOfThisType)
            {
                total += blockToAdd.Item2;
            }
            
            uniqueBlockWeights.Add((block,total));
            
        }
        
        (string,float) bestBlock = uniqueBlockWeights.MaxBy(x => x.Item2);
        
        return bestBlock.Item1;
    }
}