using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.DTO
{
    public class SpotifySongQueryResult : SpotifyQueryResult
    {
        public string Artist { get; set; }
        public int Length { get; set; }
    }
}
