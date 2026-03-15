using System.Text.RegularExpressions;
using StreamArch.Core.Models;

namespace StreamArch.Core.Services;

public static class M3uParser
{
    private static readonly Regex AttributeRegex = new("(?<key>[\\w-]+)=\"(?<value>[^\"]*)\"", RegexOptions.Compiled);

    public static List<Channel> Parse(string content)
    {
        var result = new List<Channel>();
        string[] lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        ChannelBuilder? currentBuilder = null;

        foreach (var line in lines)
        {
            string text = line.Trim();

            if (text.StartsWith("#EXTINF"))
            {
                currentBuilder = new ChannelBuilder();
                
                int lastCommaIndex = text.LastIndexOf(',');
                
                currentBuilder.Name = lastCommaIndex > -1 
                    ? text[(lastCommaIndex + 1)..].Trim() 
                    : "Sem Nome";
                
                MatchCollection matches = AttributeRegex.Matches(text);
                foreach (Match match in matches)
                {
                    var val = match.Groups["value"].Value;
                    switch (match.Groups["key"].Value)
                    {
                        case "group-title":
                            currentBuilder.Group = val;
                            break;
                        case "tvg-logo":
                            currentBuilder.Logo = val;
                            break;
                    }
                }
            }
            else if (!text.StartsWith('#') && currentBuilder != null)
            {
                result.Add(new Channel(
                    currentBuilder.Name, 
                    text, 
                    currentBuilder.Group, 
                    currentBuilder.Logo
                ));
                currentBuilder = null;
            }
        }
        return result;
    }
    
    private class ChannelBuilder
    {
        public string Name { get; set; } = string.Empty;
        public string? Group { get; set; }
        public string? Logo { get; set; }
    }
}