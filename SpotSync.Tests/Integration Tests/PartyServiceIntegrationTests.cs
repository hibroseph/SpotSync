using Moq;
using NUnit.Framework;
using Persistence;
using SpotSync.Application.Services;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Tests
{
    class PartyServiceIntegrationTests
    {
        IPartyService _partyService;
        IPartyRepository _partyRepository;
        Mock<ISpotifyHttpClient> _spotifyHttpClient;

        PartyGoer PartyHost1 = new PartyGoer("Joe");
        PartyGoer PartyHost2 = new PartyGoer("Kip");
        PartyGoer PartyHost3 = new PartyGoer("Kipseph");
        PartyGoer PartyAttendee1 = new PartyGoer("John");
        PartyGoer PartyAttendee2 = new PartyGoer("Matt");
        PartyGoer PartyAttendee3 = new PartyGoer("Joseph");

        [SetUp]
        public void SetUpPartyService()
        {
            _spotifyHttpClient = new Mock<ISpotifyHttpClient>();
            _partyRepository = new PartyRepository();
            _partyService = new PartyService(_partyRepository, _spotifyHttpClient.Object);
        }

        [Test]
        public async Task NewPartyWithHostHasNoAttendeesEmpty()
        {

            string partyCode = _partyService.StartNewParty(PartyHost1);

            PartyCodeDTO partyCodeDTO = new PartyCodeDTO { PartyCode = partyCode };
            Assert.AreEqual(0, (await _partyService.GetPartyWithHostAsync(PartyHost1)).Attendees.Count);
        }

        [Test]
        public async Task SinglePartyHasCorrectNumberOfAttendees()
        {
            string partyCode = _partyService.StartNewParty(PartyHost1);

            PartyCodeDTO partyCodeDTO = new PartyCodeDTO { PartyCode = partyCode };

            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee3);

            Assert.AreEqual(3, (await _partyService.GetPartyWithHostAsync(PartyHost1)).Attendees.Count);
        }

        [Test]
        public async Task SinglePartyHasCorrectNumberOfAttendeesWithLeavingAttendees()
        {

            string partyCode = _partyService.StartNewParty(PartyHost1);

            PartyCodeDTO partyCodeDTO = new PartyCodeDTO { PartyCode = partyCode };

            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee3);

            await _partyService.LeavePartyAsync(PartyAttendee2);

            Assert.AreEqual(2, (await _partyService.GetPartyWithHostAsync(PartyHost1)).Attendees.Count);
        }

        [Test]
        public async Task SinglePartyHasCorrectNumberOfAttendeesWithMultipleLeavingAttendees()
        {

            string partyCode = _partyService.StartNewParty(PartyHost1);

            PartyCodeDTO partyCodeDTO = new PartyCodeDTO { PartyCode = partyCode };

            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee3);

            await _partyService.LeavePartyAsync(PartyAttendee2);
            await _partyService.LeavePartyAsync(PartyAttendee3);

            Assert.AreEqual(1, (await _partyService.GetPartyWithHostAsync(PartyHost1)).Attendees.Count);
        }

        [Test]
        public async Task SinglePartyHasNoAttendeesWithAllLeavingAttendees()
        {

            string partyCode = _partyService.StartNewParty(PartyHost1);

            PartyCodeDTO partyCodeDTO = new PartyCodeDTO { PartyCode = partyCode };

            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO, PartyAttendee3);

            await _partyService.LeavePartyAsync(PartyAttendee2);
            await _partyService.LeavePartyAsync(PartyAttendee3);
            await _partyService.LeavePartyAsync(PartyAttendee1);

            Assert.AreEqual(0, (await _partyService.GetPartyWithHostAsync(PartyHost1)).Attendees.Count);
        }

        [Test]
        public async Task MultiplePartyHasCorrectNumberAttendees()
        {

            string partyCode1 = _partyService.StartNewParty(PartyHost1);

            PartyCodeDTO partyCodeDTO1 = new PartyCodeDTO { PartyCode = partyCode1 };

            string partyCode3 = _partyService.StartNewParty(PartyHost2);

            PartyCodeDTO partyCodeDTO3 = new PartyCodeDTO { PartyCode = partyCode3 };

            string partyCode2 = _partyService.StartNewParty(PartyHost3);

            PartyCodeDTO partyCodeDTO2 = new PartyCodeDTO { PartyCode = partyCode2 };

            await _partyService.JoinPartyAsync(partyCodeDTO1, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO2, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO3, PartyAttendee3);

            Assert.AreEqual(1, (await _partyService.GetPartyWithHostAsync(PartyHost1)).Attendees.Count);
            Assert.AreEqual(1, (await _partyService.GetPartyWithHostAsync(PartyHost2)).Attendees.Count);
            Assert.AreEqual(1, (await _partyService.GetPartyWithHostAsync(PartyHost3)).Attendees.Count);
        }

        [Test]
        public async Task MultiplePartyHasCorrectNumberAttendees_Leaving()
        {

            string partyCode1 = _partyService.StartNewParty(PartyHost1);

            PartyCodeDTO partyCodeDTO1 = new PartyCodeDTO { PartyCode = partyCode1 };

            string partyCode2 = _partyService.StartNewParty(PartyHost2);

            PartyCodeDTO partyCodeDTO2 = new PartyCodeDTO { PartyCode = partyCode2 };

            string partyCode3 = _partyService.StartNewParty(PartyHost3);

            PartyCodeDTO partyCodeDTO3 = new PartyCodeDTO { PartyCode = partyCode3 };


            await _partyService.JoinPartyAsync(partyCodeDTO1, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO2, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO3, PartyAttendee3);

            await _partyService.LeavePartyAsync(PartyAttendee1);
            await _partyService.LeavePartyAsync(PartyAttendee2);

            Assert.AreEqual(0, (await _partyService.GetPartyWithHostAsync(PartyHost1)).Attendees.Count);
            Assert.AreEqual(0, (await _partyService.GetPartyWithHostAsync(PartyHost2)).Attendees.Count);
            Assert.AreEqual(1, (await _partyService.GetPartyWithHostAsync(PartyHost3)).Attendees.Count);
        }

        [Test]
        public async Task PartyGoerAttendingFindsPartyWithAttendee()
        {

            string partyCode1 = _partyService.StartNewParty(PartyHost1);

            PartyCodeDTO partyCodeDTO1 = new PartyCodeDTO { PartyCode = partyCode1 };

            string partyCode3 = _partyService.StartNewParty(PartyHost2);

            PartyCodeDTO partyCodeDTO3 = new PartyCodeDTO { PartyCode = partyCode3 };

            string partyCode2 = _partyService.StartNewParty(PartyHost3);

            PartyCodeDTO partyCodeDTO2 = new PartyCodeDTO { PartyCode = partyCode2 };

            await _partyService.JoinPartyAsync(partyCodeDTO1, PartyAttendee1);
            await _partyService.JoinPartyAsync(partyCodeDTO2, PartyAttendee2);
            await _partyService.JoinPartyAsync(partyCodeDTO3, PartyAttendee3);

            Domain.Party party = await _partyService.GetPartyWithAttendeeAsync(PartyAttendee3);

            Assert.AreEqual(partyCodeDTO3.PartyCode, party.PartyCode);
        }


    }
}
