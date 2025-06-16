namespace WorldEdit;

public static class Autocomplete
{

    public static string AutoCompleteSelection;
    public static List<commandObject> commands = new List<commandObject>();

    public static void RegisterCommand(commandObject command)
    {
        commands.Add(command);
    }
        
        
        

    public class commandObject
    {
        public string Name;
        public string Description;
        public List<String> Arguments;
        public List<List<string>> CompleteOptions;
        public Func<string,bool> OnRan; // I want to make this a function that i can call like Autocompelte.commandObject.OnRan(arguments)
            
            
            
            
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
        
        
}