using Xunit;

namespace Zombies.Domain.Tests
{
    public class HealthShould
    {
        [Fact]
        public void BeCreatedAsAliveAndNoWounds()
        {
            var sut = Utils.CreateHealth();

            Assert.True(sut.CurrentState == HealthState.Alive);
            Assert.True(sut.Wounds == 0);
        }

        [Fact]
        public void BeInAliveStatusIfWoundedOnce()
        {
            var sut = Utils.CreateHealth();

            sut.Wound(1);

            Assert.True(sut.CurrentState == HealthState.Alive);
        }

        [Theory]
        [InlineData(new object[] { 2, (HealthState)1 })]
        [InlineData(new object[] { int.MaxValue, (HealthState)1 })]
        public void BeInDeadStatusIfWoundedTwoOrMoreTimes(int inflictedWounds, HealthState expectedState)
        {
            var sut = Utils.CreateHealth();

            sut.Wound(inflictedWounds);

            Assert.Equal(expectedState, sut.CurrentState);
        }

        [Theory]
        [InlineData(new object[] { 1, 1 })]
        [InlineData(new object[] { 2, 2 })]
        [InlineData(new object[] { 5, 2 })]
        [InlineData(new object[] { int.MaxValue, 2 })]
        public void IncreaseWoundsUpToTwoWhenWounded(int inflictedWounds, int expectedWounds)
        {
            var sut = Utils.CreateHealth();

            sut.Wound(inflictedWounds);

            Assert.Equal(expectedWounds, sut.Wounds);
        }

        [Theory]
        [InlineData(new object[] { 0 })]
        [InlineData(new object[] { -3 })]
        [InlineData(new object[] { int.MinValue })]
        public void NotIncreaseWoundsWhenInflicedIsZeroOrLess(int inflictedWounds)
        {
            var sut = Utils.CreateHealth();
            var expectedWounds = sut.Wounds;
            sut.Wound(inflictedWounds);

            Assert.Equal(expectedWounds, sut.Wounds);
        }
    }
}