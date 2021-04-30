using Zombies.Domain.Gear;

namespace Zombies.Application.History.EventMessages
{
    internal sealed class SurvivorAddedToGameEventMessage : SurvivorEventMessageBase
    {
        public SurvivorAddedToGameEventMessage(ISurvivor survivor) : base(survivor)
        {
        }

        public override string Message => $"Survivor {survivor.Name} has joined the game";
    }

    internal sealed class SurvivorDiedEventMessage : SurvivorEventMessageBase
    {
        public SurvivorDiedEventMessage(ISurvivor survivor) : base(survivor)
        {
        }

        public override string Message => $"{survivor.Name} has died!";
    }

    internal sealed class SurvivorEquipmentEventMessage : SurvivorEventMessageBase
    {
        private readonly IEquipment equipmentAdded;

        public SurvivorEquipmentEventMessage(ISurvivor survivor, IEquipment equipmentAdded) : base(survivor)
        {
            this.equipmentAdded = equipmentAdded;
        }

        public override string Message => $"{survivor.Name} has added {equipmentAdded.Name}";
    }

    internal abstract class SurvivorEventMessageBase
    {
        protected readonly ISurvivor survivor;

        protected SurvivorEventMessageBase(ISurvivor survivor) => this.survivor = survivor;

        public abstract string Message { get; }
    }

    internal sealed class SurvivorLeveledUpEventMessage : SurvivorEventMessageBase
    {
        public SurvivorLeveledUpEventMessage(ISurvivor survivor) : base(survivor)
        {
        }

        public override string Message => $"{survivor.Name} has leveled up to {survivor.Level}!";
    }

    internal sealed class SurvivorWoundedEventMessage : SurvivorEventMessageBase
    {
        public SurvivorWoundedEventMessage(ISurvivor survivor) : base(survivor)
        {
        }

        public override string Message => $"{survivor.Name} has been wounded {survivor.Wounds} times!";
    }
}