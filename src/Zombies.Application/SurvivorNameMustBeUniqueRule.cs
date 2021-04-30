using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using Zombies.Domain.BuildingBocks;

namespace Zombies.Application
{
    public class SurvivorNameMustBeUniqueRule : IBusinessRule
    {
        private readonly IReadOnlyCollection<string> listedSurvivors;
        private readonly string name;

        public SurvivorNameMustBeUniqueRule(string name, IReadOnlyCollection<string> listedSurvivors)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            Guard.Against.Null(listedSurvivors, nameof(listedSurvivors));

            this.name = name;
            this.listedSurvivors = listedSurvivors;
        }

        public string Message => $"The survivor name: {name} has already been taken. Please choose a different name";

        public bool IsBroken()
        {
            return listedSurvivors.Any(x => x.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}