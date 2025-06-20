namespace WorldEdit;

public static class Autocomplete
{

    public static string lastCommandBoxText;
    public static List<String> currentOptions = new List<string>();
    public static string Selected;
    public static List<commandObject> commands = new List<commandObject>();
    public static int lastTabbed;
    public static bool isPreviewing;

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
        
        
}