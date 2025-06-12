namespace WorldEdit;

using System.ComponentModel;
using System.Runtime.InteropServices.JavaScript;
using System.Xml;
using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Events;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Plugin;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.World;



public static class Commands
{
    public static bool Fill(String arguments)
    {
        
                
        String[] splitMessage = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    
        

        long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);
                
        (Vec3 posMin, Vec3 posMax) = Globals.FindExtremes(Globals.pos1, Globals.pos2);
        // loop through area!!!! (this is incredibly unique, i don't think we will do this anywhere else in the plugin!!!!)
                
        for (int x = (int)posMin.X; x <= posMax.X; x++)
        {
            for (int y = (int)posMin.Y; y <= posMax.Y; y++)
            {
                for (int z = (int)posMin.Z; z <= posMax.Z; z++)
                {
                    // Console.WriteLine("attempted to do something");
                    HistoryActions.PlaceBlock(splitMessage[1], "[]",new Vec3(x,y,z),actionId);
                            
                }
            }
        }
        HistoryActions.FinishAction(actionId, "Filled area with "+splitMessage[1]);

                
        
        return true;
    } 
}