﻿using AutoFixture;

namespace Zombies.Domain.Tests
{
    internal static class SurvivorProvider
    {
        public static Survivor CreateRandomSurvivor(string? name = null)
        {
            if (name == null)
            {
                name = new Fixture().Create<string>();
            }

            var s = new Survivor(name);
            return s;
        }

    }
}