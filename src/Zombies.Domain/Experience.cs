namespace Zombies.Domain
{
    internal class Experience
    {
        public Experience()
        {
            Value = 0;
        }

        public enum XpLevel
        {
            Blue = 0,
            Yellow = 6,
            Orange = 18,
            Red = 42
        }

        public XpLevel Level
        {
            get
            {
                if (Value < (int)XpLevel.Yellow)
                    return XpLevel.Blue;

                if (Value < (int)XpLevel.Orange)
                    return XpLevel.Yellow;

                if (Value < (int)XpLevel.Red)
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

        public int Value { get; private set; }

        public void Increase()
        {
            Value++;
        }
    }
}