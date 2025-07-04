namespace WorldEdit.UI;

public static class TabManager
{
    
    
    public static List<Tab> SideTabs = new();
    public static List<Tab> MainTabs = new();
    public static Tab selectedTab;
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