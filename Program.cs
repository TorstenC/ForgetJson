using System;
using System.Linq;
using System.CommandLine;
using System.CommandLine.Help;
using System.Threading.Tasks;

namespace ForgetJson;
partial class Program
{   // TODO It's better to avoid static member.
    // This can make the code easier to test and maintain in the long run.
    static async Task<int> Main(string[] args)
    {
        int returnCode = 0;
        listForgetCommand.AddAlias("lf");
        rootCommand.Description += $"{string.Join(", ",
            rootCommand.Subcommands.Select(o => string.Join(", ", o.Aliases)))}";
        // rootCommand.SetHandler(() => {
        // // TODO Execute default command here? Or print help?
        // });
        listForgetCommand.SetHandler((isGroupOpionSet) => ListForgetAsync(isGroupOpionSet), groupOption);
        returnCode = await rootCommand.InvokeAsync(args);
        if (returnCode != 0)
            Console.WriteLine($"Exited with Code {returnCode} ({(ReturnCode)returnCode}).");
        return returnCode;
    }
    static RootCommand rootCommand = new RootCommand(
        description:
            "SnapSelect helps users select which files to include in snapshots for backups."  + Environment.NewLine +
            // "See https://bit.ly/43iaDTx"  + Environment.NewLine +
            "Use one of these commands: ")
        { listForgetCommand };
}
