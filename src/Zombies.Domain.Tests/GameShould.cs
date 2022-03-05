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
        public void BeginWithTheStartingTimeInHistory()
        {
            var now = DateTime.UtcNow;

            clock.SetupGet(x => x.Now).Returns(now);
            game = fixture.Create<Game>();

            Assert.Equal(now.ToString(), game.History.First());
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
            var survivor = SurvivorProvider.CreateRandomSurvivor();
            game.AddSurvivor(survivor);

            Assert.Equal(2, game.History.Count);
            Assert.Equal($"Survivor {survivor.Name} has joined the game", game.History.Last());
        }

        [Fact]
        public void RecordASurvivorAcquiredEquipmentInTheHistory()
        {
            var survivor = SurvivorProvider.CreateRandomSurvivor();
            game.AddSurvivor(survivor);
            var addedEquipment = "equipment";
            survivor.AddEquipment(addedEquipment);

            Assert.Equal(3, game.History.Count);
            Assert.Equal($"Survivor {survivor.Name} acquired {addedEquipment}", game.History.Last());
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

            foreach (var s in survivors)
                while (s.IsAlive)
                    s.Wound();

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

            var s = survivors.ToList().ElementAt(2);

            while (s.Level < maxLevelToGainByASurvivor)
            {
                var z = new Zombie();
                while (z.IsAlive)
                    s.Attack(z);
            }

            Assert.Equal(s.Level, game.Level);
            Assert.Equal(maxLevelToGainByASurvivor, game.Level);
        }
    }
}