using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Schema;
using WorldEdit;


namespace Commands;

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



public static class Perlin
{
    public static bool RunPerlin(String arguments)
    {
        if (arguments.StartsWith('$'))
        {
            arguments = arguments.Substring(1);
            String[] splitMessage = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            long actionId = WorldEdit.Globals.MyRandom.NextInt64(1, 1_000_000_001);
                
            (Vec3 posMin, Vec3 posMax) = WorldEdit.Globals.FindExtremes(Selection.pos1, Selection.pos2);
        
            
            for (int x = (int)posMin.X; x <= posMax.X; x++)
            {
                for (int y = (int)posMin.Y; y <= posMax.Y; y++)
                {
                    for (int z = (int)posMin.Z; z <= posMax.Z; z++)
                    {
                        string block = Noise.BlockFigure(x, y, z, splitMessage[1], actionId);
                        WorldEdit.HistoryActions.PlaceBlock(block,"[]",new Vec3(x,y,z),actionId);
                    }
                }
            }
            WorldEdit.HistoryActions.FinishAction(actionId, $"Perlined area"); 
            return true;
        }
        else
        {
            return false;
        }
     
    }

    public static WorldEdit.Autocomplete.commandObject PerlinInit()
    {
        List<String> autocomplete = new List<string>();
        autocomplete.Add("<pattern: block>");
        
        
        List<List<String>> options = new List<List<string>>();




        List<string> argument1 = new List<string>();
        argument1.Add("pattern");
        

        options.Add(argument1);
        
                    
        return new WorldEdit.Autocomplete.commandObject("perlin","test",autocomplete,options,RunPerlin);
    }
}