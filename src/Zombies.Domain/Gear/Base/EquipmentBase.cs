using Zombies.Domain.BuildingBocks;

namespace Zombies.Domain.Gear.Base
{
    public abstract class EquipmentBase : ValueObject<EquipmentBase>, IEquipment
    {
        public string Name { get; protected set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        protected override bool InternalEquals(EquipmentBase other)
        {
            return Name.Equals(other.Name);
        }
    }
}