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
                
                foreach (Autocomplete.commandObject command in Autocomplete.commands)
                {
                    if (Globals.CommandBox.Text.StartsWith(command.Name))
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
                            if (!Globals.CommandBox.Text.EndsWith(' '))
                            {
                                if (Globals.IndexExists(args, argCount - 1))
                                {
                                    argumentSoFar = args[argCount - 1];    
                                }
                            }
                            
                            if (command.CompleteOptions[argCount - 1 - 1][0] == "pattern") // this is a block pattern. We need to have special logic for picking what to show. Example block pattern "perlin$0.1%50%dirt,50%air"
                            {
                                var firstSplit =argumentSoFar.Split('$');


                                if (Globals.IndexExists(firstSplit.ToList(), 1))
                                {
                                    Console.WriteLine("Getting here 5");


                                    if (firstSplit.Length == 1) // hasn't specified type of noise
                                    {

                                        Console.WriteLine("Getting her 6");
                                        List<string> potentialNoises = new List<string>();
                                        potentialNoises.Add("white");
                                        potentialNoises.Add("perlin");
                                        potentialNoises.Add("roughPerlin");

                                        foreach (string noise in potentialNoises)
                                        {
                                            if (noise.Contains(argumentSoFar) || argumentSoFar == "")
                                            {
                                                completionOptions.Add(noise);
                                            }
                                            else
                                            {
                                            }
                                        }


                                    }
                                    else if (firstSplit.Length == 2) // is specifying zoom, let them pick a float
                                    {
                                        Console.WriteLine("Getting here 4");
                                    }
                                    else if (firstSplit.Length == 3) // is specifying pattern (eg. 50%dirt,50%stone
                                    {
                                        Console.WriteLine("Getting here 3");
                                        if (Globals.IndexExists(firstSplit.ToList(), 2))
                                        {
                                            Console.WriteLine("Getting here 2");
                                            var secondSplit = firstSplit[2].Split(',');
                                            if (Globals.IndexExists(secondSplit.ToList(), secondSplit.Length - 1))
                                            {
                                                Console.WriteLine("Getting here");
                                                string arg = secondSplit[secondSplit.Length - 1];
                                                var thirdSplit = arg.Split('%');
                                                if (thirdSplit.Length == 2)
                                                {
                                                    Console.WriteLine("Know itrs the right argument");
                                                    string json = File.ReadAllText(Path.Combine(Globals.assetsPath,
                                                        "blockList.json"));
                                                    List<string> blocks =
                                                        JsonSerializer.Deserialize<List<string>>(json);

                                                    foreach (string block in blocks)
                                                    {
                                                        if (block.Contains(thirdSplit[1]))
                                                        {
                                                            completionOptions.Add(block);
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Failed 150");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine($"Failed 140 {thirdSplit.Length -1 } != 2");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Failed 162");
                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine(
                                                $"Globals.IndexExists({firstSplit.ToList()}, 2) == false");

                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{firstSplit.Length} != 3");
                                    }
                                }

                            }
                            else 
                            {
                                Console.WriteLine("Not a pattern argfumwnr");
                                foreach (string option in command.CompleteOptions[argCount - 1 - 1])
                                {
                                    if (Globals.IndexExists(args, argCount - 1))
                                    {
                                        if (option.Contains(args[argCount - 1]))
                                        {
                                            completionOptions.Add(option);
                                        }
                                    }

                                }
                            }
                        }

                        
                        else 
                        {
                            if (Globals.CommandBox.Text.EndsWith(" "))
                            {
                                if (Globals.IndexExists(command.CompleteOptions, argCount - 1))
                                {
                                    if (command.CompleteOptions[argCount-1][0] == "pattern")
                                    {
                                        completionOptions.Add("white");
                                        completionOptions.Add("perlin");
                                        completionOptions.Add("roughPerlin");
                                    }
                                    else
                                    {
                                        completionOptions = command.CompleteOptions[argCount - 1];
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
                
                

                if (foundOne == false)
                {
                    
                    
                    if (splitMessage.Length > 0)
                    {
                        string partial = splitMessage[0];
                        foreach (var command in Autocomplete.commands)
                        {
                            if (command.Name.Contains(partial))
                            {
                                completionOptions.Add(command.Name);
                            }
                        }

                        if (completionOptions.Count > 0 && !completionOptions.Contains(Autocomplete.Selected))
                        {
                            if (!Autocomplete.isPreviewing)
                            {
                                Autocomplete.Selected = completionOptions[0];
                            }
                            
                        }
                    }
                    

                    
                }

                if (completionOptions.Count > 0 && !completionOptions.Contains(Autocomplete.Selected))
                {
                    if (!Autocomplete.isPreviewing)
                    {
                        Autocomplete.Selected = completionOptions[0];
                    }

                }





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


                //create lots of textboxes for autocomplete/previous commands to go

                Vec2 tlTextArea = new Vec2(screenWidth * 0.15f, screenHeight * 0.12f);
                Vec2 brTextArea = new Vec2(screenWidth * 0.58f, screenHeight * 0.69f);
                Rect textArea = new Rect(tlTextArea, brTextArea);

                

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
                // Console.WriteLine("Set completion options");





            




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