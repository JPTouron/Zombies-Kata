using Zombies.Domain;

namespace Zombies.Application.History.EventMessages
{
    internal abstract class GameEventMessage
    {
        public abstract string Message { get; }
    }

    internal sealed class GameFinishedEventMessage : GameEventMessage
    {
        public override string Message => $"Game has ended, no more survivors are left!";
    }

    internal sealed class GameLeveledUpEventMessage : GameEventMessage
    {
        private readonly XpLevel level;

        public GameLeveledUpEventMessage(XpLevel level)
        {
            this.level = level;
        }

        public override string Message => $"Game reached {level} level!";
    }

    internal sealed class GameStartedEventMessage : GameEventMessage
    {
        public override string Message => $"A new game has started";
    }
}