using System.Net.Http.Headers;
using System.Text.Json;

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
    public static void OnHudRenderDirect2D(RendererDirect2D gfx, float delta) {
            // For example, before the loop:
            

            
            if (!Globals.NotInGui)
            {
                Onix.Render.Direct2D.RenderText(new Vec2(0,0),ColorF.White,Globals.undoPoint.ToString(),1f );
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
                Globals.CommandBox.Render(commandLine);

                //console area
                
                if (Globals.CommandBox.Text != Autocomplete.lastCommandBoxText)
                {
                    Autocomplete.isPreviewing = false;
                }
                Autocomplete.lastCommandBoxText = Globals.CommandBox.Text;

                

                string[] splitMessage = Globals.CommandBox.Text.Split(' ');

                
                
                List<String> completionOptions = new List<string>();
                string text = "";
                bool foundOne = false;
                
                var (args,argCount) = Globals.SimpleSplit(Globals.CommandBox.Text); // argCount is not 0-based !!

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

                float startIterationsPosition = screenHeight * 0.14f;
                float characterHeight = 7;
                float endPoint = screenHeight * 0.83f;
                
                for (int i = 0; i <= Globals.UndoHistory.Count; i++)
                {
                    
                    Vec2 tlTextPos = new Vec2 (screenWidth*0.67f, startIterationsPosition + characterHeight * i);
                    Vec2 brTextPos = new Vec2 (screenWidth*0.83f, startIterationsPosition +(characterHeight*i) + characterHeight);
                    
                    if (brTextPos.Y >= endPoint)
                    {
                        break;
                    }
                    if (i >= 0 && i < Globals.UndoHistory.Count)
                    {
                        
                        // if (Globals.UndoHistory[i].Id)
                        ColorF colour = ColorF.White;
                        if (i == Globals.undoPoint)
                        {
                            
                            colour = ColorF.Red;
                        }
                        
                        Onix.Render.Direct2D.RenderText(tlTextPos,colour,Globals.UndoHistory[i].Text,TextAlignment.Left,TextAlignment.Top,characterHeight/6);
                        // Rect button = new Rect(
                        //     new Vec2(tlTextPos.X- 10, tlTextPos.Y+2),
                        //     new Vec2(tlTextPos.X -2, tlTextPos.Y + 10));
                        //
                        // Globals.RevertButtons[i] = new HistoryActions.HistoryItem(i,false,button,Globals.UndoHistory[i].Id);
                        // Onix.Render.Direct2D.RenderTexture(button,Globals.UndoIcon,1f);
                        
                    }


                    Rect undo = new Rect(new Vec2(screenWidth * 0.80f, screenHeight * 0.12f),
                        new Vec2((screenWidth * 0.80f)+10, (screenHeight * 0.12f)+10));
                    
                    Onix.Render.Direct2D.RenderTexture(undo,Globals.UndoIcon,1f);
                    
                    Rect redo = new Rect(new Vec2(screenWidth * 0.815f, screenHeight * 0.12f),
                        new Vec2((screenWidth * 0.815f)+10, (screenHeight * 0.12f)+10));
                    
                    Onix.Render.Direct2D.RenderTexture(redo,Globals.RedoIcon,1f);
                   
                    Rect clear = new Rect(new Vec2(screenWidth * 0.785f, screenHeight * 0.12f),
                        new Vec2((screenWidth * 0.785f)+9, (screenHeight * 0.12f)+9));
                    
                    Onix.Render.Direct2D.RenderTexture(clear,Globals.ClearIcon,1f);
                }
            } 
            Globals.CommandBox.IsFocused = !Globals.NotInGui;
        }
}