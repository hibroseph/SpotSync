using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Classes.Responses.Common
{
    public class Result<T>
    {
        public Result(bool succeeded, string message, T result)
        {
            Succeeded = succeeded;
            Message = message;
            Content = result;
        }

        public Result(T result)
        {
            Succeeded = true;
            Content = result;
        }

        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public T Content { get; set; }
    }

    public class Result
    {
        public Result(bool succeeded, string message)
        {
            Succeeded = succeeded;
            Message = message;
        }

        public bool Succeeded { get; set; }
        public string Message { get; set; }
    }
}
