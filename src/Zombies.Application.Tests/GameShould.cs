using AutoFixture;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Zombies.Application.HistoryRecording.GameHistory;
using Zombies.Application.HistoryRecording.GameHistory.Events;
using Zombies.Application.HistoryRecording.Infrastructure;
using Zombies.Application.HistoryRecording.SuvivorHistory.Events;
using Zombies.Domain;
using static Zombies.Application.IGame;

namespace Zombies.Application.Tests
{
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
        public void GameAlwaysHaveTheSameXpLevelAndExperienceValueAsTheMaxedLeveledUpSurvivor(
            int survivor1XpPoints,
            int survivor2XpPoints,
            int survivor3XpPoints,
            int expectedGameExperience,
            XpLevel expectedGameXpLevel)
        {
            var sut = Utils.CreateGame();
            var survivor1 = sut.AddSurvivor(new Fixture().Create<string>());
            var survivor2 = sut.AddSurvivor(new Fixture().Create<string>());
            var survivor3 = sut.AddSurvivor(new Fixture().Create<string>());

            for (int i = 0; i < survivor1XpPoints; i++)
                survivor1.Kill(new Zombie());

            for (int i = 0; i < survivor2XpPoints; i++)
                survivor2.Kill(new Zombie());

            for (int i = 0; i < survivor3XpPoints; i++)
                survivor3.Kill(new Zombie());

            Assert.Equal(expectedGameExperience, sut.ExperiencePoints);
            Assert.Equal(expectedGameXpLevel, sut.Level);
        }

        [Fact]
        public void GameFinishessWhenAllSuvivorsDie()
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
                Assert.Equal(0, sut.ExperiencePoints);
                Assert.Equal(XpLevel.Blue, sut.Level);
            }
        }

        public class RecordHistory
        {
            private Mock<IGameEventsRecorder> historyRecorder;

            public RecordHistory()
            {
                historyRecorder = new Fixture().Create<Mock<IGameEventsRecorder>>();
            }

            [Fact]
            public void EventsInOrder()
            {
                var recorder = HistoryRecorder.Instance();

                recorder.Events.Clear();

                var sut = Utils.CreateGame(recorder);

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
            public void WhenAddingASurvivor()
            {
                var sut = Utils.CreateGame(historyRecorder.Object);
                var s = sut.AddSurvivor(new Fixture().Create<string>());

                historyRecorder.Verify(x => x.SurvivorAdded(s), Times.Once);
            }

            [Fact]
            public void WhenGameLevelsUp()
            {
                var sut = Utils.CreateGame(historyRecorder.Object);
                var s = sut.AddSurvivor(new Fixture().Create<string>());

                var initialLevel = s.Level;
                while (s.Level == initialLevel)
                    s.Kill(new Zombie());

                var expectedRecordedLevel = s.Level;

                historyRecorder.Verify(x => x.GameLeveledUp(expectedRecordedLevel), Times.Once);
            }

            [Fact]
            public void WhenGamesFinishes()
            {
                var sut = Utils.CreateGame(historyRecorder.Object);
                var s = sut.AddSurvivor(new Fixture().Create<string>());
                while (s.CurrentState == HealthState.Alive)
                    s.Wound(1);

                historyRecorder.Verify(x => x.GameFinished(), Times.Once);
            }

            [Fact]
            public void WhenGameStarts()
            {
                var sut = Utils.CreateGame(historyRecorder.Object);

                historyRecorder.Verify(x => x.GameStarted(), Times.Once);
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