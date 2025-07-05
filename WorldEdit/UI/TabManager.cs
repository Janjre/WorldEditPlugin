namespace WorldEdit.UI;

public static class TabManager
{
    
    
    public static List<Tab> SideTabs = new();
    public static List<Tab> MainTabs = new();
    public static Tab selectedTabMain;
    public static Tab selectedTabSide;
    public static void registerTab(Tab tab)
    {
        if (tab.Type == 0)
        {
            MainTabs.Add(tab);
        }
        else
        {
            SideTabs.Add(tab);
        }
        
    }
    
}