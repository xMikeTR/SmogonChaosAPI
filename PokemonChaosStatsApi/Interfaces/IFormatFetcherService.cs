//Interface to grab the available formats based on the date
public interface IFormatFetcherService
{
    List<string> GetFormats(string date);
}