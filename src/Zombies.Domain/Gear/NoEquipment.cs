using Zombies.Domain.Gear.Base;

namespace Zombies.Domain.Gear
{
    public sealed class NoEquipment : EquipmentBase
    {
        public NoEquipment()
        {
            Name = string.Empty;
        }
    }
}