namespace AspnetCoreBase.Models
{
    public class ApiResponseModel
    {
        public object Data { get; set; }
        public int StatusCode { get; set; } = 200;
        public string Message { get; set; } = "";
    }
}