using static Zombies.Domain.IHealth;

namespace Zombies.Domain
{
    public interface IHealth
    {
        public enum State
        {
            Alive,
            Dead
        }

        State CurrentState { get; }

        int Wounds { get; }

        void Wound(int inflictedWounds);
    }

    internal sealed class Health : IHealth
    {
        public Health()
        {
            CurrentState = IHealth.State.Alive;
            Wounds = 0;
        }

        public State CurrentState { get; private set; }

        public int Wounds { get; private set; }

        public void Wound(int inflictedWounds)
        {
            Wounds += inflictedWounds;
            if (Wounds > 2)
                Wounds = 2;

            if (Wounds == 2)
                CurrentState = State.Dead;
        }
    }
}