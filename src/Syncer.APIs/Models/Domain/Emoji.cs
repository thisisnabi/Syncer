
namespace Syncer.APIs.Models.Domain;

public class Emoji
{
    public required string Code { get; set; }
    public required string ShortName { get; set; }

    public static Emoji Create(string code, string name)
    {
        var unicode = char.ConvertToUtf32(code, 0);
        var hexadecimal = $"U+{unicode:X4}";
        return new Emoji { Code = hexadecimal, ShortName = name };
    }
}
