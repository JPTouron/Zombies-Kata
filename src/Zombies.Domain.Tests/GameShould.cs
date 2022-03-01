using System;
using System.Collections.Generic;
using Xunit;

namespace Zombies.Domain.Tests
{
    public class GameShould
    {
        [Fact]
        public void BeginWithZeroSurvivors()
        {
            var g = new Game();

            Assert.Equal(0, g.Survivors);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        public void AddASurvivor(int survivorsToAdd)
        {
            IEnumerable<Survivor> survivors;
            var g = GameProvider.CreateGameWithMultipleRandomPlayers(out survivors, survivorsToAdd);

            Assert.Equal(survivorsToAdd, g.Survivors);
        }

        [Fact]
        public void ThrowWhenAddedSurvivorNameIsNotUnique()
        {
            var survivorName = "player 1";
            var s1 = SurvivorProvider.CreateRandomSurvivor(survivorName);
            var s2 = SurvivorProvider.CreateRandomSurvivor(survivorName);

            var g = new Game();
            g.AddSurvivor(s1);

            Assert.Throws<InvalidOperationException>(() => g.AddSurvivor(s2));
        }

        [Theory]
        [InlineData(3)]
        public void EndWhenAllTheSurvivorsDie(int survivorsToAdd)
        {
            IEnumerable<Survivor> survivors;

            var g = GameProvider.CreateGameWithMultipleRandomPlayers(out survivors, survivorsToAdd);

            foreach (var s in survivors)
                while (s.IsAlive)
                    s.Wound();

            Assert.True(g.HasEnded);
        }
    }
}