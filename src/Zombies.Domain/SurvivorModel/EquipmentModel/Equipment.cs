

using Zombies.Domain.SurvivorModel.EquipmentModel;

namespace Zombies.Domain;

public partial class Survivor
{
    private class Equipment
    {
        private const int MaximumInHandEquipmentSize = 2;

        private List<string> inHandEquipment;

        private List<string> inReserveEquipment;

        public Equipment()
        {
            inHandEquipment = new List<string>();
            inReserveEquipment = new List<string>();
            CurrentMaximumInReserveEquipmentSize = 3;
        }

        public enum EquipmentType
        {
            InHand,
            InReserve
        }

        public int CurrentMaximumInReserveEquipmentSize { get; private set; }

        public IReadOnlyCollection<string> InHandEquipment => inHandEquipment;

        public IReadOnlyCollection<string> InReserveEquipment => inReserveEquipment;

        public void DecreaseInReserveCapactityByOne()
        {
            DecreaseCurrentMaximumInReserveSize();

            RemoveLastInReserveItem();
        }

        public void AddEquipment(EquipmentType type, string equipmentName)
        {
            if (string.IsNullOrWhiteSpace(equipmentName))
                throw new ArgumentException(nameof(equipmentName), "The equipment name is required and cannot be empty");

            AddEquipmentIfNotFull(type, equipmentName);
        }

        private void DecreaseCurrentMaximumInReserveSize()
        {
            if (CurrentMaximumInReserveEquipmentSize > 0)
                CurrentMaximumInReserveEquipmentSize--;
        }

        private void RemoveLastInReserveItem()
        {
            while (inReserveEquipment.Count > 0 && inReserveEquipment.Count > CurrentMaximumInReserveEquipmentSize)
            {
                inReserveEquipment.RemoveAt(inReserveEquipment.Count - 1);
            }
        }

        private void AddEquipmentIfNotFull(EquipmentType type, string equipmentName)
        {
            if (type == EquipmentType.InHand)
            {
                if (inHandEquipment.Count == MaximumInHandEquipmentSize)
                    throw new EquipmentFullException();

                inHandEquipment.Add(equipmentName);
            }
            else
            {
                if (inReserveEquipment.Count == CurrentMaximumInReserveEquipmentSize)
                    throw new EquipmentFullException();

                inReserveEquipment.Add(equipmentName);
            }
        }
    }
}