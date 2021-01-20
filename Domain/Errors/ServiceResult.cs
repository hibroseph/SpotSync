using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SpotSync.Domain.Errors
{
    public class ServiceResult<T>
    {
        public ServiceResult() { Errors = new List<T>(); }
        public bool Success => Errors.Count == 0;
        public List<T> Errors { get; set; }

        public void AddError(T error)
        {
            Errors.Add(error);
        }
    }
}
