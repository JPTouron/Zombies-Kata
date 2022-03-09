using AutoFixture;
using System;
using System.Linq;
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
            Assert.Throws(expectedException, () => new Survivor(name, new SkillTreeFactory()));
        }

        [Fact]
        public void ThrowErrorOnNullSkillTree()
        {
            var name = new Fixture().Create<string>();

            Assert.Throws<ArgumentNullException>(() => new Survivor(name, null));
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

        [Fact]
        public void BeCreatedWithASkillTreeWithOnlyPotentialSkills()
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            Assert.Empty(survivor.UnlockedSkills);
            Assert.NotEmpty(survivor.PotentialSkills);
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
        public void OnlyAddEquipmentWhenNameIsNotNullOrEmpty(string equipmentName, Type exceptionType)
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

            var survivor = SurvivorProvider.CreateRandomSurvivor(name);

            for (int i = 0; i < woundsToInflict; i++)
            {
                survivor.Wound();
            }

            Assert.Equal(shouldBeAlive, survivor.IsAlive);
            Assert.Equal(shouldBeAlive == false, survivor.IsDead);
        }

        [Theory]
        [InlineData(0, Level.Blue)]
        [InlineData(6, Level.Yellow)]
        [InlineData(18, Level.Orange)]
        [InlineData(42, Level.Red)]
        [InlineData(61, Level.Red)]
        [InlineData(86, Level.Red)]
        [InlineData(129, Level.Red)]
        public void KeepLevelRedWhileStillGaininigExperiencePoints(int experiencePointsReached, Level expectedSurvivorLevel)
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            survivor.LevelUpSurvivorTo(experiencePointsReached);

            Assert.Equal(expectedSurvivorLevel, survivor.Level);
        }

        [Fact]
        public void HaveAutomaticallyUnlockAnExtraActionSkillWhenReachingYellowLevel()
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            survivor.LevelUpSurvivorTo(Level.Yellow);

            Assert.Contains(survivor.UnlockedSkills, x => string.Compare(x.Name, "+1 Action", StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        [Theory]
        [InlineData(18, "+1 Die (Ranged)")]
        [InlineData(42, "+1 Die (Melee)")]
        public void HaveCertainAvailableSkillsWhenReachingSpecificExperiencePoints(int experiencePointsReached, string expectedSkillAvailable)
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            survivor.LevelUpSurvivorTo(experiencePointsReached);

            Assert.Contains(survivor.PotentialSkills, x => string.Compare(x.Name, expectedSkillAvailable, StringComparison.InvariantCultureIgnoreCase) == 0
                                                               && x.IsAvailable);
        }

        [Theory]
        [InlineData(6, "+1 Action")]
        [InlineData(61, "+1 Free Move Action")]
        [InlineData(86, "Hoard")]
        [InlineData(129, "Tough")]
        public void HaveCertainAutoUnlockedSkillsWhenReachingSpecificExperiencePoints(int experiencePointsReached, string expectedSkillAvailable)
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            survivor.LevelUpSurvivorTo(experiencePointsReached);

            Assert.Contains(survivor.UnlockedSkills, x => string.Compare(x.Name, expectedSkillAvailable, StringComparison.InvariantCultureIgnoreCase) == 0
                                                       && x.IsAvailable);
        }

        [Fact]
        public void HaveNoMorePotentialSkillsWhenReaching50ExperiencePoints()
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            survivor.LevelUpSurvivorTo(50);
            survivor.PotentialSkills.ToList().ForEach(x => x.Unlock());

            Assert.Empty(survivor.PotentialSkills);
        }

        [Theory]
        [InlineData(Level.Yellow)]
        [InlineData(Level.Orange)]
        [InlineData(Level.Red)]
        public void HaveFourAvailableActionsInTurnAfterReachingYellowLevel(Level levelUpTo)
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            survivor.LevelUpSurvivorTo(levelUpTo);

            Assert.Equal(4, survivor.AvailableActionsInTurn);
        }

        [Theory]
        [InlineData(6)]
        [InlineData(10)]
        public void HaveUpToSixEquipmentItemsWhenHoardSkillIsUnlocked(int equipmentToAdd)
        {
            var expectedMaxStoredEquipment = 6;
            var survivor = SurvivorProvider.CreateRandomSurvivor();

            survivor.LevelUpSurvivorTo(82);

            for (int i = 0; i < equipmentToAdd; i++)
            {
                var e = new Fixture().Create<string>();
                survivor.AddEquipment(e);
            }

            Assert.Contains(survivor.UnlockedSkills, x => x.Name == "Hoard" && x.IsUnlocked);
            Assert.Equal(expectedMaxStoredEquipment, survivor.InHand + survivor.InReserve);
        }
    }
}