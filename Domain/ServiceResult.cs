using SpotSync.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain
{
    public class ServiceResult<T>
    {
        public T Result { get; set; }
        public ErrorType? Error { get; set; }

        public bool IsSuccessful()
        {
            return Error == null;
        }
    }
}
