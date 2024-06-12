using System.Collections.Generic;

namespace Scripts_v2.Data.Responses
{
    public class Response<T>
    {
        public bool isSuccess { get; }
        public T data { get; }
        public string message { get; }
        public List<string> errors { get; }

        private Response(bool isSuccess, T data, string message, List<string> errors)
        {
            this.isSuccess = isSuccess;
            this.data = data;
            this.message = message;
            this.errors = errors;
        }

        public static Response<T> Success(T data, string message = null) => new(true, data, message, null);

        public static Response<T> Failure(List<string> errors, string message = null) => new(false, default, message, errors);

        public static Response<T> Failure(string error, string message = null) => new(false, default, message, new List<string> { error });
    }
}