namespace Zombies.Domain
{
    public interface IKillingSurvivor
    {
        string Name { get; }
    }

    public class Zombie : IZombieUnderAttack
    {
        public Zombie()
        {
            Health = 2;
            KilledBy = "none";
        }

        public int Health { get; private set; }

        public bool IsAlive => Health > 0;

        public string KilledBy { get; private set; }

        public void Wound(IKillingSurvivor killingSurvivor)
        {
            Health--;

            if (IsAlive == false)
                KilledBy = killingSurvivor.Name;
        }
    }
}