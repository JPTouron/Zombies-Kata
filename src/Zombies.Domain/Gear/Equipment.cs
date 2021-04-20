using Ardalis.GuardClauses;
using Zombies.Domain.Gear.Base;

namespace Zombies.Domain.Gear
{
    internal sealed class Equipment : EquipmentBase
    {
        public Equipment(string name)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Name = name;
        }
    }
}