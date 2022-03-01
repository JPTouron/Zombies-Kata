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
        private IFixture f;
        private Mock<IClock> clock;
        private Game g;

        public GameShould()
        {
            f = new Fixture().Customize(new AutoMoqCustomization());
            clock = f.Freeze<Mock<IClock>>();
            g = f.Create<Game>();
        }

        [Fact]
        public void BeginsWithZeroSurvivors()
        {
            Assert.Equal(0, g.Survivors);
        }

        [Fact]
        public void BeginsWithLevelBlue()
        {
            Assert.Equal(Level.Blue, g.Level);
        }

        [Fact]
        public void BeginsWithTheStartingTimeInHistory()
        {
            var now = DateTime.UtcNow;

            clock.SetupGet(x => x.Now).Returns(now);
            g = f.Create<Game>();

            Assert.Equal(now.ToString(), g.History.First());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        public void AddASurvivor(int survivorsToAdd)
        {
            var survivors = GameProvider.CreateGameWithMultipleRandomPlayers(g, survivorsToAdd);

            Assert.Equal(survivorsToAdd, g.Survivors);
        }

        [Fact]
        public void ThrowWhenAddedSurvivorNameIsNotUnique()
        {
            var survivorName = "player 1";
            var s1 = SurvivorProvider.CreateRandomSurvivor(survivorName);
            var s2 = SurvivorProvider.CreateRandomSurvivor(survivorName);

            g.AddSurvivor(s1);

            Assert.Throws<InvalidOperationException>(() => g.AddSurvivor(s2));
        }

        [Theory]
        [InlineData(3)]
        public void EndWhenAllTheSurvivorsDie(int survivorsToAdd)
        {
            var survivors = GameProvider.CreateGameWithMultipleRandomPlayers(g, survivorsToAdd);

            foreach (var s in survivors)
                while (s.IsAlive)
                    s.Wound();

            Assert.True(g.HasEnded);
        }

        [Theory]
        [InlineData(Level.Blue)]
        [InlineData(Level.Yellow)]
        [InlineData(Level.Orange)]
        [InlineData(Level.Red)]
        public void HaveALevelMatchingTheHighestSurvivorLevel(Level maxLevelToGainByASurvivor)
        {
            var survivorsToAdd = 3;
            var survivors = GameProvider.CreateGameWithMultipleRandomPlayers(g, survivorsToAdd);

            var s = survivors.ToList().ElementAt(2);

            while (s.Level < maxLevelToGainByASurvivor)
            {
                var z = new Zombie();
                while (z.IsAlive)
                    s.Attack(z);
            }

            Assert.Equal(s.Level, g.Level);
            Assert.Equal(maxLevelToGainByASurvivor, g.Level);
        }
    }
}