//URI interface used in the HTTP requests
public interface IUriService
{
    public Uri GetPageUri(PaginationFilter filter, string route);
}