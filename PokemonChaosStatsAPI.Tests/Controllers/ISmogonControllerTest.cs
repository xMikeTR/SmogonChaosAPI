public interface ISmogonControllerService
{
    IActionResult GetAvailableDates();
    IActionResult GetAvailableFormats(string date);
    Task<IActionResult> GetFromJsonAsync(string date, string format, [FromQuery] PaginationFilter filter);
    Task<IActionResult> GetFromJsonAsync(string date, string format,string selected);

}