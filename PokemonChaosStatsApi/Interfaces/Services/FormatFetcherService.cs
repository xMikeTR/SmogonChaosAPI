//Implementation of Format interface grabbing formats based on the Date provided, using WebGet

using HtmlAgilityPack;

public class FormatDateFetcherService : IFormatFetcherService
{
    public List<string> GetFormats(string date)
    {
        var Webget = new HtmlWeb();
        var doc =  Webget.Load($"https://www.smogon.com/stats/{date}/chaos");
        List<string> formats = new List<string>();

        foreach(HtmlNode node in doc.DocumentNode.SelectNodes("//a"))
        {
            formats.Add(node.ChildNodes[0].InnerHtml);
        }

        return formats.Skip(1).ToList();
    }            
}