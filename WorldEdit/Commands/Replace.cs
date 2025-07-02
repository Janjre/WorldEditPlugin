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



public static class Replace
{
    public static bool RunReplace(String arguments)
    {
        
                
        String[] splitMessage = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    
        

        long actionId = Globals.MyRandom.NextInt64(1, 1_000_000_001);
                
        (Vec3 posMin, Vec3 posMax) = Globals.FindExtremes(Selection.pos1, Selection.pos2);
                
        for (int x = (int)posMin.X; x <= posMax.X; x++)
        {
            for (int y = (int)posMin.Y; y <= posMax.Y; y++)
            {
                for (int z = (int)posMin.Z; z <= posMax.Z; z++)
                {
                    // Console.WriteLine("attempted to do something");
                    if (Onix.LocalPlayer.Region.GetBlock(x, y, z).Name == splitMessage[1])
                    {
                        HistoryActions.PlaceBlock(splitMessage[2], "[]",new Vec3(x,y,z),actionId);
                    }
                    
                            
                }
            }
        }
        HistoryActions.FinishAction(actionId, $"Replaced {splitMessage[1]} with {splitMessage[2]}");

                
        
        return true;
    }

    public static Autocomplete.commandObject ReplaceInit()
    {
        List<String> autocomplete = new List<string>();
        autocomplete.Add("<replace what: block>");
        autocomplete.Add("<replace with: block>");
        
        List<List<String>> options = new List<List<string>>();
        

        string json = File.ReadAllText(Path.Combine(Gui.assetsPath,"blockList.json"));
        
        List<string> argument1 = JsonSerializer.Deserialize<List<string>>(json);
        List<string> argument2 = JsonSerializer.Deserialize<List<string>>(json);

        options.Add(argument1);
        options.Add(argument2);
                    
        return new Autocomplete.commandObject("replace","test",autocomplete,options,RunReplace);
    }
}