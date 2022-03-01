using Ardalis.GuardClauses;
using AutoFixture;
using System;
using System.Collections.Generic;
using Xunit;

namespace Zombies.Domain.Tests
{
    public class SurvivorShould
    {
        public SurvivorShould()
        {
        }

        [Theory]
        [InlineData("John")]
        [InlineData("Martin")]
        public void HaveAName(string name)
        {
            var s = SurvivorProvider.CreateRandomSurvivor(name);

            Assert.Equal(name, s.Name);
        }

        [Theory]
        [InlineData("", typeof(ArgumentException))]
        [InlineData((string?)null, typeof(ArgumentNullException))]
        public void ThrowErrorOnEmptyNames(string name, Type expectedException)
        {
            Assert.Throws(expectedException, () => new Survivor(name));
        }

        [Fact]
        public void BeCreatedWithZeroWounds()
        {
            var s = SurvivorProvider.CreateRandomSurvivor();

            Assert.Equal(0, s.Wounds);
        }

        [Fact]
        public void BeCreatedWithAliveStatus()
        {
            var s = SurvivorProvider.CreateRandomSurvivor();

            Assert.True(s.IsAlive);
        }

        [Fact]
        public void BeCreatedWithAbilityToPerformUpToThreeActionsPerTurn()
        {
            var s = SurvivorProvider.CreateRandomSurvivor();

            Assert.Equal(3, s.AvailableActionsInTurn);
        }

        [Fact]
        public void BeCreatedWithNoEquipment()
        {
            var s = SurvivorProvider.CreateRandomSurvivor();

            Assert.Equal(0, s.InHand);
            Assert.Equal(0, s.InReserve);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(10)]
        public void HaveUpToFiveEquipmentItems(int equipmentToAdd)
        {
            var s = SurvivorProvider.CreateRandomSurvivor();

            for (int i = 0; i < equipmentToAdd; i++)
            {
                var e = new Fixture().Create<string>();
                s.AddEquipment(e);
            }

            Assert.Equal(5, s.InHand + s.InReserve);
        }

        [Theory]
        [InlineData("", typeof(ArgumentException))]
        [InlineData((string?)null, typeof(ArgumentNullException))]
        public void OnlyAddEquipmentWhenNameIsNotNorrOrEmpty(string equipmentName, Type exceptionType)
        {
            var s = SurvivorProvider.CreateRandomSurvivor();

            Assert.Throws(exceptionType, () => s.AddEquipment(equipmentName));
        }


        [Fact]
        public void AddEquipmentToHandsFirstAndThenToReserve()
        {
            var s = SurvivorProvider.CreateRandomSurvivor();

            var equipmentToAdd = new Fixture().CreateMany<string>(5);

            var i = 0;
            foreach (var equipment in equipmentToAdd)
            {
                i++;
                s.AddEquipment(equipment);

                if (i <= 2)
                {
                    Assert.Equal(i, s.InHand);
                    Assert.Equal(0, s.InReserve);
                }
                if (i > 2)
                {
                    Assert.Equal(i-2, s.InReserve);
                    Assert.Equal(2, s.InHand);
                }

            }

            Assert.Equal(2, s.InHand);
            Assert.Equal(3, s.InReserve);


        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(10, false)]
        public void DieWhenAtLeastTwoWoundsAreReceived(int woundsToInflict, bool shouldBeAlive)
        {
            var name = new Fixture().Create<string>();

            var s = new Survivor(name);

            for (int i = 0; i < woundsToInflict; i++)
            {
                s.Wound();
            }

            Assert.Equal(shouldBeAlive, s.IsAlive);
        }
    }

    internal class Survivor
    {
        private List<string> equipmentInHand;

        private List<string> equipmentInReserve;

        public Survivor(string name)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));

            Name = name;
            Wounds = 0;
            AvailableActionsInTurn = 3;
            equipmentInHand = new List<string>();
            equipmentInReserve = new List<string>();
        }

        public string Name { get; }

        public int Wounds { get; private set; }

        public int AvailableActionsInTurn { get; }

        public int InHand => equipmentInHand.Count;

        public int InReserve => equipmentInReserve.Count;

        public bool IsAlive => Wounds < 2;

        internal void AddEquipment(string equipmentName)
        {
            Guard.Against.NullOrEmpty(equipmentName, nameof(equipmentName));
            
            if (InHand == 2 && InReserve < 3)
            {
                equipmentInReserve.Add(equipmentName);
            }

            if (InHand < 2)
            {
                equipmentInHand.Add(equipmentName);
            }

        }

        internal void Wound()
        {
            Wounds++;
        }
    }
}