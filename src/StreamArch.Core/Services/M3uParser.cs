using System.Text.RegularExpressions;
using StreamArch.Core.Models;
using StreamArch.Core.Services.Commands;

namespace StreamArch.Core.Services;

public static partial class M3UParser
{
    private static readonly char[] LineSeparators = ['\r', '\n'];
    private static readonly Regex AttributeRegex = MyRegex();
    private const string InformationsTag = "#EXTINF";

    public static List<Channel> Parse(string content)
    {
        var result = new List<Channel>();
        string[] lines = content.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
        
        ChannelBuilder? currentBuilder = null;

        foreach (string line in lines)
        {
            string text = line.Trim();

            if (text.StartsWith(InformationsTag))
            {
                currentBuilder = new ChannelBuilder();
                
                int lastCommaIndex = text.LastIndexOf(',');
                
                currentBuilder.Name = lastCommaIndex > -1 
                    ? text[(lastCommaIndex + 1)..].Trim() 
                    : "Sem Nome";
                
                MatchCollection matches = AttributeRegex.Matches(text);
                
                foreach (Match match in matches)
                {
                    string val = match.Groups["value"].Value;
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

    [GeneratedRegex("(?<key>[\\w-]+)=\"(?<value>[^\"]*)\"", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}