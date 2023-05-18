using OneOf;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.CommandLine;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ForgetJson.OneOfException;

namespace ForgetJson;
partial class Program {
    static async Task<int> ListForgetAsync(bool isGroupOpionSet)
    {
        // "If you return Task<int> from any of the Func* overloads, it will be used to set the exit code",
        // see https://github.com/dotnet/command-line-api/issues/1570#issuecomment-1170100340
        int returnCode = 0;
        if (isGroupOpionSet)
            returnCode = WriteError(ReturnCode.OptionNotImplemented, groupOption.Description);
        var input2 = await GetInput();
        if (input2.IsT1) // Exception
            return (int)input2.AsT1.Code;
        var groups = input2.AsT0;
        Console.WriteLine($"Grouping into {groups.Count()} groups:");
        Console.WriteLine("G  Host          Tags        Paths"); // TODO add Tags
        Console.WriteLine("----------------------------------------");
        char c = 'A';
        foreach (var item in groups) {
            item.Symbol = c++;
            Console.Write($"{item.Symbol}  {String.Format("{0,-12}", item.host)}  "); // {item.paths[0]}
            var itemTagsLength = item.tags?.Length ?? 0;
            var lines = Math.Max(item.paths.Length, itemTagsLength);
            var tagsSpacer = new string(' ', 12);
            for (int line = 0; line < lines; line++)
            {
                Console.Write((line > 0) ? new String(' ', 17) : "");                   
                Console.Write((line < itemTagsLength) ? String.Format("{0,-12}", item.tags[line]) : tagsSpacer);
                Console.WriteLine((line < item.paths.Length) ? item.paths[line] : "" );
            }
            // TODO With String.Join the ^^`(line > 0) ?` would be omitted. - Better?
            // TODO Output via `Format-Table` (PowerShell) or `table` (Nushell) to adapt column widths
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
        Console.WriteLine(new string('-', 43 + reasons.First().Value.Length * 2));
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
        async Task<OneOf<Group[], OneOfException>> GetInput()
        {
            try
            {
                var jsonString = (Console.IsInputRedirected)
                    ? await Console.In.ReadToEndAsync()
                    : await File.ReadAllTextAsync(@"/home/toc/mnt/hosthome/projects/RepositoryCopy/forget-1.json");
                    // HACK Delete this^^ default input, it's for test only.
                var sourceGenOptions = new JsonSerializerOptions { TypeInfoResolver = SourceGenerationContext.Default };
                // TODO Check https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation#deserialization-examples
                // for <PublishAot>true</PublishAot>
                // Maybe with <Stream,TypeInfo>-Overload it's better? Will wrong path then be in Exception.Message?
                return JsonSerializer.Deserialize<Group[]>(jsonString, sourceGenOptions);
            }
            catch (JsonException ex)
            {
                return new OneOfException(ReturnCode.InvalidJson, ex,
                   ex.Message + Environment.NewLine +
                    "File: <Hier müsste der Dateiname stehen.>");
            }
            catch (FileNotFoundException ex)
            {
                return new OneOfException(ReturnCode.InputFileNotFound, ex,
                    ex.Message);
            }
            catch (Exception ex)
            {
                return new OneOfException(ReturnCode.UnforseenException, ex,
                    $"Unexpected [{ex.GetType().Name}]: \"{ex.Message}\"{Environment.NewLine}{ex.ToString().Split(Environment.NewLine).Last()})");
            }
        }
    }
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
}