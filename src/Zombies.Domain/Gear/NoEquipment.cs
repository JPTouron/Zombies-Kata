using Zombies.Domain.Gear.Base;

namespace Zombies.Domain.Gear
{
    internal sealed class NoEquipment : EquipmentBase
    {
        public NoEquipment()
        {
            Name = string.Empty;
        }
    }
}