using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotSync.Domain.PartyAggregate;

namespace SpotSync.Application.Services
{
    public class DiagnosticsService : IDiagnosticsService
    {
        private readonly IPartyService _partyService;

        public DiagnosticsService(IPartyService partyService)
        {
            _partyService = partyService;
        }

        public async Task<PartyDiagnostics> GetPartyDiagnosticAsync(string partyCode)
        {
            Party party = await _partyService.GetPartyWithCodeAsync(partyCode);

            return party.GetDiagnostics();
        }

        public async Task<List<PartyDiagnostics>> GetPartyDiagnosticsAsync()
        {
            List<Party> parties = await _partyService.GetAllPartiesAsync();

            return parties.Select(party => party.GetDiagnostics()).ToList();
        }
    }
}
