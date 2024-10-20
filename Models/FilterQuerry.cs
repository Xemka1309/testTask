namespace TestTask.Models;

public record FilterQuerry(DateTime DateTime, string Prefix)
{
    public static readonly HashSet<string> Prefixes = new(){
        "eq", "ne", "gt", "lt", "ge", "le", "sa", "eb", "ap"
    };
    public static bool IsValidPrefix(string value)
    {
        return Prefixes.Contains(value);
    }
}