using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Zombies.Domain.Tests
{
    public class GameShould
    {
        [Fact]
        public void BeAbleToAddANewSurivor()
        {
            var sut = new Game();
            var survivor = Utils.CreateSurvivor();

            sut.AddSurvivor(survivor);

            Assert.Equal(1, sut.SurvivorCount);
        }

        [Theory]
        [ClassData(typeof(ExperienceTestsProvider))]
        public void GameAlwaysHaveTheSameXpLevelAndExperienceValueAsTheMaxedLeveledUpSurvivor(int xpSurvivor1, int xpSurvivor2, int xpSurvivor3, int expectedGameExperience, XpLevel expectedGameXpLevel)
        {
            var sut = new Game();
            var survivor1 = Utils.CreateSurvivor(xp: Utils.CreateXP(xpSurvivor1));
            var survivor2 = Utils.CreateSurvivor(xp: Utils.CreateXP(xpSurvivor2));
            var survivor3 = Utils.CreateSurvivor(xp: Utils.CreateXP(xpSurvivor3));
            sut.AddSurvivor(survivor1);
            sut.AddSurvivor(survivor2);
            sut.AddSurvivor(survivor3);

            Assert.Equal(expectedGameExperience, sut.ExperienceValue);
            Assert.Equal(expectedGameXpLevel, sut.Level);
        }

        [Fact]
        public void GameEndsWhenAllSuvivorsDie()
        {
            var sut = new Game();
            var survivor1 = Utils.CreateSurvivor();
            var survivor2 = Utils.CreateSurvivor();
            var survivor3 = Utils.CreateSurvivor();
            sut.AddSurvivor(survivor1);
            sut.AddSurvivor(survivor2);
            sut.AddSurvivor(survivor3);

            survivor1.Wound(3);
            survivor2.Wound(3);
            survivor3.Wound(3);

            Assert.Equal(Game.GameState.Finished, sut.State);
        }

        [Fact]
        public void GameIsOnGoingIfAnySurvivorsLive()
        {
            var sut = new Game();
            var survivor1 = Utils.CreateSurvivor();
            var survivor2 = Utils.CreateSurvivor();
            var survivor3 = Utils.CreateSurvivor();
            sut.AddSurvivor(survivor1);
            sut.AddSurvivor(survivor2);
            sut.AddSurvivor(survivor3);

            survivor1.Wound(3);
            survivor2.Wound(3);

            Assert.Equal(Game.GameState.OnGoing, sut.State);
        }

        [Fact]
        public void ThrowWhenAddingANullSurvivor()
        {
            var sut = new Game();
            Survivor survivor = null;

            Assert.Throws<ArgumentNullException>(() => sut.AddSurvivor(survivor));
        }

        [Fact]
        public void ThrowWhenAddingASurvivorWithExistingName()
        {
            var sut = new Game();

            var survivor = Utils.CreateSurvivor();
            sut.AddSurvivor(survivor);

            survivor = Utils.CreateSurvivor();
            sut.AddSurvivor(survivor);

            Assert.Throws<InvalidOperationException>(() => sut.AddSurvivor(survivor));
        }

        public class BeCreated
        {
            [Fact]
            public void WithZeroSurvivors()
            {
                var sut = new Game();

                Assert.Equal(0, sut.SurvivorCount);
                Assert.Equal(Game.GameState.Finished, sut.State);
                Assert.Equal(0, sut.ExperienceValue);
                Assert.Equal(XpLevel.Blue, sut.Level);
            }
        }

        private class ExperienceTestsProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { 3, 4, 5, 5, XpLevel.Blue };
                yield return new object[] { 10, 4, 5, 10, XpLevel.Yellow };
                yield return new object[] { 3, 20, 5, 20, XpLevel.Orange };
                yield return new object[] { 3, 4, 50, 50, XpLevel.Red };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}