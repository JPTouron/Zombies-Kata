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

            Assert.Equal(expectedIncreasedExperience, sut.ExperiencePoints);
        }

        [Theory]
        [InlineData(new object[] { 6, 6, 18 })]
        [InlineData(new object[] { 18, 18, 42 })]
        [InlineData(new object[] { 42, 42, 42 })]
        public void LevelUpWhenNextLevelExperiencePointsAreReached(int increaseTimes, int expectedLevel, int expectedMaxPoints)
        {
            var sut = new Experience();

            for (int i = 0; i < increaseTimes; i++)
                sut.Increase();

            Assert.Equal(expectedLevel, sut.ExperiencePoints);
            Assert.Equal((XpLevel)expectedLevel, sut.Level);
            Assert.Equal(expectedMaxPoints, sut.MaxLevelPoints);
        }

        public class BeCreated
        {
            [Fact]
            public void WithZeroExperiencePointsAndLevelBlueAndMaxLevelPointsSix()
            {
                var sut = new Experience();

                Assert.Equal(0, sut.ExperiencePoints);
                Assert.Equal(6, sut.MaxLevelPoints);
                Assert.Equal(XpLevel.Blue, sut.Level);
            }
        }
    }
}