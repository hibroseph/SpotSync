using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts.Services
{
    public interface IDiagnosticsService
    {
        Task<List<PartyDiagnostics>> GetPartyDiagnosticsAsync();
        Task<PartyDiagnostics> GetPartyDiagnosticAsync(string partyCode);
    }
}
