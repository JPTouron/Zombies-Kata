using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Zombies.Domain.Tests
{
    public class GameShould
    {
        private IFixture fixture;
        private Mock<IClock> clock;
        private Game game;

        public GameShould()
        {
            fixture = new Fixture().Customize(new AutoMoqCustomization());

            clock = fixture.Freeze<Mock<IClock>>();

            //Whenever a IGameHistory is required, then Provide it using the static method in game
            fixture.Register<IGameHistoryTracker>(() => Game.CreateGameHistoryTracker(clock.Object));

            game = fixture.Create<Game>();
        }

        [Fact]
        public void BeginWithZeroSurvivors()
        {
            Assert.Equal(0, game.PlayingSurvivors);
        }

        [Fact]
        public void BeginWithLevelBlue()
        {
            Assert.Equal(Level.Blue, game.Level);
        }

        [Fact]
        public void BeginWithRecordingGameStarted()
        {
            var now = DateTime.UtcNow;

            clock.SetupGet(x => x.Now).Returns(now);
            game = fixture.Create<Game>();

            Assert.Equal(now, game.History.First().IncidentDate);
            Assert.Equal("A new game has started", game.History.First().Incident);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        public void AddASurvivor(int survivorsToAdd)
        {
            GameProvider.AddSurvivorsToAGame(game, survivorsToAdd);

            Assert.Equal(survivorsToAdd, game.PlayingSurvivors);
        }

        [Fact]
        public void RecordASurvivorWasAddedInTheHistory()
        {
            var now = DateTime.UtcNow;
            clock.SetupGet(x => x.Now).Returns(now);

            var survivor = SurvivorProvider.CreateRandomSurvivor();
            game.AddSurvivor(survivor);

            Assert.Equal(2, game.History.Count);
            Assert.Equal(now, game.History.Last().IncidentDate);
            Assert.Contains(game.History, x => x.Incident == $"Survivor {survivor.Name} has joined the game");
        }

        [Fact]
        public void RecordASurvivorAcquiredEquipmentInTheHistory()
        {
            var now = DateTime.UtcNow;
            clock.SetupGet(x => x.Now).Returns(now);

            var survivor = SurvivorProvider.CreateRandomSurvivor();
            game.AddSurvivor(survivor);
            var addedEquipment = "equipment";
            survivor.AddEquipment(addedEquipment);

            Assert.Equal(now, game.History.Last().IncidentDate);
            Assert.Contains(game.History, x => x.Incident == $"Survivor {survivor.Name} acquired {addedEquipment}");
        }

        [Fact]
        public void RecordASurvivorWasWoundedInTheHistory()
        {
            var now = DateTime.UtcNow;
            clock.SetupGet(x => x.Now).Returns(now);

            var survivor = SurvivorProvider.CreateRandomSurvivor();
            game.AddSurvivor(survivor);
            survivor.Wound();

            Assert.Equal(now, game.History.Last().IncidentDate);
            Assert.Contains(game.History, x => x.Incident == $"Survivor {survivor.Name} has been wounded!");
        }

        [Fact]
        public void RecordASurvivorDiedInTheHistory()
        {
            var now = DateTime.UtcNow;
            clock.SetupGet(x => x.Now).Returns(now);

            var survivor = SurvivorProvider.CreateRandomSurvivor();
            game.AddSurvivor(survivor);
            survivor.KillSurvivor();

            Assert.Equal(now, game.History.Last().IncidentDate);
            Assert.Contains(game.History, x => x.Incident == $"Survivor {survivor.Name} has died!");
        }

        [Fact]
        public void RecordASurvivorHasLeveledUpInTheHistory()
        {
            var now = DateTime.UtcNow;
            clock.SetupGet(x => x.Now).Returns(now);

            var survivor = SurvivorProvider.CreateRandomSurvivor();
            game.AddSurvivor(survivor);

            var expectedSurvivorLevel = Level.Yellow;
            survivor.LevelUpSurvivorTo(expectedSurvivorLevel);

            Assert.Equal(now, game.History.Last().IncidentDate);
            Assert.Contains(game.History, x => x.Incident == $"Survivor {survivor.Name} LeveledUp to level: {expectedSurvivorLevel}!");
        }

        [Fact]
        public void RecordTheGameHasLeveledUpInTheHistory()
        {
            var expectedGameLevel = Level.Orange;
            var now = DateTime.UtcNow;
            clock.SetupGet(x => x.Now).Returns(now);

            var s1 = SurvivorProvider.CreateRandomSurvivor();
            game.AddSurvivor(s1);
            var s2 = SurvivorProvider.CreateRandomSurvivor();
            game.AddSurvivor(s2);

            s1.LevelUpSurvivorTo(expectedGameLevel);
            s2.LevelUpSurvivorTo(Level.Yellow);

            Assert.Equal(now, game.History.Last().IncidentDate);
            Assert.Equal(s1.Level, game.Level);
            Assert.Contains(game.History, x => x.Incident == $"The Game has LeveledUp to level: {expectedGameLevel}!");
        }

        [Fact]
        public void RecordTheGameHasEndedInTheHistory()
        {
            var now = DateTime.UtcNow;
            clock.SetupGet(x => x.Now).Returns(now);

            var s1 = SurvivorProvider.CreateRandomSurvivor("s1");
            game.AddSurvivor(s1);

            var s2 = SurvivorProvider.CreateRandomSurvivor("s2");
            game.AddSurvivor(s2);

            s1.KillSurvivor();
            s2.KillSurvivor();

            Assert.Equal(now, game.History.Last().IncidentDate);
            Assert.Equal($"The Game has Ended. All survivors have died... Max level reached: {game.Level}", game.History.Last().Incident);
        }

        [Fact]
        public void ThrowWhenAddedSurvivorNameIsNotUnique()
        {
            var survivorName = "player 1";
            var s1 = SurvivorProvider.CreateRandomSurvivor(survivorName);
            var s2 = SurvivorProvider.CreateRandomSurvivor(survivorName);

            game.AddSurvivor(s1);

            Assert.Throws<InvalidOperationException>(() => game.AddSurvivor(s2));
        }

        [Theory]
        [InlineData(3)]
        public void EndWhenAllTheSurvivorsDie(int survivorsToAdd)
        {
            var survivors = GameProvider.AddSurvivorsToAGame(game, survivorsToAdd);

            foreach (var survivor in survivors)
                survivor.KillSurvivor();

            Assert.True(game.HasEnded);
        }

        [Theory]
        [InlineData(Level.Blue)]
        [InlineData(Level.Yellow)]
        [InlineData(Level.Orange)]
        [InlineData(Level.Red)]
        public void HaveALevelMatchingTheHighestSurvivorLevel(Level maxLevelToGainByASurvivor)
        {
            var survivorsToAdd = 3;
            var survivors = GameProvider.AddSurvivorsToAGame(game, survivorsToAdd);

            var survivor = survivors.ToList().ElementAt(2);

            survivor.LevelUpSurvivorTo(maxLevelToGainByASurvivor);

            Assert.Equal(survivor.Level, game.Level);
            Assert.Equal(maxLevelToGainByASurvivor, game.Level);
        }
    }
}