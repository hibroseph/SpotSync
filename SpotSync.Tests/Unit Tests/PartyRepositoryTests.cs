using NUnit.Framework;
using Persistence;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Tests.Unit_Tests
{
    [TestFixture]
    class PartyRepositoryTests
    {
        public IPartyRepository PartyRepository;
        public const string PARTIER_NAME = "Kip";
        public const string PARTIER_NAME_1 = "Kip420";

        [SetUp]
        public void Initalize()
        {
            PartyRepository = new PartyRepository();
        }

        [Test]
        public void CannotCreateTwoPartiesWithSameCode_ThrowsException()
        {

            PartyGoer partyGoer = new PartyGoer(PARTIER_NAME);

            Party party = new Party(partyGoer);

            PartyRepository.CreateParty(party);

            Assert.Throws<Exception>(() => PartyRepository.CreateParty(party));
        }

        [Test]
        public void CannotUpdatePartyThatDoesntExist_ThrowsException()
        {
            PartyGoer partyGoer = new PartyGoer(PARTIER_NAME);

            Party party = new Party(partyGoer);

            Assert.Throws<Exception>(() => PartyRepository.UpdateParty(party));
        }

        [Test]
        public async Task HostIsListeningInParty()
        {
            PartyGoer partyGoer = new PartyGoer(PARTIER_NAME);

            Party party = new Party(partyGoer);

            PartyRepository.CreateParty(party);

            Assert.IsTrue(await PartyRepository.IsUserInAPartyAsync(partyGoer));
        }

        [Test]
        public async Task CanGetPartyWithHost()
        {
            PartyGoer partyGoer = new PartyGoer(PARTIER_NAME);
            PartyGoer partyGoer1 = new PartyGoer(PARTIER_NAME_1);

            Party party = new Party(partyGoer);
            Party party1 = new Party(partyGoer1);

            PartyRepository.CreateParty(party);
            PartyRepository.CreateParty(party1);

            Assert.AreEqual(party, await PartyRepository.GetPartyWithHostAsync(partyGoer));
            Assert.AreEqual(party1, await PartyRepository.GetPartyWithHostAsync(partyGoer1));
        }


    }
}
