namespace APIERP.ViewModels
{
    public class ResponsePostView
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public ResponsePostView(string Message, int StatusCode)
        {
            this.Message = Message;
            this.StatusCode = StatusCode;
        }

        public ResponsePostView() { }
    }

    public class Response
    {
        public string Message { get; set; }

        public object Data { get; set; }

        public string ErrorCode { get; set; }

        public bool Success { get; set; }

        public Response(string message, object data, string errorcode, bool success)
        {
            Message = message;
            Data = data;
            ErrorCode = errorcode;
            Success = success;
        }
    }
}
