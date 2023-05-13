using System;
using System.Text.Json.Serialization;

// for <PublishAot>true</PublishAot>, but does not work:
[JsonSerializable(typeof(Group[]))]
internal partial class SourceGenerationContext : JsonSerializerContext { }
public class Group
{
    public string tags { get; set; }
    public string host { get; set; }
    public string[] paths { get; set; }
    public Snapshot[] keep { get; set; }
    public Snapshot[] remove { get; set; }
    public Reason[] reasons { get; set; }
    public char Symbol { get; set; } // added afterwards
}
public class Snapshot
{
    public DateTime time { get; set; }
    public string parent { get; set; }
    public string tree { get; set; }
    public string[] paths { get; set; }
    public string hostname { get; set; }
    public string username { get; set; }
    public string id { get; set; }
    public string short_id { get; set; }
    public string[] excludes { get; set; }
    public string original { get; set; }
    public char GroupSymbol { get; set; } // added afterwards
}
public class Reason
{
    public Snapshot snapshot { get; set; }
    public string[] matches { get; set; }
    // public Counters counters { get; set; }
}
// public class Counters
// {
// }