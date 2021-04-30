namespace Zombies.Domain.Survivors
{
    public sealed class Health
    {
        public Health()
        {
            CurrentState = HealthState.Alive;
            Wounds = 0;
        }

        public HealthState CurrentState { get; private set; }

        public int Wounds { get; private set; }

        public void Wound(int inflictedWounds)
        {
            Wounds += inflictedWounds;
            if (Wounds > 2)
                Wounds = 2;

            if (Wounds == 2)
                CurrentState = HealthState.Dead;
        }
    }
}