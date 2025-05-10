public class ApiResponse<T>
{
    public ApiResponse()
    {

    }
    public T Data {get;set;}
    public bool Succeeded { get; set; }
    public string Message { get; set; }
    public string[] Errors {get;set;}

    public ApiResponse(T data)
    {
        Succeeded = true;
        Message = string.Empty;
        Data = data;
        Errors = null;
    }
}
