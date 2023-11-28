using AutoFixture;
using Moq;
using Xunit;
using Zombies.Domain.GameHistory;
using Zombies.Domain.GameModel;
using static Zombies.Domain.GameHistory.GameEvent;
using static Zombies.Domain.GameModel.IGame;

namespace Zombies.Domain.Tests;

public class GameTests
{
    private IFixture fixture;
    private HistoryTrackerFactory historyTrackerFactory;
    private Mock<IClock> clock;

    public GameTests()
    {
        fixture = new Fixture();
        clock = fixture.Create<Mock<IClock>>();

        historyTrackerFactory = new HistoryTrackerFactory(clock.Object);
    }

    [Fact]
    public void WhenGameCreated_ThenHasExpectedState()
    {
        var survivorsInGame = 0;
        var state = GameState.Started;

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());

        Assert.Equal(state, game.State);
        Assert.Equal(survivorsInGame, game.SurvivorsInGame);
    }

    [Theory]
    [InlineData("juan")]
    [InlineData("pedro", "pablo")]
    public void GivenAValidGame_WhenASurvivorIsAdded_ThenSurvivorCountIncreases(params string[] survivorNames)
    {
        var survivorsInGame = survivorNames.Length;

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());

        foreach (var name in survivorNames)
        {
            game.AddSurvivor(name);
        }

        Assert.Equal(survivorsInGame, game.SurvivorsInGame);
    }

    [Fact]
    public void GivenAValidGame_WhenASurvivorIsAddedWithAnAlreadyExistingName_ThenThrows()
    {
        var survivorName1 = "player1";
        var survivorName2 = "player1";

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());
        game.AddSurvivor(survivorName2);

        Assert.Throws<SurvivorAlreadyExistsInGameException>(() => game.AddSurvivor(survivorName1));
    }

    [Fact]
    public void GivenAValidGame_WhenAllSurvivorsDie_ThenGameEnds()
    {
        var survivorName1 = "player1";
        var survivorName2 = "player2";
        var expectedGameState = GameState.Ended;

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());
        game.AddSurvivor(survivorName2);
        game.AddSurvivor(survivorName1);

        //kill player1
        game.WoundSurvivor(survivorName1);
        game.WoundSurvivor(survivorName1);

        //kill player2
        game.WoundSurvivor(survivorName2);
        game.WoundSurvivor(survivorName2);

        var survivor1 = game.GetSurvivor(survivorName1);
        var survivor2 = game.GetSurvivor(survivorName2);

        Assert.True(survivor1.IsDead);
        Assert.True(survivor2.IsDead);
        Assert.Equal(expectedGameState, game.State);
    }

    [Theory]
    [InlineData(ISurvivor.SurvivorLevel.Blue, GameLevel.Blue)]
    [InlineData(ISurvivor.SurvivorLevel.Yellow, GameLevel.Yellow)]
    [InlineData(ISurvivor.SurvivorLevel.Orange, GameLevel.Orange)]
    [InlineData(ISurvivor.SurvivorLevel.Red, GameLevel.Red)]
    public void GivenAValidGame_WhenASurvivorIncreasesLevel_ThenGameHasSameLevelAsSurvivor(ISurvivor.SurvivorLevel survivorLevelIncrease, GameLevel expectedGameLevel)
    {
        var survivorName1 = "player1";

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());
        game.AddSurvivor(survivorName1);

        var survivor1 = game.GetSurvivor(survivorName1);

        while (survivor1.Level < survivorLevelIncrease)
        {
            var zombie = new Zombie();
            while (zombie.IsAlive)
                survivor1.HitZombie(zombie);
        }

        Assert.Equal(expectedGameLevel, game.Level);
    }

    [Fact]
    public void GivenAValidGameAndThreeSurvivors_WhenASurvivorWithMaxLevelDies_ThenGameHasSameLevelAsNextLivingMaxLevelSurvivor()
    {
        var expectedGameLevel = ISurvivor.SurvivorLevel.Orange;

        var survivorName1 = "player1";
        var survivorName2 = "player2";
        var survivorName3 = "player3";

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());

        game.AddSurvivor(survivorName1);
        game.AddSurvivor(survivorName2);
        game.AddSurvivor(survivorName3);

        var survivor1 = game.GetSurvivor(survivorName1);
        var survivor2 = game.GetSurvivor(survivorName2);
        var survivor3 = game.GetSurvivor(survivorName3);

        survivor1.IncreaseSurvivorLevel(ISurvivor.SurvivorLevel.Blue);
        survivor2.IncreaseSurvivorLevel(ISurvivor.SurvivorLevel.Yellow);
        survivor3.IncreaseSurvivorLevel(ISurvivor.SurvivorLevel.Orange);

        Assert.Equal((int)expectedGameLevel, (int)game.Level);
    }

    [Fact]
    public void GivenAValidGame_WhenGameStarts_ThenItGetsRecordedInHistory()
    {
        var expectedEventType = HistoryEventTypes.GameStarted;
        var expectedEventsCount = 1;
        var expectedTimeEvent = DateTime.Now;
        SetupClockDependency(expectedTimeEvent);

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());

        var history = game.History;

        var recordedGameStartedEvent = history.First();

        Assert.Equal(expectedEventsCount, history.Count);
        Assert.Equal(expectedEventType, recordedGameStartedEvent.Type);
        Assert.Equal(expectedTimeEvent, recordedGameStartedEvent.DateTime);
        Assert.Equal("A new game has started!", recordedGameStartedEvent.Message);
    }

    [Fact]
    public void GivenAValidGame_WhenSurvivorIsAddedToTheGame_ThenItGetsRecordedInHistory()
    {
        var expectedEventType = HistoryEventTypes.SurvivorAddedToGame;
        var expectedSurvivorName = "player1";
        var expectedTimeEvent = DateTime.Now;
        SetupClockDependency(expectedTimeEvent);

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());
        game.AddSurvivor(expectedSurvivorName);

        var history = game.History;

        var recordedGameStartedEvent = history.Single(x => x.Type == expectedEventType && x.Message.Contains(expectedSurvivorName));

        Assert.Equal(expectedEventType, recordedGameStartedEvent.Type);
        Assert.Equal($"Survivor {expectedSurvivorName} just joined the game", recordedGameStartedEvent.Message);
        Assert.Equal(expectedTimeEvent, recordedGameStartedEvent.DateTime);
    }

    [Fact]
    public void GivenAValidGame_WhenSurvivorAcquiresHandEquipment_ThenItGetsRecordedInHistory()
    {
        var expectedEventType = HistoryEventTypes.SurvivorAcquiredEquipment;
        var expectedSurvivorName = "player1";
        var expectedEquipmentName = "golf club";
        var expectedTimeEvent = DateTime.Now;
        SetupClockDependency(expectedTimeEvent);

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());
        game.AddSurvivor(expectedSurvivorName);

        var survivor = game.GetSurvivor(expectedSurvivorName);
        survivor.AddHandEquipment(new AdHocWeapon(expectedEquipmentName));

        var history = game.History;

        var recordedGameStartedEvent = history.Single(x => x.Type == expectedEventType && x.Message.Contains(expectedSurvivorName));

        Assert.Equal(expectedEventType, recordedGameStartedEvent.Type);
        Assert.Equal($"Survivor {expectedSurvivorName} acquired {expectedEquipmentName}", recordedGameStartedEvent.Message);
        Assert.Equal(expectedTimeEvent, recordedGameStartedEvent.DateTime);
    }

    [Fact]
    public void GivenAValidGame_WhenSurvivorAcquiresInReserveEquipment_ThenItGetsRecordedInHistory()
    {
        var expectedEventType = HistoryEventTypes.SurvivorAcquiredEquipment;
        var expectedSurvivorName = "player1";
        var expectedEquipmentName = "golf club";
        var expectedTimeEvent = DateTime.Now;
        SetupClockDependency(expectedTimeEvent);

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());
        game.AddSurvivor(expectedSurvivorName);

        var survivor = game.GetSurvivor(expectedSurvivorName);
        survivor.AddInReserveEquipment(new AdHocWeapon(expectedEquipmentName));

        var history = game.History;

        var recordedGameStartedEvent = history.Single(x => x.Type == expectedEventType && x.Message.Contains(expectedSurvivorName));

        Assert.Equal(expectedEventType, recordedGameStartedEvent.Type);
        Assert.Equal($"Survivor {expectedSurvivorName} acquired {expectedEquipmentName}", recordedGameStartedEvent.Message);
        Assert.Equal(expectedTimeEvent, recordedGameStartedEvent.DateTime);
    }

    [Fact]
    public void GivenAValidGame_WhenSurvivorIswounded_ThenItGetsRecordedInHistory()
    {
        var expectedEventType = HistoryEventTypes.SurvivorWasWounded;
        var expectedSurvivorName = "player1";
        var expectedTimeEvent = DateTime.Now;
        var expectedWoundsreceived = 1;
        var expectedRemainingHealth = 1;
        SetupClockDependency(expectedTimeEvent);

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());
        game.AddSurvivor(expectedSurvivorName);

        var survivor = game.GetSurvivor(expectedSurvivorName);
        survivor.InflictWound(expectedWoundsreceived);

        var history = game.History;

        var recordedGameStartedEvent = history.Single(x => x.Type == expectedEventType && x.Message.Contains(expectedSurvivorName));

        Assert.Equal(expectedEventType, recordedGameStartedEvent.Type);
        Assert.Equal($"Survivor {expectedSurvivorName} was wounded {expectedWoundsreceived} times, remaining health: {expectedRemainingHealth}", recordedGameStartedEvent.Message);
        Assert.Equal(expectedTimeEvent, recordedGameStartedEvent.DateTime);
    }

    [Fact]
    public void GivenAValidGame_WhenSurvivorDies_ThenItGetsRecordedInHistory()
    {
        var expectedEventType = HistoryEventTypes.SurvivorDied;
        var expectedSurvivorName = "player1";
        var expectedTimeEvent = DateTime.Now;
        SetupClockDependency(expectedTimeEvent);

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());
        game.AddSurvivor(expectedSurvivorName);

        var survivor = game.GetSurvivor(expectedSurvivorName);
        survivor.Kill();

        var history = game.History;

        var recordedGameStartedEvent = history.Single(x => x.Type == expectedEventType && x.Message.Contains(expectedSurvivorName));

        Assert.Equal(expectedEventType, recordedGameStartedEvent.Type);
        Assert.Equal($"Survivor {expectedSurvivorName} has died", recordedGameStartedEvent.Message);
        Assert.Equal(expectedTimeEvent, recordedGameStartedEvent.DateTime);
    }

    [Theory]
    [InlineData(ISurvivor.SurvivorLevel.Yellow)]
    [InlineData(ISurvivor.SurvivorLevel.Orange)]
    [InlineData(ISurvivor.SurvivorLevel.Red)]
    public void GivenAValidGame_WhenSurvivorLevelsUp_ThenItGetsRecordedInHistory(ISurvivor.SurvivorLevel expectedLevel)
    {
        var expectedEventType = HistoryEventTypes.SurvivorLeveledUp;
        var expectedSurvivorName = "player1";
        var expectedTimeEvent = DateTime.Now;

        SetupClockDependency(expectedTimeEvent);

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());
        game.AddSurvivor(expectedSurvivorName);

        var survivor = game.GetSurvivor(expectedSurvivorName);
        survivor.IncreaseSurvivorLevel(expectedLevel);

        var history = game.History;

        var recordedGameStartedEvent = history.Single(x => x.Type == expectedEventType && x.Message.Contains(expectedLevel.ToString()));

        Assert.Equal(expectedEventType, recordedGameStartedEvent.Type);
        Assert.Equal($"Survivor {expectedSurvivorName} has leveled up to {expectedLevel} level!", recordedGameStartedEvent.Message);
        Assert.Equal(expectedTimeEvent, recordedGameStartedEvent.DateTime);
    }

    [Theory]
    [InlineData(ISurvivor.SurvivorLevel.Yellow, GameLevel.Yellow)]
    [InlineData(ISurvivor.SurvivorLevel.Orange, GameLevel.Orange)]
    [InlineData(ISurvivor.SurvivorLevel.Red, GameLevel.Red)]
    public void GivenAValidGame_WhenGameLevelsUp_ThenItGetsRecordedInHistory(ISurvivor.SurvivorLevel desiredSurvivorLevel, GameLevel expectedGameLevel)
    {
        var expectedEventType = HistoryEventTypes.GameLeveledUp;
        var expectedSurvivorName = "player1";
        var expectedTimeEvent = DateTime.Now;

        SetupClockDependency(expectedTimeEvent);

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());
        game.AddSurvivor(expectedSurvivorName);

        var survivor = game.GetSurvivor(expectedSurvivorName);
        survivor.IncreaseSurvivorLevel(desiredSurvivorLevel);

        var history = game.History;

        var recordedGameLeveledUpEvent = history.Single(x => x.Type == expectedEventType && x.Message.Contains(expectedGameLevel.ToString()));

        Assert.Equal(expectedEventType, recordedGameLeveledUpEvent.Type);
        Assert.Equal($"Game reached a new level: {expectedGameLevel}!", recordedGameLeveledUpEvent.Message);
        Assert.Equal(expectedTimeEvent, recordedGameLeveledUpEvent.DateTime);
    }

    [Fact]
    public void GivenAValidGame_WhenAllSurvivorsDieAndGameEnds_ThenItGetsRecordedInHistory()
    {
        var survivorName1 = "player1";
        var survivorName2 = "player2";
        var expectedEventType = HistoryEventTypes.GameEnded;
        var expectedTimeEvent = DateTime.Now;

        SetupClockDependency(expectedTimeEvent);

        var game = Game.Start(historyTrackerFactory.CreateHistoryTracker());
        game.AddSurvivor(survivorName2);
        game.AddSurvivor(survivorName1);

        //kill player1
        game.WoundSurvivor(survivorName1);
        game.WoundSurvivor(survivorName1);

        //kill player2
        game.WoundSurvivor(survivorName2);
        game.WoundSurvivor(survivorName2);

        var history = game.History;

        var recordedGameEndedEvent = history.Single(x => x.Type == expectedEventType);

        Assert.Equal(expectedEventType, recordedGameEndedEvent.Type);
        Assert.Equal($"Game Ended, all survivors have died!", recordedGameEndedEvent.Message);
        Assert.Equal(expectedTimeEvent, recordedGameEndedEvent.DateTime);
    }

    private void SetupClockDependency(DateTime mockedNow)
    {
        clock.SetupGet(x => x.UtcNow).Returns(mockedNow);
    }
}