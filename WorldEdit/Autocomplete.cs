using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Xml;
using WorldEdit.UI.Main;

namespace WorldEdit;

public static class Autocomplete
{

    public static string lastCommandBoxText;
    public static List<String> currentOptions = new List<string>();
    public static string Selected;
    public static List<commandObject> commands = new List<commandObject>();
    public static int lastTabbed;
    public static bool isPreviewing;
    public static bool isCurrentlyANoisePattern;
    
    static private string json = File.ReadAllText(Path.Combine(Gui.assetsPath,
        "blockList.json")); // get blocks
    public static List<string>  blocks =
        JsonSerializer.Deserialize<List<string>>(json); // put them in a list


    public static void RegisterCommand(commandObject command)
    {
        commands.Add(command);
    }
    
    public static List<string> SortCompletionOptions(List<string> options, string input)
    {
        input = input.ToLower();

        List<(string item, int priority, int score)> sorted = new List<(string, int, int)>();

        foreach (string option in options)
        {
            string lower = option.ToLower();
            int priority = 2; // default: other contains
            int score = lower.Length;

            if (lower.StartsWith(input))
            {
                priority = 0;
                if (lower == input)
                    score = int.MinValue; // exact match goes last
            }
            else if (ContainsLettersInOrder(lower, input))
            {
                priority = 1;
            }

            sorted.Add((option, priority, score));
        }

        return sorted
            .OrderBy(item => item.priority)
            .ThenBy(item => item.score)
            .Select(item => item.item)
            .ToList();
    }

    private static bool ContainsLettersInOrder(string word, string pattern)
    {
        int index = 0;
        foreach (char c in pattern)
        {
            index = word.IndexOf(c, index);
            if (index == -1)
                return false;
            index++;
        }
        return true;
    }

        
        
        

    public class commandObject
    {
        public string Name;
        public string Description;
        public List<String> Arguments;
        public List<List<string>> CompleteOptions;
        public Func<string,bool> OnRan; 
        
            
            
            
            
        public commandObject(string name, string description, List<String> argumentHelp,
            List<List<string>> options, Func<string,bool> onRan)
        {
            Name = name;
            Description = description;
            Arguments = argumentHelp;
            CompleteOptions = options;
            OnRan = onRan;
        }
    }


    public static bool Complete()
    {
        var (args, argCount) = Globals.SimpleSplit(ConsoleUI.CommandBox.Text); // argCount is not 0-based

        List<string> noiseOptions = new List<string>();
        noiseOptions.Add("$white");
        noiseOptions.Add("$perlin");
        noiseOptions.Add("$roughPerlin");
        int removeAtEnd = 0;


        if (!string.IsNullOrEmpty(Autocomplete.Selected))
        {
            if (ConsoleUI.CommandBox.Text.EndsWith("  "))
            {
                args.Add(Autocomplete.Selected);
            }
            else
            {
                if (Globals.IndexExists(args, argCount - 1))
                {
                    if (Autocomplete.currentOptions.Contains(args[argCount - 1]))
                    {
                        return true;
                    }

                    if (noiseOptions.Contains(Autocomplete.Selected)) //are you doing noisy things?
                    {
                        removeAtEnd = 4; // put a $ not a " " at the end
                    }

                    string argument = args[argCount - 1];
                    /*
                     * Special logic for noise patterns
                     * If it starts with a $
                     * text = the current arguments string up to the last percentage (including the last percentage)
                     * text += Autocomplete.Selected
                     * args[argCount] = text
                     * make it so it doesn't put a space at the end
                     */
                    string text = "";
                    int lastPercent = argument.LastIndexOf('%');
                    if (argument.StartsWith('$') && lastPercent != -1)
                    {


                        text = argument.Substring(0, lastPercent + 1); // get up to the last percentage
                        text += Autocomplete.Selected; // add the completed bit



                        removeAtEnd = 2; // don't add space
                        args[argCount - 1] = text;
                    }
                    else
                    {
                        args[argCount - 1] = Autocomplete.Selected;
                        if (args[argCount - 1].StartsWith('$'))
                        {
                            removeAtEnd = 4;
                        }
                    }
                }
            }


            

            if (Autocomplete.currentOptions.Contains(Autocomplete.Selected))
            {
                if (args.Count == 0)
                {
                    args.Add(Autocomplete.Selected);
                }


                string reconstruction = "";

                int count = 0;
                foreach (string arg in args)
                {

                    if (arg != "")
                    {
                        reconstruction += arg;

                        if (removeAtEnd == 1)
                        {
                            if (count == args.Count - 3)
                            {
                                reconstruction += "$";
                            }
                            else
                            {
                                reconstruction += " ";
                            }
                        }
                        else if (removeAtEnd == 2)
                        {
                            if (count + 1 == args.Count - 3)
                            {
                                reconstruction += "";
                            }
                            else
                            {
                                reconstruction += " ";
                            }
                        }
                        else if (removeAtEnd == 4) // replace with $ but for pre-started completrions #
                        {
                            if (count + 1 == args.Count - 3)
                            {
                                reconstruction += "$";
                            }
                            else
                            {
                                reconstruction += " ";
                            }
                        }
                        else
                        {
                            reconstruction += " ";
                        }
                    }

                    count++;
                }

                ConsoleUI.CommandBox.Text = reconstruction;
            }
        }

        return false;
    }

