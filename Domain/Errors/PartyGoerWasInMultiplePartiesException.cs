using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Errors
{
    public class PartyGoerWasInMultiplePartiesException : Exception
    {
        public PartyGoerWasInMultiplePartiesException(string message) : base(message) { }
    }
}
