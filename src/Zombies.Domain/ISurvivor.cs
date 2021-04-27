using System.Collections.Generic;
using Zombies.Domain.Gear;

namespace Zombies.Domain
{



    public interface ISurvivor
    {
        IReadOnlyCollection<IEquipment> BackPack { get; }

        IHealth.State CurrentState { get; }

        int ExperienceValue { get; }

        IEquipment LeftHandEquip { get; set; }

        XpLevel Level { get; }

        string Name { get; }

        int RemainingActions { get; }

        IEquipment RightHandEquip { get; set; }

        int Wounds { get; }

        void AddEquipment(IEquipment equipment);

        void Kill(Zombie zombie);

        void Wound(int inflictedWounds);
    }
}