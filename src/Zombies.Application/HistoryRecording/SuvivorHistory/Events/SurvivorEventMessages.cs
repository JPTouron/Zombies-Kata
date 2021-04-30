using Zombies.Domain.Gear;
using static Zombies.Application.HistoryRecording.SuvivorHistory.ISurvivorEventsRecorder;

namespace Zombies.Application.HistoryRecording.SuvivorHistory.Events
{
    public sealed class SurvivorAddedToGameEventMessage : SurvivorEventMessageBase
    {
        public SurvivorAddedToGameEventMessage(ISurvivor survivor) : base(survivor)
        {
        }

        public override string Message => $"Survivor {survivor.Name} has joined the game";
    }

    public sealed class SurvivorDiedEventMessage : SurvivorEventMessageBase
    {
        public SurvivorDiedEventMessage(ISurvivor survivor) : base(survivor)
        {
        }

        public override string Message => $"{survivor.Name} has died!";
    }

    public sealed class SurvivorEquipmentEventMessage : SurvivorEventMessageBase
    {
        private readonly IEquipment equipmentAdded;

        public SurvivorEquipmentEventMessage(ISurvivor survivor, IEquipment equipmentAdded) : base(survivor)
        {
            this.equipmentAdded = equipmentAdded;
        }

        public override string Message => $"{survivor.Name} has added {equipmentAdded.Name} to the Backpack. {survivor.BackPackCapacity} slots remain in the backpack";
    }

    public abstract class SurvivorEventMessageBase
    {
        protected readonly ISurvivor survivor;

        protected SurvivorEventMessageBase(ISurvivor survivor) => this.survivor = survivor;

        public abstract string Message { get; }

        public static implicit operator string(SurvivorEventMessageBase msg)
        {

            return msg.Message;
        }
    }

    public sealed class SurvivorGrabbedWeaponEventMessage : SurvivorEventMessageBase
    {
        private readonly IEquipment equipment;
        private readonly Hand hand;
        IEquipment oppositeHandContents;

        public SurvivorGrabbedWeaponEventMessage(ISurvivor survivor, IEquipment equipment, Hand hand) : base(survivor)
        {
            this.equipment = equipment;
            this.hand = hand;
            oppositeHandContents = hand == Hand.Left ? survivor.RightHandEquip : survivor.LeftHandEquip;
        }


        public override string Message => $"{survivor.Name} has grabbed {equipment.Name} with the {hand} hand! The other hand has {oppositeHandContents.Name}";
    }
    public sealed class SurvivorKilledAZombieEventMessage : SurvivorEventMessageBase
    {
        public SurvivorKilledAZombieEventMessage(ISurvivor survivor) : base(survivor)
        {
        }

        public override string Message => $"{survivor.Name} has killed a Zombie!";
    }

    public sealed class SurvivorLeveledUpEventMessage : SurvivorEventMessageBase
    {
        public SurvivorLeveledUpEventMessage(ISurvivor survivor) : base(survivor)
        {
        }

        public override string Message => $"{survivor.Name} has leveled up to {survivor.Level}!";
    }

    public sealed class SurvivorWoundedEventMessage : SurvivorEventMessageBase
    {
        public SurvivorWoundedEventMessage(ISurvivor survivor) : base(survivor)
        {
        }

        public override string Message => $"{survivor.Name} has been wounded {survivor.Wounds} times!";
    }
}