    public static List<String> generateOptions()
    {
        string[] splitMessage = ConsoleUI.CommandBox.Text.Split(' ');

                
                
        List<String> completionOptions = new List<string>();
        string text = "";
        bool foundOne = false;
        
        var (args,argCount) = Globals.SimpleSplit(ConsoleUI.CommandBox.Text); // argCount is not 0-based !!
        Autocomplete.isCurrentlyANoisePattern = false;
        foreach (Autocomplete.commandObject command in Autocomplete.commands)
        {
            if (ConsoleUI.CommandBox.Text.StartsWith(command.Name))
            {
                
                
                foundOne = true;
                text = command.Name;
                foreach (string arg in command.Arguments)
                {
                    text += " " + arg;

                }

                // come up with all potential options
                if (Globals.IndexExists(command.CompleteOptions, argCount - 1-1))
                {
                   
                    
                    string argumentSoFar = "";
                    if (!ConsoleUI.CommandBox.Text.EndsWith(' '))
                    {
                        if (Globals.IndexExists(args, argCount - 1))
                        {
                            argumentSoFar = args[argCount - 1];    
                        }
                    }
                    
                    if (command.CompleteOptions[argCount - 1 - 1][0] == "pattern") // this is a block pattern. We need to have special logic for picking what to show. Example block pattern "perlin$0.1%50%dirt,50%air"
                    {
                        if (argumentSoFar.StartsWith('$'))
                        {
                            var argSoFarWithStart = argumentSoFar;
                            argumentSoFar = argumentSoFar.Substring(1); // remove the $ at tghe sart
                            var firstSplit = argumentSoFar.Split('$');
                            

                            if (firstSplit.Length == 1) // hasn't specified type of noise
                            {
                                List<string> potentialNoises = new List<string>();
                                potentialNoises.Add("$white");
                                potentialNoises.Add("$perlin");
                                potentialNoises.Add("$roughPerlin");

                                foreach (string noise in potentialNoises)
                                {
                                    if (noise.Contains(argSoFarWithStart) || argSoFarWithStart == "")
                                    {
                                        completionOptions.Add(noise);
                                    }
                                }
                            }
                            else if (firstSplit.Length == 2) // is specifying zoom, let them pick a float
                            {
                            }
                            else if (firstSplit.Length == 3) // is specifying pattern (eg. 50%dirt,50%stone
                            {
                                if (Globals.IndexExists(firstSplit.ToList(), 2))
                                {
                                    var secondSplit =
                                        firstSplit[2].Split(','); // each secondSplit[] would be 50%dirt
                                    if (Globals.IndexExists(secondSplit.ToList(), secondSplit.Length - 1))
                                    {
                                        string arg =
                                            secondSplit[secondSplit.Length - 1]; // gets the last one
                                        var thirdSplit = arg.Split('%'); // [50,dirt]
                                        if (thirdSplit.Length == 2) // if the dirt bit exists
                                        {
                                            string json = File.ReadAllText(Path.Combine(Gui.assetsPath,
                                                "blockList.json")); // get blocks
                                            List<string> blocks =
                                                JsonSerializer.Deserialize<List<string>>(json); // put them in a list

                                            isCurrentlyANoisePattern =
                                                true; // set the autocompelte thing

                                            foreach (string block in blocks)
                                            {
                                                if (block.Contains(thirdSplit[1])) //if the block fits
                                                {
                                                    completionOptions.Add(block); // add it
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            
                        }
                        else
                        {
                            if (Globals.IndexExists(args, argCount - 1)) // check we can index it
                            {
                                string json = File.ReadAllText(Path.Combine(Gui.assetsPath,
                                    "blockList.json")); // get blocks
                                List<string> options =
                                    JsonSerializer.Deserialize<List<string>>(json); // put them in a list
 
                                foreach (string option in options) // for each option
                                {
                            
                                
                                    if (option.Contains(args[argCount - 1])) // if it is in the argument
                                    {
                                        completionOptions.Add(option); // add it
                                    }
                                

                                }    
                            }
                        }
                    } //end of all the pattern stuff
                    else // not in a pattern, are starting with a known commands and command.CompleteOptions[argCount-2] is safe
                    {
                        if (Globals.IndexExists(args, argCount - 1)) // check we can index it
                        {
                            foreach (string option in command.CompleteOptions[argCount - 1 - 1]) // for each option
                            {
                                if (option.Contains(args[argCount - 1])) // if it is in the argument
                                {
                                    completionOptions.Add(option); // add it
                                }
                            }    
                        }
                    }
                }
                else 
                {
                    if (ConsoleUI.CommandBox.Text.EndsWith(" ")) // hasn't started typing command yet, show them alll of the optpiopmns
                    {
                        if (Globals.IndexExists(command.CompleteOptions, argCount - 1))
                        {
                            completionOptions = command.CompleteOptions[argCount - 1];
                            if (command.CompleteOptions[argCount - 1][0] == "pattern")
                            {
                                List<string> potentialOptions = new List<string>(blocks); // make copy of it
                                potentialOptions.Add("$perlin");
                                potentialOptions.Add("$white");
                                potentialOptions.Add("$roughPerlin");
                                completionOptions = potentialOptions;
                            }
                        }
                    }
                }
                break;
            }
        }
        
        

        if (foundOne == false) // no command found
        {
            if (splitMessage.Length > 0) // checks it exists
            {
                string partial = splitMessage[0];
                foreach (var command in commands)
                {
                    if (command.Name.Contains(partial))
                    {
                        completionOptions.Add(command.Name);
                    }
                }

                if (completionOptions.Count > 0 && !completionOptions.Contains(Autocomplete.Selected))
                {
                    if (!isPreviewing)
                    {
                        Selected = completionOptions[0];
                    }
                    
                }
            }
        }

        if (completionOptions.Count > 0 && !completionOptions.Contains(Autocomplete.Selected))
        {
            if (!isPreviewing)
            {
                Selected = completionOptions[0];
            }

        }

        return completionOptions;
    }
}