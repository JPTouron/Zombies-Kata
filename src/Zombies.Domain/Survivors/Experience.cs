using Zombies.Domain.BuildingBocks;

namespace Zombies.Domain.Survivors
{
    internal class ExperienceCannotIncreaseOverThresholdRule : IBusinessRuleCheck
    {
        private readonly short currentExperiencePoints;
        private readonly short experiencePointsIncrease;
        private readonly short maxExperiencePoints;

        public ExperienceCannotIncreaseOverThresholdRule(short currentExperiencePoints, short experiencePointsIncrease, short maxExperiencePoints)
        {
            this.currentExperiencePoints = currentExperiencePoints;
            this.experiencePointsIncrease = experiencePointsIncrease;
            this.maxExperiencePoints = maxExperiencePoints;
        }

        public bool IsBroken()
        {
            var deltaToThreshold = maxExperiencePoints - currentExperiencePoints;

            return experiencePointsIncrease > deltaToThreshold;
        }
    }
    public sealed class Experience
    {
        public Experience()
        {
            ExperiencePoints = 0;
        }

        public short ExperiencePoints { get; private set; }

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
        private const int MaxExperiencePoints = 149;

        public void Increase()
        {
            short increase = 1;

            var rule = new ExperienceCannotIncreaseOverThresholdRule(ExperiencePoints, increase, MaxExperiencePoints);
            var canIncreaseExperience = !rule.IsBroken();

            if (canIncreaseExperience)
                ExperiencePoints++;
        }
    }
}