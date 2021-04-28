using NUnit.Framework;
using SpotSync.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Tests.Unit_Tests
{
    public class ExtensionTests
    {
        [Test]
        public void NullList_IsNullOrEmpty_ReturnsTrue()
        {
            List<string> list = null;

            Assert.IsTrue(list.IsNullOrEmpty());
        }


        [Test]
        public void EmptyList_IsNullOrEmpty_ReturnsTrue()
        {
            List<string> list = new List<string>();

            Assert.IsTrue(list.IsNullOrEmpty());
        }


        [Test]
        public void NonEmptyList_IsNullOrEmpty_ReturnsFalse()
        {
            List<string> list = new List<string>() { "kip" };

            Assert.IsFalse(list.IsNullOrEmpty());
        }
    }
}
