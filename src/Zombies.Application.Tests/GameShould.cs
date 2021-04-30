using AutoFixture;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Zombies.Application.History.EventMessages;
using Zombies.Domain;
using static Zombies.Application.IGame;

namespace Zombies.Application.Tests
{
    //JP: ADD TESTS FOR APPLICATION.PROVIDERS
    public class GameShould
    {
        [Fact]
        public void BeAbleToAddANewSurivor()
        {
            var sut = Utils.CreateGame();
            var survivor = new Fixture().Create<string>();

            sut.AddSurvivor(survivor);

            Assert.Equal(1, sut.SurvivorCount);
        }

        [Fact]
        public void BeOnGoingIfAnySurvivorsLive()
        {
            var sut = Utils.CreateGame();

            var fixture = new Fixture();

            var survivor1 = sut.AddSurvivor(fixture.Create<string>());
            var survivor2 = sut.AddSurvivor(fixture.Create<string>());
            sut.AddSurvivor(fixture.Create<string>());

            survivor1.Wound(3);
            survivor2.Wound(3);

            Assert.Equal(GameState.OnGoing, sut.State);
        }

        [Theory]
        [ClassData(typeof(ExperienceTestsProvider))]
        public void GameAlwaysHaveTheSameXpLevelAndExperienceValueAsTheMaxedLeveledUpSurvivor(int xpSurvivor1, int xpSurvivor2, int xpSurvivor3, int expectedGameExperience, XpLevel expectedGameXpLevel)
        {
            var sut = Utils.CreateGame();
            var survivor1 = sut.AddSurvivor(new Fixture().Create<string>());
            var survivor2 = sut.AddSurvivor(new Fixture().Create<string>());
            var survivor3 = sut.AddSurvivor(new Fixture().Create<string>());

            for (int i = 0; i < xpSurvivor1; i++)
                survivor1.Kill(new Zombie());

            for (int i = 0; i < xpSurvivor2; i++)
                survivor2.Kill(new Zombie());

            for (int i = 0; i < xpSurvivor3; i++)
                survivor3.Kill(new Zombie());

            Assert.Equal(expectedGameExperience, sut.ExperienceValue);
            Assert.Equal(expectedGameXpLevel, sut.Level);
        }

        [Fact]
        public void GameEndsWhenAllSuvivorsDie()
        {
            var sut = Utils.CreateGame();

            var survivor1 = sut.AddSurvivor(new Fixture().Create<string>());
            var survivor2 = sut.AddSurvivor(new Fixture().Create<string>());
            var survivor3 = sut.AddSurvivor(new Fixture().Create<string>());

            survivor1.Wound(3);
            survivor2.Wound(3);
            survivor3.Wound(3);

            Assert.Equal(GameState.Finished, sut.State);
        }

        [Fact]
        public void RecordHistoricEventsInOrder()
        {
            var sut = Utils.CreateGame();

            var survivor = sut.AddSurvivor(new Fixture().Create<string>());
            survivor.Wound(2);

            var expectedOrderedMessages =
                new Dictionary<int, string>(
                    new List<KeyValuePair<int, string>> {
                        new KeyValuePair<int, string>(0,new GameStartedEventMessage().Message),
                        new KeyValuePair<int, string>(1,new SurvivorAddedToGameEventMessage(survivor).Message),
                        new KeyValuePair<int, string>(2,new SurvivorWoundedEventMessage(survivor).Message),
                        new KeyValuePair<int, string>(3,new SurvivorDiedEventMessage(survivor).Message),
                        new KeyValuePair<int, string>(4,new GameFinishedEventMessage().Message)
                });

            var history = sut.Events;

            Assert.Equal(5, history.Count);
            for (int i = 0; i < history.Count; i++)
                Assert.Equal(expectedOrderedMessages[i], history[i].Message);
        }

        [Fact]
        public void ThrowWhenAddingANullSurvivor()
        {
            var sut = Utils.CreateGame();
            string survivor = null;

            Assert.Throws<ArgumentNullException>(() => sut.AddSurvivor(survivor));
        }

        [Fact]
        public void ThrowWhenAddingASurvivorWithExistingName()
        {
            var sut = Utils.CreateGame();

            var survivor = new Fixture().Create<string>();
            sut.AddSurvivor(survivor);

            Assert.Throws<InvalidOperationException>(() => sut.AddSurvivor(survivor));
        }

        public class BeCreated
        {
            [Fact]
            public void WithZeroSurvivors()
            {
                var sut = Utils.CreateGame();

                Assert.Equal(0, sut.SurvivorCount);
                Assert.Equal(GameState.Finished, sut.State);
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