using AutoFixture;

namespace Zombies.Domain.Tests
{
    internal static class SurvivorProvider
    {
        public static Survivor CreateRandomSurvivor(string? name = null, SkillTreeFactory? treeFactory = null)
        {
            if (name == null)
                name = new Fixture().Create<string>();

            if (treeFactory == null)
                treeFactory = new SkillTreeFactory();

            var s = new Survivor(name, treeFactory);
            return s;
        }
    }

    internal static class SurvivorExtensions
    {
        public static void LevelUpSurvivorTo(this Survivor survivor, Level levelToGoTo)
        {
            while (survivor.Level < levelToGoTo)
                KillAZombie(survivor);
        }

        public static void LevelUpSurvivorTo(this Survivor survivor, int experiencePoints)
        {
            while (survivor.Experience < experiencePoints)
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