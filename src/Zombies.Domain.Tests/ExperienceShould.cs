using Xunit;
using Zombies.Domain.Survivors;

namespace Zombies.Domain.Tests
{
    public class ExperienceShould
    {
        [Theory]
        [InlineData(new object[] { 1, 1 })]
        [InlineData(new object[] { 2, 2 })]
        [InlineData(new object[] { 3, 3 })]
        public void IncreaseExperienceByOne(int increaseTimes, int expectedIncreasedExperience)
        {
            var sut = new Experience();

            for (int i = 0; i < increaseTimes; i++)
                sut.Increase();

            Assert.Equal(expectedIncreasedExperience, sut.ExperienceValue);
        }

        [Theory]
        [InlineData(new object[] { 6, 6, 18 })]
        [InlineData(new object[] { 18, 18, 42 })]
        [InlineData(new object[] { 42, 42, 42 })]
        public void LevelUpWhenNextLevelValueIsReached(int increaseTimes, int expectedLevel, int expectedMaxValue)
        {
            var sut = new Experience();

            for (int i = 0; i < increaseTimes; i++)
                sut.Increase();

            Assert.Equal(expectedLevel, sut.ExperienceValue);
            Assert.Equal((XpLevel)expectedLevel, sut.Level);
            Assert.Equal(expectedMaxValue, sut.MaxValue);
        }

        public class BeCreated
        {
            [Fact]
            public void WithZeroValueAndLevelBlueAndMaxValueSix()
            {
                var sut = new Experience();

                Assert.Equal(0, sut.ExperienceValue);
                Assert.Equal(6, sut.MaxValue);
                Assert.Equal(XpLevel.Blue, sut.Level);
            }
        }
    }
}