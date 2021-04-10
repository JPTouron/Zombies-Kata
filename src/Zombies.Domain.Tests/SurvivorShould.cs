using Xunit;

namespace Zombies.Domain.Tests
{
    public class SurvivorShould
    {
        private Survivor sut;

        [Fact]
        public void DieWhenTwoWoundsAreInflicted()
        {
            var inflictedWounds = 2;
            var expectedState = Survivor.State.Dead;
            var sut = Utils.CreateSurvivor();

            sut.Wound(inflictedWounds);

            Assert.Equal(expectedState, sut.CurrentState);
        }

        [Fact]
        public void NotDieWhenASingleWoundIsInflicted()
        {
            var inflictedWounds = 1;
            var expectedState = Survivor.State.Alive;
            var sut = Utils.CreateSurvivor();

            sut.Wound(inflictedWounds);

            Assert.Equal(expectedState, sut.CurrentState);
        }

        [Fact]
        public void NotHaveMoreThanTwoWounds()
        {
            var inflictedWounds = 15;
            var expectedMaxWounds = 2;

            var sut = Utils.CreateSurvivor();

            sut.Wound(inflictedWounds);

            Assert.Equal(expectedMaxWounds, sut.Wounds);


        }
        [Fact]
        public void NotHaveMoreWoundsAfterDeath()
        {
            var inflictedWounds = 15;
            var expectedMaxWounds = 2;
            

            var sut = Utils.CreateSurvivor();

            sut.Wound(inflictedWounds);
            Assert.Equal(Survivor.State.Dead, sut.CurrentState);

            sut.Wound(inflictedWounds);
            Assert.Equal(expectedMaxWounds, sut.Wounds);


        }

        public class BeCreated
        {
            private Survivor sut;

            [Fact]
            public void WithANameAndZeroWoundsAndThreeRemainingActionsAndAlive()
            {
                var name = "JP";
                var expectedWounds = 0;
                var expectedRemainingActions = 3;
                var expectedState = Survivor.State.Alive;

                sut = Utils.CreateSurvivor(name);

                Assert.Equal(name, sut.Name);
                Assert.Equal(expectedWounds, sut.Wounds);
                Assert.Equal(expectedRemainingActions, sut.RemainingActions);
                Assert.Equal(expectedState, sut.CurrentState);
            }
        }
    }
}