using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Contracts
{
    public interface IAuthenticationService
    {
        Task<PartyGoerDetails> AuthenticateUserWithAccessCodeAsync(string code);
        Task LogOutUserAsync(string userId);
    }
}
