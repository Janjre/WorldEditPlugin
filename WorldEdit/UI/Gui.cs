using System.Net.Http.Headers;
using System.Text.Json;
using OnixRuntime.Api.OnixClient;
using WorldEdit.UI;
using WorldEdit.UI.Sidebar;

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
using OnixRuntime.Api.UI;
using OnixRuntime.Api.World;

public class Gui
{
    public static bool NotInGui = true;
    public static OnixTextbox CommandBox = new OnixTextbox(128, "", "World edit command here");
    public static string assetsPath;
    
    public static void DrawScreen(RendererCommon2D gfx) {
        
            gfx.RenderText(new Vec2(0,0),ColorF.White,History.undoPoint.ToString(),1f );
            float screenWidth = Onix.Gui.ScreenSize.X;
            float screenHeight = Onix.Gui.ScreenSize.Y;
            
            Rect consoleArea = new Rect(new Vec2(screenWidth * 0.13f, screenHeight * 0.10f), new Vec2(screenWidth * 0.60f, screenHeight * 0.85f));
            
            ColorF darkGray = new ColorF(0.1f, 0.1f, 0.1f,0.95f);
            Onix.Render.Direct2D.FillRoundedRectangle(consoleArea, darkGray , 10, 10);
            Onix.Render.Direct2D.DrawRoundedRectangle(consoleArea, ColorF.White , 0.25f, 10);

            ColorF lightGray = new ColorF(0.34f, 0.34f, 0.34f, 1f);
            Rect commandLine = new Rect(new Vec2(screenWidth * 0.15f, screenHeight * 0.75f), new Vec2(screenWidth * 0.58f, screenHeight * 0.82f));
            gfx.FillRoundedRectangle(commandLine, lightGray , 5f, 10);
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
            
            var (args,argCount) = Globals.SimpleSplit(CommandBox.Text); // argCount is not 0-based !!

            completionOptions = Autocomplete.generateOptions();


            //create lots of textboxes for autocomplete/previous commands to go

            Vec2 tlTextArea = new Vec2(screenWidth * 0.15f, screenHeight * 0.12f);
            Vec2 brTextArea = new Vec2(screenWidth * 0.58f, screenHeight * 0.69f);
            Rect textArea = new Rect(tlTextArea, brTextArea);

            Rect syntaxLine = new Rect(new Vec2(screenWidth * 0.15f, screenHeight * 0.71f), new Vec2(screenWidth * 0.58f, screenHeight * 0.78f));
            Onix.Render.Direct2D.RenderText(syntaxLine,ColorF.White, text,TextAlignment.Left,TextAlignment.Top,1f);

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

                Onix.Render.Direct2D.RenderText(textBox, textColour , option, TextAlignment.Left, TextAlignment.Top, 1f);
            }

            Autocomplete.currentOptions = completionOptions;
            

        




            Rect sidebarArea = new Rect(new Vec2(screenWidth * 0.65f, screenHeight * 0.10f), new Vec2(screenWidth * 0.85f, screenHeight * 0.85f));
            Onix.Render.Direct2D.FillRoundedRectangle(sidebarArea,darkGray,10,10);
            Onix.Render.Direct2D.DrawRoundedRectangle(sidebarArea, ColorF.White , 0.25f, 10);

            float tabHeight = screenHeight * 0.03f;
            Rect tabArea = new Rect(new Vec2(sidebarArea.BottomLeft.X + 5, sidebarArea.TopLeft.Y+5),
                new Vec2(sidebarArea.BottomRight.X - 5, sidebarArea.TopRight.Y + tabHeight+5));
            
            float tabSize = ((tabArea.BottomRight.X - tabArea.BottomLeft.X) / TabManager.tabs.Count)-TabManager.tabs.Count*5;
            
            float tabPoint = 0.0f;

            tabPoint += 7.5f;
            foreach (Tab tab in TabManager.tabs)
            {
                tab.Button = new Rect(
                    new Vec2(tabArea.BottomLeft.X+tabPoint,tabArea.TopLeft.Y),
                    new Vec2(tabArea.BottomLeft.X+tabPoint+tabSize,tabArea.TopLeft.Y+tabHeight));
                ColorF colour = new ColorF("383838");
                if (tab.TabNumber == TabManager.selectedTab.TabNumber)
                {
                    colour = new ColorF("4c4c4a");
                }
                Onix.Render.Direct2D.FillRoundedRectangle(tab.Button, colour, 1f);
                
                
                
                Onix.Render.Direct2D.RenderText(tab.Button,ColorF.White,tab.Name,TextAlignment.Center,TextAlignment.Top);
                tabPoint += tabSize + 5;
            }

            // TabManager.selectedTab.Render(sidebarArea, screenHeight, screenWidth,delta);
            

            
            CommandBox.IsFocused = !NotInGui;
        }
}