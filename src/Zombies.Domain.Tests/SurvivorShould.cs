using Ardalis.GuardClauses;
using AutoFixture;
using System;
using Xunit;

namespace Zombies.Domain.Tests
{
    public class SurvivorShould
    {
        public SurvivorShould()
        {
        }

        [Theory]
        [InlineData("John")]
        [InlineData("Martin")]
        public void HaveAName(string name)
        {
            var s = SurvivorProvider.CreateRandomSurvivor(name);

            Assert.Equal(name, s.Name);
        }

        [Theory]
        [InlineData("", typeof(ArgumentException))]
        [InlineData((string?)null, typeof(ArgumentNullException))]
        public void ThrowErrorOnEmptyNames(string name, Type expectedException)
        {
            Assert.Throws(expectedException, () => new Survivor(name));
        }

        [Fact]
        public void BeCreatedWithZeroWounds()
        {
            var s = SurvivorProvider.CreateRandomSurvivor();

            Assert.Equal(0, s.Wounds);
        }

        [Fact]
        public void BeCreatedWithAliveStatus()
        {
            var s = SurvivorProvider.CreateRandomSurvivor();

            Assert.True(s.IsAlive);
        }

        [Fact]
        public void BeCreatedWithAbilityToPerformUpToThreeActionsPerTurn()
        {
            var s = SurvivorProvider.CreateRandomSurvivor();

            Assert.Equal(3, s.AvailableActionsInTurn);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(10, false)]
        public void DieWhenAtLeastTwoWoundsAreReceived(int woundsToInflict, bool shouldBeAlive)
        {
            var name = new Fixture().Create<string>();

            var s = new Survivor(name);

            for (int i = 0; i < woundsToInflict; i++)
            {
                s.Wound();
            }

            Assert.Equal(shouldBeAlive, s.IsAlive);
        }
    }

    internal class Survivor
    {
        public Survivor(string name)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));

            Name = name;
            Wounds = 0;
            AvailableActionsInTurn = 3;
        }

        public string Name { get; }

        public int Wounds { get; private set; }

        public int AvailableActionsInTurn { get; }

        public bool IsAlive => Wounds < 2;

        internal void Wound()
        {
            Wounds++;
        }
    }
}