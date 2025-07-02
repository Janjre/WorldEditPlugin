using System.Text.Json;
using System.Text.Json.Nodes;
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



public static class Fill
{
    public static bool RunFill(String arguments)
    {
        
                
        String[] splitMessage = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    
        

        long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);
                
        (Vec3 posMin, Vec3 posMax) = Globals.FindExtremes(WorldEdit.Selection.pos1, WorldEdit.Selection.pos2);
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

    public static Autocomplete.commandObject FillInit()
    {
        List<String> autocomplete = new List<string>();
        autocomplete.Add("<block>");
        
        List<List<String>> options = new List<List<string>>();
        

        string json = File.ReadAllText(Path.Combine(Gui.assetsPath,"blockList.json"));
        
        List<string> argument1 = JsonSerializer.Deserialize<List<string>>(json);
 // List<string>();
        //
        // argument1.Add("air");
        // argument1.Add("dirt");
        // argument1.Add("stone");
        // argument1.Add("gold_block");
        // argument1.Add("diamond_block");
        
        // List<String> argument1 = new//
        options.Add(argument1);
                    
        return new Autocomplete.commandObject("fill","test",autocomplete,options,RunFill);
    }
}