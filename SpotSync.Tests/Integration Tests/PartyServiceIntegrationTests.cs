using Moq;
using NUnit.Framework;
using Persistence;
using SpotSync.Application.Services;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.Contracts.SpotifyApi;
using SpotSync.Domain.Contracts.SpotifyApi.Models;
using SpotSync.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpotSync.Domain.PartyAggregate;

namespace SpotSync.Tests
{
    class PartyServiceIntegrationTests
    {
        IPartyService _partyService;
        IPartyRepository _partyRepository;
        Mock<ISpotifyHttpClient> _spotifyHttpClient;
        Mock<ILogService> _logService;
        Mock<IPartyGoerService> _partyGoerService;

        PartyGoer PartyHost1 = new PartyGoer("Joe", false, "US", "premium");
        PartyGoer PartyHost2 = new PartyGoer("Kip", false, "US", "premium");
        PartyGoer PartyHost3 = new PartyGoer("Kipseph", false, "US", "premium");
        PartyGoer PartyAttendee1 = new PartyGoer("John", false, "US", "premium");
        PartyGoer PartyAttendee2 = new PartyGoer("Matt", false, "US", "premium");
        PartyGoer PartyAttendee3 = new PartyGoer("Joseph", false, "US", "premium");

        [SetUp]
        public void SetUpPartyService()
        {
            _spotifyHttpClient = new Mock<ISpotifyHttpClient>();
            _partyRepository = new PartyRepository();
            _logService = new Mock<ILogService>();
            _partyGoerService = new Mock<IPartyGoerService>();

            _spotifyHttpClient.Setup(p => p.RequestAccessAndRefreshTokenFromSpotifyAsync(It.IsAny<string>())).Returns(Task.FromResult(new User { SpotifyId = It.IsAny<string>() }));

            _partyService = new PartyService(_partyRepository, _spotifyHttpClient.Object, _logService.Object, _partyGoerService.Object);
        }

        [Test]
        [Ignore("Test needs to be rewritten")]
        public async Task NewPartyWithHostHasNoAttendeesEmpty()
        {

            string partyCode = await _partyService.StartNewPartyAsync(PartyHost1);

            PartyCodeDTO partyCodeDTO = new PartyCodeDTO { PartyCode = partyCode };
            Assert.AreEqual(0, (await _partyService.GetPartyWithHostAsync(PartyHost1)).GetListenerCount());
        }

        [Test]
        [Ignore("Test needs to be rewritten")]
        public async Task SinglePartyHasCorrectNumberOfAttendees()
        {
            string partyCode = await _partyService.StartNewPartyAsync(PartyHost1);

            PartyCodeDTO partyCodeDTO = new PartyCodeDTO { PartyCode = partyCode };

            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee3);

            Assert.AreEqual(3, (await _partyService.GetPartyWithHostAsync(PartyHost1)).GetListenerCount());
        }

        [Test]
        [Ignore("Test needs to be rewritten")]
        public async Task SinglePartyHasCorrectNumberOfAttendeesWithLeavingAttendees()
        {

            string partyCode = await _partyService.StartNewPartyAsync(PartyHost1);

            PartyCodeDTO partyCodeDTO = new PartyCodeDTO { PartyCode = partyCode };

            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee3);

            await _partyService.LeavePartyAsync(PartyAttendee2);

