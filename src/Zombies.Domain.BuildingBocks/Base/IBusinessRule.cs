namespace Zombies.Domain.BuildingBocks
{
    public interface IBusinessRule
    {
        string Message { get; }

        bool IsBroken();
    }
}