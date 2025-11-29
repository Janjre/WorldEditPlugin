using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.OnixClient;
using WorldEdit.Tool;

namespace WorldEdit {
    public partial class WorldEditConfig : OnixModuleSettingRedirector {
        
        [Category("Tool HUD Settings")]
        
        [Value(true)]
        [Name("Do tenth slot", "Should show the tenth slot of your hotbar. If false, you would get tools from your inventory as normal")]
        public partial bool Do10ThSlot { get; set; }
        
        [Value(true)]
        [Name("Show tool names", "Should show the names of the tools when selecting them. This is redundant if you do not have the tenth slot")]
        public partial bool ShowToolNames { get; set; }
        
        [Air(5)]
        
        
        [Value(0.63794f)]
        [MinMax(0f, 1f)]
        [Name("Tenth slot x", "The X position of the tenth slot. Use configure to adjust.")]
        public partial float TenthSlotX { get; set; }
        
        [Value(0.9200f)]
        [MinMax(0f, 1f)]
        [Name("Tenth slot y", "The Y position of the tenth slot. Use configure to adjust.")]
        public partial float TenthSlotY { get; set; }
        
        [Value(0.03120f)]
        [MinMax(0f, 1f)]
        [Name("Tenth slot size", "The size the tenth slot. Use configure to adjust.")]
        public partial float TenthSlotSize { get; set; }
        
        
        void Configure() {
            Console.WriteLine("Button pressed");
            ToolManager.Configure();
        }
        [Button(nameof(Configure), "Configure")]
        [Name("Adjust tenth slot position", "Once pressed, you can use the arrow keys to change the position of the tenth slot and pg up and pg down to change the size. Once finished, press enter to save you changes or escape to cancel")]
        public partial OnixSetting.SettingChangedDelegate ConfigureButtonPressed { get; set; }
        
        [Air(5)]
        
        
        [Value(InputKey.Type.Num0)]
        [Name("10th slot key", "The key to select the 10th slot of your inventory.")]
        public partial InputKey HotbarKey10 { get; set; } 
        
        [Value(InputKey.Type.Num1)]
        [Name("1st slot key", "The key to select the 1st slot of your inventory. Just mirror what is in your settings")]
        public partial InputKey HotbarKey1 { get; set; } 
        
        [Value(InputKey.Type.Num2)]
        [Name("2nd slot key", "The key to select the 2nd slot of your inventory. Just mirror what is in your settings")]
        public partial InputKey HotbarKey2 { get; set; } 
        
        [Value(InputKey.Type.Num3)]
        [Name("3rd slot key", "The key to select the 3rd slot of your inventory. Just mirror what is in your settings")]
        public partial InputKey HotbarKey3 { get; set; } 
        
        [Value(InputKey.Type.Num4)]
        [Name("4th slot key", "The key to select the 4th slot of your inventory. Just mirror what is in your settings")]
        public partial InputKey HotbarKey4 { get; set; } 
        
        [Value(InputKey.Type.Num5)]
        [Name("5th slot key", "The key to select the 5th slot of your inventory. Just mirror what is in your settings")]
        public partial InputKey HotbarKey5 { get; set; } 
        
        [Value(InputKey.Type.Num6)]
        [Name("6th slot key", "The key to select the 6th slot of your inventory. Just mirror what is in your settings")]
        public partial InputKey HotbarKey6 { get; set; } 
        
        [Value(InputKey.Type.Num7)]
        [Name("7th slot key", "The key to select the 7th slot of your inventory. Just mirror what is in your settings")]
        public partial InputKey HotbarKey7 { get; set; } 
        
        [Value(InputKey.Type.Num8)]
        [Name("8th slot key", "The key to select the 8th slot of your inventory. Just mirror what is in your settings")]
        public partial InputKey HotbarKey8 { get; set; } 
        
        
        [Value(InputKey.Type.Num9)]
        [Name("9th slot key", "The key to select the 9th slot of your inventory. Just mirror what is in your settings")]
        public partial InputKey HotbarKey9 { get; set; } 
        
    }
}