            Assert.AreEqual(2, (await _partyService.GetPartyWithHostAsync(PartyHost1)).GetListenerCount());
        }

        [Test]
        [Ignore("Test needs to be rewritten")]
        public async Task SinglePartyHasCorrectNumberOfAttendeesWithMultipleLeavingAttendees()
        {

            string partyCode = await _partyService.StartNewPartyAsync(PartyHost1);

            PartyCodeDTO partyCodeDTO = new PartyCodeDTO { PartyCode = partyCode };

            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee3);

            await _partyService.LeavePartyAsync(PartyAttendee2);
            await _partyService.LeavePartyAsync(PartyAttendee3);

            Assert.AreEqual(1, (await _partyService.GetPartyWithHostAsync(PartyHost1)).GetListenerCount());
        }

        [Test]
        [Ignore("Test needs to be rewritten")]
        public async Task SinglePartyHasNoAttendeesWithAllLeavingAttendees()
        {

            string partyCode = await _partyService.StartNewPartyAsync(PartyHost1);

            PartyCodeDTO partyCodeDTO = new PartyCodeDTO { PartyCode = partyCode };

            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee3);

            await _partyService.LeavePartyAsync(PartyAttendee2);
            await _partyService.LeavePartyAsync(PartyAttendee3);
            await _partyService.LeavePartyAsync(PartyAttendee1);

            Assert.AreEqual(0, (await _partyService.GetPartyWithHostAsync(PartyHost1)).GetListenerCount());
        }

        [Test]
        [Ignore("Test needs to be rewritten")]
        public async Task MultiplePartyHasCorrectNumberAttendees()
        {

            string partyCode1 = await _partyService.StartNewPartyAsync(PartyHost1);

            PartyCodeDTO partyCodeDTO1 = new PartyCodeDTO { PartyCode = partyCode1 };

            string partyCode3 = await _partyService.StartNewPartyAsync(PartyHost2);

            PartyCodeDTO partyCodeDTO3 = new PartyCodeDTO { PartyCode = partyCode3 };

            string partyCode2 = await _partyService.StartNewPartyAsync(PartyHost3);

            PartyCodeDTO partyCodeDTO2 = new PartyCodeDTO { PartyCode = partyCode2 };

            await _partyService.JoinPartyAsync(partyCodeDTO1, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO2, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO3, PartyAttendee3);

            Assert.AreEqual(2, (await _partyService.GetPartyWithHostAsync(PartyHost1)).GetListenerCount());
            Assert.AreEqual(2, (await _partyService.GetPartyWithHostAsync(PartyHost2)).GetListenerCount());
            Assert.AreEqual(2, (await _partyService.GetPartyWithHostAsync(PartyHost3)).GetListenerCount());
        }

        [Test]
        [Ignore("Test needs to be rewritten")]
        public async Task MultiplePartyHasCorrectNumberAttendees_Leaving()
        {

            string partyCode1 = await _partyService.StartNewPartyAsync(PartyHost1);

            PartyCodeDTO partyCodeDTO1 = new PartyCodeDTO { PartyCode = partyCode1 };

            string partyCode2 = await _partyService.StartNewPartyAsync(PartyHost2);

            PartyCodeDTO partyCodeDTO2 = new PartyCodeDTO { PartyCode = partyCode2 };

            string partyCode3 = await _partyService.StartNewPartyAsync(PartyHost3);

            PartyCodeDTO partyCodeDTO3 = new PartyCodeDTO { PartyCode = partyCode3 };


            await _partyService.JoinPartyAsync(partyCodeDTO1, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO2, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO3, PartyAttendee3);

            await _partyService.LeavePartyAsync(PartyAttendee1);
            await _partyService.LeavePartyAsync(PartyAttendee2);

            Assert.AreEqual(0, (await _partyService.GetPartyWithHostAsync(PartyHost1)).GetListenerCount());
            Assert.AreEqual(0, (await _partyService.GetPartyWithHostAsync(PartyHost2)).GetListenerCount());
            Assert.AreEqual(1, (await _partyService.GetPartyWithHostAsync(PartyHost3)).GetListenerCount());
        }

        [Ignore("Test needs to be rewritten")]
        [Test]
        public async Task PartyGoerAttendingFindsPartyWithAttendee()
        {

            string partyCode1 = await _partyService.StartNewPartyAsync(PartyHost1);

            PartyCodeDTO partyCodeDTO1 = new PartyCodeDTO { PartyCode = partyCode1 };

            string partyCode3 = await _partyService.StartNewPartyAsync(PartyHost2);

            PartyCodeDTO partyCodeDTO3 = new PartyCodeDTO { PartyCode = partyCode3 };

            string partyCode2 = await _partyService.StartNewPartyAsync(PartyHost3);

            PartyCodeDTO partyCodeDTO2 = new PartyCodeDTO { PartyCode = partyCode2 };

            await _partyService.JoinPartyAsync(partyCodeDTO1, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO2, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO3, PartyAttendee3);

            Party party = await _partyService.GetPartyWithAttendeeAsync(PartyAttendee3);

            Assert.AreEqual(partyCodeDTO3.PartyCode, party.GetPartyCode());
        }
    }
}
