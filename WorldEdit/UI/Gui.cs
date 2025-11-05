using System.Net.Http.Headers;
using System.Text.Json;
using OnixRuntime.Api.OnixClient;
using WorldEdit.UI;
using WorldEdit.UI.Main;
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
    
    public static string assetsPath;
    
    public static void DrawScreen(RendererCommon2D gfx) {
        gfx.RenderText(new Vec2(0,20), ColorF.White, "NEW" + ConsoleUI.shoudCommandBoxBeFocused.ToString());
        
        gfx.RenderText(new Vec2(0,0),ColorF.White,History.undoPoint.ToString(),1f );
        float screenWidth = Onix.Gui.ScreenSize.X;
        float screenHeight = Onix.Gui.ScreenSize.Y;
        
        Rect mainArea = new Rect(new Vec2(screenWidth * 0.13f, screenHeight * 0.10f), new Vec2(screenWidth * 0.60f, screenHeight * 0.85f));
        
        ColorF darkGray = new ColorF(0.1f, 0.1f, 0.1f,0.95f);
        Onix.Render.Direct2D.FillRoundedRectangle(mainArea, darkGray , 10, 10);
        Onix.Render.Direct2D.DrawRoundedRectangle(mainArea, ColorF.White , 0.25f, 10);

        float tabHeightMain = screenHeight * 0.03f;
        Rect tabAreaMain = new Rect(new Vec2(mainArea.BottomLeft.X + 5, mainArea.TopLeft.Y+5),
            new Vec2(mainArea.BottomRight.X - 5, mainArea.TopRight.Y + tabHeightMain+5));
        
        
        float tabSizeMain = ((mainArea.BottomRight.X - mainArea.BottomLeft.X) / TabManager.MainTabs.Count)-TabManager.MainTabs.Count*5;
        
        float tabPointMain = 0.0f;

        tabPointMain += 0f;

        foreach (Tab tab in TabManager.MainTabs)
        {
            tab.Button = new Rect(
                new Vec2(tabAreaMain.BottomLeft.X+tabPointMain,tabAreaMain.TopLeft.Y),
                new Vec2(tabAreaMain.BottomLeft.X+tabPointMain+tabSizeMain,tabAreaMain.TopLeft.Y+tabHeightMain));
            ColorF colour = new ColorF("383838");
            if (tab.TabNumber == TabManager.selectedTabMain.TabNumber)
            {
                colour = new ColorF("4c4c4a");
            }

            gfx.FillRoundedRectangle(tab.Button, colour, 1f);
            
            
            
            gfx.RenderText(tab.Button,ColorF.White,tab.Name,TextAlignment.Center,TextAlignment.Top);
            tabPointMain += tabSizeMain + 5;
        }
        TabManager.selectedTabMain.Render(mainArea, screenHeight, screenWidth, -1);
        


        Rect sidebarArea = new Rect(new Vec2(screenWidth * 0.65f, screenHeight * 0.10f), new Vec2(screenWidth * 0.85f, screenHeight * 0.85f));
        Onix.Render.Direct2D.FillRoundedRectangle(sidebarArea,darkGray,10,10);
        Onix.Render.Direct2D.DrawRoundedRectangle(sidebarArea, ColorF.White , 0.25f, 10);

        float tabHeightSide = screenHeight * 0.03f;
        Rect tabAreaSide = new Rect(new Vec2(sidebarArea.BottomLeft.X + 5, sidebarArea.TopLeft.Y+5),
            new Vec2(sidebarArea.BottomRight.X - 5, sidebarArea.TopRight.Y + tabHeightSide+5));
        
        float tabSizeSide = ((tabAreaSide.BottomRight.X - tabAreaSide.BottomLeft.X) / TabManager.SideTabs.Count)-TabManager.SideTabs.Count*5;
        
        float tabPointSide = 0.0f;

        tabPointSide += 7.5f;
        foreach (Tab tab in TabManager.SideTabs)
        {
            tab.Button = new Rect(
                new Vec2(tabAreaSide.BottomLeft.X+tabPointSide,tabAreaSide.TopLeft.Y),
                new Vec2(tabAreaSide.BottomLeft.X+tabPointSide+tabSizeSide,tabAreaSide.TopLeft.Y+tabHeightSide));
            ColorF colour = new ColorF("383838");
            if (tab.TabNumber == TabManager.selectedTabSide.TabNumber)
            {
                colour = new ColorF("4c4c4a");
            }
            Onix.Render.Direct2D.FillRoundedRectangle(tab.Button, colour, 1f);
            
            
            
            Onix.Render.Direct2D.RenderText(tab.Button,ColorF.White,tab.Name,TextAlignment.Center,TextAlignment.Top);
            tabPointSide += tabSizeSide + 5;
        }


        if (ConsoleUI.CommandBox.IsFocused == false && ConsoleUI.previousIsFocused == true && ConsoleUI.shoudCommandBoxBeFocused == true )
        { 
            
            //user has pressed escape to leave the textbox, this means we make the artificial setter know its false
            Console.WriteLine("Reached 111!");
            ConsoleUI.shoudCommandBoxBeFocused = false;
            ConsoleUI.previousIsFocused = false;
        }
        
        ConsoleUI.CommandBox.IsFocused = ConsoleUI.shoudCommandBoxBeFocused; // the ONLY place where ConsoleUI.CommandBox.IsFocused should be set

        ConsoleUI.previousIsFocused = ConsoleUI.CommandBox.IsFocused; 
        
        
        
        
        TabManager.selectedTabSide.Render(sidebarArea,screenHeight,screenWidth,-1);
    }
}