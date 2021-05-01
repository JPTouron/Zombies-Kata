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

        [Fact]
        public void WhenExperiencePointsIsOnMaxThenPossibleValueItCannotIncrease()
        {
            var sut = Utils.CreateMaxedOutExperience();
            short expectedExpPoints = 149;

            sut.Increase();

            Assert.Equal(expectedExpPoints, sut.ExperiencePoints);
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

        public class ComplyWithBusinessRules
        {
            [Fact]
            public void WhenExperiencePointsIsNotOnMaxThenPossibleValueItCanIncrease()
            {
                short maxExperiencePoints = 149;
                var currentExpPoints = (short)(maxExperiencePoints - 1);
                var sut = new ExperienceCannotIncreaseOverThresholdRule(currentExpPoints, 1,maxExperiencePoints);

                Assert.False(sut.IsBroken());
            }

            [Fact]
            public void WhenExperiencePointsIsOnMaxThenPossibleValueItCannotIncrease()
            {
                short maxExperiencePoints = 149;
                var currentExpPoints = maxExperiencePoints;
                var sut = new ExperienceCannotIncreaseOverThresholdRule(currentExpPoints, 1, maxExperiencePoints);

                Assert.True(sut.IsBroken());
            }
        }
    }
}