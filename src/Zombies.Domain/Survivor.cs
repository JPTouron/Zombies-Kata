using Ardalis.GuardClauses;
using System.Collections.Generic;

namespace Zombies.Domain
{
    public class Survivor
    {
        private List<string> equipmentInHand;

        private List<string> equipmentInReserve;

        public Survivor(string name)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));

            Name = name;
            Wounds = 0;
            AvailableActionsInTurn = 3;
            equipmentInHand = new List<string>();
            equipmentInReserve = new List<string>();
        }

        public string Name { get; }

        public int Wounds { get; private set; }

        public int AvailableActionsInTurn { get; }

        public int InHand => equipmentInHand.Count;

        public int InReserve => equipmentInReserve.Count;

        public bool IsAlive => Wounds < 2;

        public void AddEquipment(string equipmentName)
        {
            Guard.Against.NullOrEmpty(equipmentName, nameof(equipmentName));

            if (InHand == 2 && InReserve < 3)
            {
                equipmentInReserve.Add(equipmentName);
            }

            if (InHand < 2)
            {
                equipmentInHand.Add(equipmentName);
            }
        }

        public void Wound()
        {
            Wounds++;

            if (IsAlive)
            {
                if (InReserve > 0)
                {
                    equipmentInReserve.RemoveAt(InReserve - 1);
                }
                if (InReserve == 0 && InHand > 0)
                {
                    equipmentInHand.RemoveAt(InHand - 1);
                }
            }
        }
    }
}