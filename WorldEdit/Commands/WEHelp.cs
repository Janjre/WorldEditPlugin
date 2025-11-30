using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.OnixClient.Commands;
using OnixRuntime.Api.World;

namespace WorldEdit.Commands {
    
    public class WEHelp : OnixCommandBase {
        public WEHelp() : base("WEhelp", "Gives a list of commands and some information about WorldEdit", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        [Overload]
        OnixCommandOutput HelpExecute()
        {
            string cuboid = "§e";
            string arbitary = "§g";
            string both = "§b";
            string reset = "§r";

            Console.WriteLine("§lWorld Edit help - don't read everything if you don't need specific help - just the top and General Info and warnings");
            Console.WriteLine("");
            Console.WriteLine("§lGeneral info");
            Console.WriteLine("Everything you can do is generally is a tool or command. You will see a list of tools and commands below. To select a tool you can either use the tenth slot system or just use the items normally. You can pick which one of these you want to do in the configuration. If you are using the tenth slot system you may need to do some configuration. To move the tenth slot press the configure button. Make sure the keybind for the tenth slot are accurate");
            Console.WriteLine("§lList of commands:");
            
            Console.WriteLine($".box <Block block> - sets all the faces of your {cuboid}cuboid{reset} selection with a block");
            Console.WriteLine($".brush <Item item> <Block block> <int size> [string mask] [string name] - creates a tool which makes a sphere of radius size. If specified, the tool will only affect blocks called mask. ");
            Console.WriteLine($".copy [string name = clipboard] - copies your {cuboid}cuboid{reset} selection as a structure with the specified name");
            Console.WriteLine($"extrude [Block block] [int distance] - extrudes all blocks within {both}any{reset} selection type by distance");
            Console.WriteLine($"history");
            Console.WriteLine($"    clear - clears your history");
            Console.WriteLine($"    list [int depth = 20] lists your most recent actions");
            Console.WriteLine($".line <Block block> - makes a line between {cuboid}pos1{reset} and {cuboid}pos2{reset}");
            Console.WriteLine($".move <int x> <int y> <int z> - moves the blocks within {both}any{reset} selection type");
            Console.WriteLine($".noise <NoiseType type> <string pattern> <zoom> - fills {both}any{reset} selection with some form of noise. You can specify the blocks in your noise and their frequency in the pattern in the format '50%dirt/50%stone'. Zoom is only applicable to perlin and rough noise");
            Console.WriteLine($".paste [PastePosition position = clipboard] - pastes a structure at either your player position or the pos1 of your {cuboid}cuboid{reset} selection");
            Console.WriteLine($".redo - if you have undone an action it will undo that. If you are ever confused do .history list");
            Console.WriteLine($".replace <Block replace_what> <Block replace_with> - replaces all blocks within {both}any{reset} selection type with another block");
            Console.WriteLine($".selection");
            Console.WriteLine($"    clear - clears whatever selection type you currently have selected");
            Console.WriteLine($"    cube - selects a {cuboid}cube{reset} around you");
            Console.WriteLine($"    get - prints info about whatever selection type you currently have");
            Console.WriteLine($"    include <BlockPos pos> - expands the selection so that this block is selection");
            Console.WriteLine($"    mode <SelectionType type> - manually sets what selection type is being used and displayed");
            Console.WriteLine($"    move <Direction dir> <int distance> - moves the selection, not the blocks in it but what it is. Works on {both}both{reset} selection types");
            Console.WriteLine($"    pos1 <BlockPos pos> - sets pos1 of {cuboid}cuboid{reset} selection");
            Console.WriteLine($"    pos2 <BlockPos pos> - sets pos2 of {cuboid}cuboid{reset} selection");
            Console.WriteLine($"    replace <Block block> - selection all blocks of this name in {both}any{reset} selection type");
            Console.WriteLine($"    sphere <int radius> - selects a sphere of the specified radius at your players position as an {arbitary}arbitrary{reset} selection. Laggy - not advised");
            Console.WriteLine($".set <Block block> -sets {both}any{reset} selection to be a certain block. Does not work with noise. Block states and data are unsupported right now");
            Console.WriteLine($".sphere <Block block> - creates a sphere of centre {cuboid}pos1{reset} and radius {cuboid}pos2{reset} - {cuboid}pos1{reset}");
            Console.WriteLine($".tool");
            Console.WriteLine($"    item <Tool tool> <Item new_item> sets the item a tool uses");
            Console.WriteLine($"    large_kernel <bool> sets whether the smooth tool should use a small or large smoothing kernel");
            Console.WriteLine($"    remove <Tool tool> - removes a tool");
            Console.WriteLine($"    show_all_tools <bool> - makes it so you can see all tools HUDs at the same time");
            Console.WriteLine($"    smooth_radius <int radius> - sets the radius of sphere that is effected by the smooth tool");
            Console.WriteLine($".undo - undoes the last action. Do .history list if you are confused");
            Console.WriteLine($".up <int distance> - teleports the player up some blocks, places glass under them and makes sure they are not suffocating");
            Console.WriteLine($".wall <Block block> - creates a potentially diagonal wall between {cuboid}pos1{reset} and {cuboid}pos2{reset}");
            Console.WriteLine($".walls <Block block> - sets all vertical faces to a block. Only works on {cuboid}cuboid{reset} selections");
            Console.WriteLine($".WEhelp - this command!");
            
            
            Console.WriteLine("§lTools");
            Console.WriteLine($"Selection - selects pos1 and pos2 with left and right click to get a {cuboid}cuboid{reset} selection. If you aren't selecting a block it will just do 3 blocks in front of you. Middle clicking a block will expand the selection so it is within it.");
            Console.WriteLine($"Bezier - creates a bezier curve. Add points with left click. Voxelise with right click to get an {arbitary}arbitrary{reset} selection. You can right click to clear an arbitrary selection");
            Console.WriteLine($"Circle - creates a circle. Click to add a centre, one radius then another to get an {arbitary}arbitrary{reset} selection");
            Console.WriteLine($"Smooth - smooths out a sphere with size customisable in .tool with a convolutionary kernel also customisable in .tool");

            Console.WriteLine("§l§cWarnings!§r");
            Console.WriteLine(" - §lMake backups§r when working on things you like");
            Console.WriteLine(" - Block states do not work yet");
            Console.WriteLine(" - If you are undoing be careful as doing most commands will remove any undo history past the point you are undoing to");
            Console.WriteLine(" - This has not been rigorously tested - §lmake backups");

            Console.WriteLine("§l§aSummary - Read this first!");
            Console.WriteLine(" - To select a tool you can either use the tenth slot system or just use the items normally. You can pick which one of these you want to do in the configuration. If you are using the tenth slot system you may need to do some configuration. Press the Configure button in the settings then leave the gui to do this");
            Console.WriteLine(" - Selections can be arbitrary or cuboid. Arbitrary selections are just a list of blocks. Cuboid are defined by two points");
            Console.WriteLine(" - All commands start with the prefix '.'");
            Console.WriteLine(" - Read warnings and any commands/tools you don't know how to use");
            
            
            return Success("");
        }
    }
    
}