using AutoFixture;
using System;
using Xunit;

namespace Zombies.Domain.Tests
{
    public class SurvivorShould
    {
        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 1)]
        public void GainOneExperiencePointWhenKillingAZombie(bool killZombie, int gainedExperience)
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            var initialExp = survivor.Experience;

            var z = new Zombie();

            if (killZombie)
                while (z.IsAlive)
                    survivor.Attack(z);

            Assert.Equal(gainedExperience, survivor.Experience);
        }

        [Theory]
        [InlineData(0, Level.Blue)]
        [InlineData(5, Level.Blue)]
        [InlineData(6, Level.Yellow)]
        [InlineData(17, Level.Yellow)]
        [InlineData(18, Level.Orange)]
        [InlineData(41, Level.Orange)]
        [InlineData(42, Level.Red)]
        public void LevelUp(int zombiesToKill, Level expectedLevel)
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            for (int i = 0; i < zombiesToKill; i++)
            {
                var z = new Zombie();

                while (z.IsAlive)
                    survivor.Attack(z);
            }

            Assert.Equal(zombiesToKill, survivor.Experience);
            Assert.Equal(expectedLevel, survivor.Level);
        }

        [Theory]
        [InlineData("John")]
        [InlineData("Martin")]
        public void HaveAName(string name)
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor(name);

            Assert.Equal(name, survivor.Name);
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
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            Assert.Equal(0, survivor.Wounds);
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
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            Assert.Equal(3, survivor.AvailableActionsInTurn);
        }

        [Fact]
        public void BeCreatedWithNoEquipment()
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            Assert.Equal(0, survivor.InHand);
            Assert.Equal(0, survivor.InReserve);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(10)]
        public void HaveUpToFiveEquipmentItems(int equipmentToAdd)
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            for (int i = 0; i < equipmentToAdd; i++)
            {
                var e = new Fixture().Create<string>();
                survivor.AddEquipment(e);
            }

            Assert.Equal(5, survivor.InHand + survivor.InReserve);
        }

        [Theory]
        [InlineData("", typeof(ArgumentException))]
        [InlineData((string?)null, typeof(ArgumentNullException))]
        public void OnlyAddEquipmentWhenNameIsNotNorrOrEmpty(string equipmentName, Type exceptionType)
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            Assert.Throws(exceptionType, () => survivor.AddEquipment(equipmentName));
        }

        [Fact]
        public void AddEquipmentToHandsFirstAndThenToReserve()
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            var equipmentToAdd = new Fixture().CreateMany<string>(5);

            var i = 0;
            foreach (var equipment in equipmentToAdd)
            {
                i++;
                survivor.AddEquipment(equipment);

                if (i <= 2)
                {
                    Assert.Equal(i, survivor.InHand);
                    Assert.Equal(0, survivor.InReserve);
                }
                if (i > 2)
                {
                    Assert.Equal(i - 2, survivor.InReserve);
                    Assert.Equal(2, survivor.InHand);
                }
            }

            Assert.Equal(2, survivor.InHand);
            Assert.Equal(3, survivor.InReserve);
        }

        [Theory]
        [InlineData(1, 4)]
        public void DropsTheLatestAddedEquipmentWhenWounded(int woundsToReceive, int remainingEquipment)
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            var equipmentToAdd = new Fixture().CreateMany<string>(5);

            foreach (var equipment in equipmentToAdd)
                survivor.AddEquipment(equipment);

            for (int i = 0; i < woundsToReceive; i++)
                survivor.Wound();

            Assert.Equal(remainingEquipment, survivor.InHand + survivor.InReserve);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(10, false)]
        public void DieWhenAtLeastTwoWoundsAreReceived(int woundsToInflict, bool shouldBeAlive)
        {
            var name = new Fixture().Create<string>();

            var survivor = new Survivor(name);

            for (int i = 0; i < woundsToInflict; i++)
            {
                survivor.Wound();
            }

            Assert.Equal(shouldBeAlive, survivor.IsAlive);
        }

        [Fact]
        public void BeCreatedWithZeroExperience()
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            Assert.Equal(0, survivor.Experience);
        }

        [Fact]
        public void BeCreatedWithLevelBlue()
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            Assert.Equal(Level.Blue, survivor.Level);
        }
    }
}