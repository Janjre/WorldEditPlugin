using System.Net.Http.Headers;
using System.Text.Json;
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
    
    public static void OnHudRenderDirect2D(RendererDirect2D gfx, float delta) {
            // For example, before the loop:
            

            
            if (!Gui.NotInGui)
            {
                Onix.Render.Direct2D.RenderText(new Vec2(0,0),ColorF.White,History.undoPoint.ToString(),1f );
                float screenWidth = Onix.Gui.ScreenSize.X;
                float screenHeight = Onix.Gui.ScreenSize.Y;
                
                Rect consoleArea = new Rect(new Vec2(screenWidth * 0.13f, screenHeight * 0.10f), new Vec2(screenWidth * 0.60f, screenHeight * 0.85f));
                // Onix.Render.Direct2D.DrawRoundedRectangle(myRect, ColorF.Gray, 10,10);
                
                ColorF darkGray = new ColorF(0.1f, 0.1f, 0.1f,0.95f);
                Onix.Render.Direct2D.FillRoundedRectangle(consoleArea, darkGray , 10, 10);
                Onix.Render.Direct2D.DrawRoundedRectangle(consoleArea, ColorF.White , 0.25f, 10);

                ColorF lightGray = new ColorF(0.34f, 0.34f, 0.34f, 1f);
                Rect commandLine = new Rect(new Vec2(screenWidth * 0.15f, screenHeight * 0.75f), new Vec2(screenWidth * 0.58f, screenHeight * 0.82f));
                Onix.Render.Direct2D.FillRoundedRectangle(commandLine, lightGray , 5f, 10);
                Gui.CommandBox.Render(commandLine);

                //console area
                
                if (Gui.CommandBox.Text != Autocomplete.lastCommandBoxText)
                {
                    Autocomplete.isPreviewing = false;
                }
                Autocomplete.lastCommandBoxText = CommandBox.Text;

                

                string[] splitMessage = CommandBox.Text.Split(' ');

                
                
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

                TabManager.selectedTab.Render(sidebarArea, screenHeight, screenWidth);
                
            } 
            CommandBox.IsFocused = !NotInGui;
        }
}