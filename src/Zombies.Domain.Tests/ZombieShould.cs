using Xunit;

namespace Zombies.Domain.Tests
{
    public class ZombieShould
    {
        [Fact]
        public void BeCreatedWithTwoHealthPoints()
        {
            var z = new Zombie();

            Assert.Equal(2, z.Health);
        }

        [Fact]
        public void BeCreatedWithNoKilledByName()
        {
            var z = new Zombie();

            Assert.Equal("none", z.KilledBy);
        }

        [Fact]
        public void DieAfterTwoHits()
        {
            var s = SurvivorProvider.CreateRandomSurvivor() as IKillingSurvivor;
            var z = new Zombie();

            z.Wound(s);
            z.Wound(s);

            Assert.False(z.IsAlive);
            Assert.True(z.IsDead);
        }

        [Fact]
        public void RecordSurvivorNameThatKilledIt()
        {
            var s = SurvivorProvider.CreateRandomSurvivor() as IKillingSurvivor;

            var z = new Zombie();

            z.Wound(s);
            z.Wound(s);

            Assert.Equal(s.Name, z.KilledBy);
        }
    }
}