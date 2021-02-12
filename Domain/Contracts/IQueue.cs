using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts
{
    interface ITrackQueue
    {
        int Count { get; }
        Track First { get; }
    }
}
