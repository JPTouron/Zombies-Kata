namespace Zombies.Domain.Survivors
{
    public sealed class Experience
    {
        public Experience()
        {
            ExperiencePoints = 0;
        }

        public int ExperiencePoints { get; private set; }

        public XpLevel Level
        {
            get
            {
                if (ExperiencePoints < (int)XpLevel.Yellow)
                    return XpLevel.Blue;

                if (ExperiencePoints < (int)XpLevel.Orange)
                    return XpLevel.Yellow;

                if (ExperiencePoints < (int)XpLevel.Red)
                    return XpLevel.Orange;

                return XpLevel.Red;
            }
        }

        public int MaxLevelPoints
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
            ExperiencePoints++;
        }
    }
}