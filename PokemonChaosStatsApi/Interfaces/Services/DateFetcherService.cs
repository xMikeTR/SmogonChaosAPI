//Implementation of DateFetcher Service, using HtmlWeb to parse the dates

using HtmlAgilityPack;

public class DateFetcherService : IDateFetcherService
{
    public List<string> GetAvailableDates()
    {
        var dates = new List<string>();
        var web = new HtmlWeb();
        var doc = web.Load("https://www.smogon.com/stats");

        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//a"))
        {
            dates.Add(node.InnerHtml);
        }

        return dates.Skip(1).ToList();
    }
}