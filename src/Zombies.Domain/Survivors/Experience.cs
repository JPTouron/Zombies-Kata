namespace Zombies.Domain.Survivors
{
    public sealed class Experience
    {
        public Experience()
        {
            ExperienceValue = 0;
        }

        public int ExperienceValue { get; private set; }

        public XpLevel Level
        {
            get
            {
                if (ExperienceValue < (int)XpLevel.Yellow)
                    return XpLevel.Blue;

                if (ExperienceValue < (int)XpLevel.Orange)
                    return XpLevel.Yellow;

                if (ExperienceValue < (int)XpLevel.Red)
                    return XpLevel.Orange;

                return XpLevel.Red;
            }
        }

        public int MaxValue
        {
            get
            {
                if (Level == XpLevel.Blue)
                    return (int)XpLevel.Yellow;

                if (Level == XpLevel.Yellow)
                    return (int)XpLevel.Orange;

                return (int)XpLevel.Red;
            }
        }

        public void Increase()
        {
            ExperienceValue++;
        }
    }
}