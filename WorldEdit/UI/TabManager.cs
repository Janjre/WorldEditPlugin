namespace WorldEdit.UI;

public static class TabManager
{
    
    
    public static List<Tab> tabs = new();
    public static Tab selectedTab;
    public static void registerTab(Tab tab)
    {
        tabs.Add(tab);
    }
    
}