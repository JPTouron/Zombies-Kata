using AutoFixture;

namespace Zombies.Domain.Tests
{
    internal static class SurvivorProvider
    {
        public static Survivor CreateRandomSurvivor(string? name = null)
        {
            if (name == null)
                name = new Fixture().Create<string>();

            var s =  Survivor.CreateWithEmptySkillTree(name);
            return s;
        }
    }

    internal static class SurvivorExtensions {
        public static void LevelUpSurvivorTo(this Survivor survivor, Level levelToGoTo)
        {
            while (survivor.Level < levelToGoTo)
                KillAZombie(survivor);
        }

        public static void KillSurvivor(this Survivor survivor)
        {
            while (survivor.IsAlive)
                survivor.Wound();
        }

        public static void KillAZombie(this Survivor survivor)
        {
            var zombie = new Zombie();

            while (zombie.IsAlive)
                survivor.Attack(zombie);
        }
    }
}