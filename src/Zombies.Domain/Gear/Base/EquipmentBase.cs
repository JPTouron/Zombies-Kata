using Zombies.Domain.BuildingBocks;

namespace Zombies.Domain.Gear.Base
{

    /// <summary>
    /// equipment is a value object, it must not be persisted as i'd have diff equips each haveing diff names and diff attack values or whateve, 
    /// and they all are loaded in mem
    /// a survivor then would have a list of equipment associated to it, just as a some sort of id. ie: surivorA has a golf club and a base ball bat as their equipment
    /// we could then load that from the db and instantiate a survivor with it
    /// CONCLUSION: THIS IMPLIES THAT AN LIST OF EQUIP HAS TO BE LOADED AS A CONSTRUCTOR ON THE INVENTORY HANDLER OR SMETHING
    /// </summary>
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