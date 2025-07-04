using OnixRuntime.Api;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;

namespace WorldEdit.UI.Main;

public class ConsoleUI
{
    public static OnixTextbox CommandBox = new OnixTextbox(128, "", "World edit command here");
    public static bool render(Rect renderArea, float screenWidth, float screenHeight, float delta)
    {
        ColorF lightGray = new ColorF(0.34f, 0.34f, 0.34f, 1f);
        Rect commandLine = new Rect(new Vec2(screenWidth * 0.15f, screenHeight * 0.75f),
            new Vec2(screenWidth * 0.58f, screenHeight * 0.82f));
        Onix.Render.Direct2D.FillRoundedRectangle(commandLine, lightGray, 5f, 10);
        CommandBox.Render(commandLine);

        //console area

        if (CommandBox.Text != Autocomplete.lastCommandBoxText)
        {
            Autocomplete.isPreviewing = false;
        }

        Autocomplete.lastCommandBoxText = CommandBox.Text;

        List<String> completionOptions = new List<string>();
        string text = "";
        bool foundOne = false;

        var (args, argCount) = Globals.SimpleSplit(CommandBox.Text); // argCount is not 0-based !!

        completionOptions = Autocomplete.generateOptions();


        //create lots of textboxes for autocomplete/previous commands to go

        Vec2 tlTextArea = new Vec2(screenWidth * 0.15f, screenHeight * 0.12f);
        Vec2 brTextArea = new Vec2(screenWidth * 0.58f, screenHeight * 0.69f);
        Rect textArea = new Rect(tlTextArea, brTextArea);

        Rect syntaxLine = new Rect(new Vec2(screenWidth * 0.15f, screenHeight * 0.71f),
            new Vec2(screenWidth * 0.58f, screenHeight * 0.78f));
        Onix.Render.Direct2D.RenderText(syntaxLine, ColorF.White, text, TextAlignment.Left, TextAlignment.Top, 1f);

        if (Globals.IndexExists(args, argCount - 1))
        {
            completionOptions = Autocomplete.SortCompletionOptions(completionOptions, args[argCount - 1]);
        }

        Autocomplete.currentOptions = completionOptions;

        if (completionOptions.Count > 0)
        {
            if (!Autocomplete.isPreviewing)
            {
                Autocomplete.Selected = completionOptions[0];
            }
        }
        else
        {
            if (!Autocomplete.isPreviewing)
            {
                Autocomplete.Selected = null;
            }

        }

        float lineHeight = 7f;
        float startY = screenHeight * 0.68f;

        for (int i = 0; i < completionOptions.Count; i++)
        {
            string option = completionOptions[i];

            Vec2 topLeft = new Vec2(screenWidth * 0.15f, startY - i * lineHeight);
            Vec2 bottomRight = new Vec2(screenWidth * 0.58f, topLeft.Y + lineHeight);
            Rect textBox = new Rect(topLeft, bottomRight);

            if (textBox.TopLeft.Y < textArea.TopLeft.Y)
            {
                break;
            }

            ColorF textColour = ColorF.Gray;

            if (Autocomplete.Selected == option)
            {
                textColour = ColorF.White;
            }

            Onix.Render.Direct2D.RenderText(textBox, textColour, option, TextAlignment.Left, TextAlignment.Top, 1f);
        }

        Autocomplete.currentOptions = completionOptions;
        return true;
    }
    
    public static Tab tab = new (render, 1, "Console", History.UndoIcon,0);
}