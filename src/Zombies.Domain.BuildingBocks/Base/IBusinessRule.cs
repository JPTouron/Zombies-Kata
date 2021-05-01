namespace Zombies.Domain.BuildingBocks
{
    public interface IBusinessRule : IBusinessRuleCheck
    {
        string Message { get; }
    }

    public interface IBusinessRuleCheck
    {
        bool IsBroken();
    }
}