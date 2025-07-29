using Microsoft.AspNetCore.Http;

namespace Business.Results
{


    public class OperationResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public int StatusCode { get; set; } = StatusCodes.Status200OK;

        public static OperationResult<T> Ok(T data) => new OperationResult<T>() { Success = true, Data = data };
        public static OperationResult<T> Fail(string error, int statusCode) => new OperationResult<T>()
        {
            Success = false,
            ErrorMessage = error,
            StatusCode = statusCode
        };

        public static OperationResult<T> NotFound(string error) => Fail(error, StatusCodes.Status404NotFound);
        public static OperationResult<T> BadRequest(string error) => Fail(error, StatusCodes.Status400BadRequest);
        public static OperationResult<T> Conflict(string error) => Fail(error, StatusCodes.Status409Conflict);
    }

}
