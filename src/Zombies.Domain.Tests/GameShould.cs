using System;
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


            Assert.Equal( Game.GamesState.Finished, sut.State);
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


            Assert.Equal(Game.GamesState.OnGoing, sut.State);
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
                Assert.Equal(Game.GamesState.Finished, sut.State);
            }
        }
    }
}