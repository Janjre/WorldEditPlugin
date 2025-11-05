using OnixRuntime.Api.Entities;
using OnixRuntime.Api.OnixClient.Commands;

namespace PluginCommandExample {
    
    public class ExampleCommand : OnixCommandBase {
        public ExampleCommand() : base("example", "A simple example command to show you how they work.", CommandExecutionTarget.Client, CommandPermissionLevel.Any) { }

        [Overload]
        OnixCommandOutput Example() {
            return Success("Hello world!");
        }
    }
}