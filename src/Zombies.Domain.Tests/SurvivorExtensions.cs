namespace Zombies.Domain.Tests
{
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