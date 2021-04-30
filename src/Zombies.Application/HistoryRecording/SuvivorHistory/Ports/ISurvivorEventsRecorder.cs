using Zombies.Domain.Gear;

namespace Zombies.Application.HistoryRecording.SuvivorHistory
{
    public interface ISurvivorEventsRecorder
    {
        public enum Hand { Right, Left}

        void GrabbedEquipment(ISurvivor survivor, IEquipment equipment, Hand hand);
        void AddedEquipment(ISurvivor survivor, IEquipment equipmentAdded);

        void Died(ISurvivor survivor);

        void KilledAZombie(ISurvivor survivor);

        void LeveledUp(ISurvivor survivor);

        void Wounded(ISurvivor survivor);
    }
}