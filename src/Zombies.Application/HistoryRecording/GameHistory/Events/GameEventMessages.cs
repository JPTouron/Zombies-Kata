using Zombies.Domain;

namespace Zombies.Application.HistoryRecording.GameHistory.Events
{
    public abstract class GameEventMessageBase
    {
        public abstract string Message { get; }

        public static implicit operator string(GameEventMessageBase msg)
        {
            return msg.Message;
        }
    }

    public sealed class GameFinishedEventMessage : GameEventMessageBase
    {
        public override string Message => $"Game has ended, no more survivors are left!";
    }

    public sealed class GameLeveledUpEventMessage : GameEventMessageBase
    {
        private readonly XpLevel level;

        public GameLeveledUpEventMessage(XpLevel level)
        {
            this.level = level;
        }

        public override string Message => $"Game reached {level} level!";
    }

    public sealed class GameStartedEventMessage : GameEventMessageBase
    {
        public override string Message => $"A new game has started";
    }
}