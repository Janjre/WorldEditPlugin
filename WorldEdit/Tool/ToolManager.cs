using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.Rendering;

namespace WorldEdit.Tools;

public static class ToolManager
{
    public static List<BaseTool> RegisteredTools = new List<BaseTool>();

    public static void AddTool(BaseTool baseTool)
    {
        RegisteredTools.Add(baseTool);
        CommandEnumRegistry.AddSoftEnumValue("tools",baseTool.Name);
    }

    public static void RemoveTool(BaseTool baseTool)
    {
        RegisteredTools.Remove(baseTool);
        CommandEnumRegistry.RemoveSoftEnumValue("tools",baseTool.Name);
    }

    // public static bool RenderTenthSlot(RendererCommon2D gfx)
    // {
    //     
    //     return true;
    // }
}