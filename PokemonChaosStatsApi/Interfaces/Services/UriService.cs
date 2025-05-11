using Microsoft.AspNetCore.WebUtilities;

public class UriService : IUriService
{
    private readonly string _baseUri;
    public UriService(String baseUri)
    {
        _baseUri = baseUri;
    }
    public Uri GetPageUri(PaginationFilter filter, string route)
    {
        var _endpointUri = new Uri(string.Concat(_baseUri,route));
        var modifiedUri = QueryHelpers.AddQueryString(_endpointUri.ToString(),"pageNumber",filter.PageNumber.ToString());
        modifiedUri = QueryHelpers.AddQueryString(modifiedUri,"pageSize",filter.PageSize.ToString());
        return new Uri(modifiedUri);
    }
}