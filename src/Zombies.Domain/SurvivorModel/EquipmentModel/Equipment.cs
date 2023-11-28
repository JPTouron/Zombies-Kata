using Zombies.Domain.SurvivorModel.EquipmentModel;
using Zombies.Domain.WeaponsModel;

namespace Zombies.Domain;

public partial class Survivor
{
    private class Equipment
    {
        private const int MaximumInHandEquipmentSize = 2;

        private List<IWeapon> inHandEquipment;

        private List<IWeapon> inReserveEquipment;

        public Equipment()
        {
            inHandEquipment = new List<IWeapon>();
            inReserveEquipment = new List<IWeapon>();
            CurrentMaximumInReserveEquipmentSize = 3;
        }

        public enum EquipmentType
        {
            InHand,
            InReserve
        }

        public int CurrentMaximumInReserveEquipmentSize { get; private set; }

        public IReadOnlyCollection<IWeapon> InHandEquipment => inHandEquipment;

        public IReadOnlyCollection<IWeapon> InReserveEquipment => inReserveEquipment;

        public void DecreaseInReserveCapactityByOne()
        {
            DecreaseCurrentMaximumInReserveSize();

            RemoveLastInReserveItem();
        }

        public void AddEquipment(EquipmentType type, IWeapon weapon)
        {
            if (weapon is null)
                throw new ArgumentNullException(nameof(weapon), "The weapon is required and cannot be null");

            if (string.IsNullOrWhiteSpace(weapon.Name))
                throw new ArgumentException(nameof(weapon), "The equipment name is required and cannot be empty");

            AddEquipmentIfNotFull(type, weapon);
        }

        public void EnhanceMeleeWeapons(int damageCountIncrease)
        {
            var tempy = inReserveEquipment.Select(x => (x is IMeleeWeapon) ? new EnhancedWeapon(x, damageCountIncrease) : x);
            inReserveEquipment = tempy.ToList();

            tempy = inHandEquipment.Select(x => (x is IMeleeWeapon) ? new EnhancedWeapon(x, damageCountIncrease) : x);
            inHandEquipment = tempy.ToList();
        }

        public void EnhanceRangedWeapons(int damageCountIncrease)
        {
            var tempy = inReserveEquipment.Select(x => (x is IRangeWeapon) ? new EnhancedWeapon(x, damageCountIncrease) : x);
            inReserveEquipment = tempy.ToList();

            tempy = inHandEquipment.Select(x => (x is IRangeWeapon) ? new EnhancedWeapon(x, damageCountIncrease) : x);
            inHandEquipment = tempy.ToList();
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

        private void AddEquipmentIfNotFull(EquipmentType type, IWeapon weapon)
        {
            if (type == EquipmentType.InHand)
            {
                if (inHandEquipment.Count == MaximumInHandEquipmentSize)
                    throw new EquipmentFullException();

                inHandEquipment.Add(weapon);
            }
            else
            {
                if (inReserveEquipment.Count == CurrentMaximumInReserveEquipmentSize)
                    throw new EquipmentFullException();

                inReserveEquipment.Add(weapon);
            }
        }
    }
}