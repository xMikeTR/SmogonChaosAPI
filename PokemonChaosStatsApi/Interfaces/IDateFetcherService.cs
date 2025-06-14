//Interface to grab avaialble dates on Chaos page of Smogon
public interface IDateFetcherService
{
    List<string> GetAvailableDates();
}