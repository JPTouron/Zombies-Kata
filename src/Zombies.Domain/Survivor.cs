using Ardalis.GuardClauses;
using static Zombies.Domain.IHealth;

namespace Zombies.Domain
{
    public class Survivor : IHealth
    {
        private IHealth health;

        public Survivor(string name)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));

            Name = name;
            RemainingActions = 3;

            health = new Health();
        }

        public string Name { get; }

        public int RemainingActions { get; }

        public State CurrentState => health.CurrentState;

        public int Wounds => health.Wounds;

        public void Wound(int inflictedWounds)
        {
            health.Wound(inflictedWounds);
        }
    }
}