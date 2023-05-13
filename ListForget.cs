using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.CommandLine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForgetJson;
partial class Program {
    static Option<bool> groupOption = new Option<bool>(
        aliases: new string[] { "--group", "-g" },
        description:
            "This option is not yet implememted:" + Environment.NewLine +
            "Show one table per group, instead of a column for groups.");
    static Command listForgetCommand = new Command(
        name: "listforget",
        description:
            "The command `listforget` shows the JSON result from `restic forget` in a table like `restic snapshots`." + Environment.NewLine +
            "Usage: `restic forget --dry-run --json POLICIES | snapselect lf`")
            { groupOption };
    static async Task<int> ListForgetAsync(bool groupOptval)
    {
        // "If you return Task<int> from any of the Func* overloads, it will be used to set the exit code",
        // see https://github.com/dotnet/command-line-api/issues/1570#issuecomment-1170100340
        int returnCode = 0;
        if (groupOptval) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(groupOption.Description);
            Console.ResetColor();
            returnCode = (int)ReturnCode.OptionNotImplemented;
        }
        string input;
        // TODO catch error using `OneOf<string, Exception>`:
        input = (Console.IsInputRedirected)
            ? Console.In.ReadToEnd()
            : await File.ReadAllTextAsync(@"/home/toc/mnt/hosthome/projects/RepositoryCopy/forget-1.json");
        // TODO Check https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation#deserialization-examples
        // for <PublishAot>true</PublishAot>
        var sourceGenOptions = new JsonSerializerOptions { TypeInfoResolver = SourceGenerationContext.Default };
        Group[] groups = JsonSerializer.Deserialize<Group[]>(input, sourceGenOptions);
        // TODO catch error using `OneOf<string, Exception>` and print first lines/characters of `input`.
        Console.WriteLine($"Grouping into {groups.Count()} groups:");
        Console.WriteLine("G  Host        Path"); // TODO add Tags
        Console.WriteLine("--------------------------");
        char c = 'A';
        foreach (var item in groups) {
            item.Symbol = c++;
            // TODO multiline for paths[>0]:
            Console.WriteLine($"{item.Symbol}  {String.Format("{0,-10}", item.host)}  {item.paths[0]}");
            if (item.remove is null) item.remove = Array.Empty<Snapshot>();
            if (item.keep is null) item.keep = Array.Empty<Snapshot>();
        }
        var policies = new Dictionary<char,string>();
        c = '1';
        foreach (var item in groups.SelectMany(o => o.reasons.SelectMany(p => p.matches)).Distinct())
            policies.Add(c++, item);
        var reasons = new Dictionary<string,string>();
        foreach (var item in groups.SelectMany(o => o.reasons).OrderBy(p => p.snapshot.time)) {
            var policyMarker = string.Join(" ", policies.Select(i => (item.matches.Any(j => j == i.Value) ? i.Key.ToString() : "·")));
            reasons.Add(item.snapshot.tree, policyMarker);
        }
        Console.WriteLine();
        Console.WriteLine($"Applying {policies.Count} policies:");
        Console.WriteLine($"{string.Join(Environment.NewLine, policies.Select(policy => $"{policy.Key}  {policy.Value}"))}");
        Console.WriteLine();
        Console.WriteLine($"Keeping {reasons.Count} snapshots:");
        Console.WriteLine("ID        Original  Date       Time      G  Result  Reasons");
        Console.WriteLine("-----------------------------------------------------------");
        // TODO If more than 4 policies lengthen line^^.
        foreach (var snapshot in groups.SelectMany(item => item.keep.Concat(item.remove)
            .Select(o => { o.GroupSymbol = item.Symbol; return o; }))
            .OrderBy(i => i.time))
        {
            Console.Write($"{snapshot.short_id}  {snapshot.original?.Substring(0, 8) ?? "        "}  {snapshot.time.ToString("yyyy-MM-dd HH:mm:ss")}  {snapshot.GroupSymbol}");
            if (reasons.TryGetValue(snapshot.tree, out var policyMarker))
                Console.WriteLine($"  keep    {policyMarker}");
            else
                Console.WriteLine($"  remove");
        }
        return returnCode;
    }
}