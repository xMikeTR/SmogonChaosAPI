public class SmogonControllerTest : ISmogonControllerService
{
    public GetAvailableDates() => throw new NotImplementedException();
    public GetAvailableFormats(string date) => throw new NotImplementedException();
    public GetFromJsonAsync(string date, string format, [FromQuery] PaginationFilter filter) => throw new NotImplementedException();
    public GetFromJsonAsync(string date, string format,string selected) => throw new NotImplementedException();
}