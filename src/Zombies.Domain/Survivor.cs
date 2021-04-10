namespace Zombies.Domain
{
    public class Survivor
    {
        public Survivor(string name)
        {
            Name = name;
            Wounds = 0;
            RemainingActions = 3;
            CurrentState = State.Alive;
        }

        public enum State
        {
            Alive,
            Dead
        }

        public State CurrentState { get; private set; }

        public string Name { get; }

        public int RemainingActions { get; }

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