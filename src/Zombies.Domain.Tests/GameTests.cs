using Xunit;
using Zombies.Domain.GameModel;
using static Zombies.Domain.GameModel.IGame;

namespace Zombies.Domain.Tests;

public class GameTests
{
    [Fact]
    public void WhenGameCreated_ThenHasExpectedState()
    {
        var survivorsInGame = 0;
        var state = GameState.Started;

        var game = Game.Start();

        Assert.Equal(state, game.State);
        Assert.Equal(survivorsInGame, game.SurvivorsInGame);
    }

    [Theory]
    [InlineData("juan")]
    [InlineData("pedro", "pablo")]
    public void GivenAValidGame_WhenASurvivorIsAdded_ThenSurvivorCountIncreases(params string[] survivorNames)
    {
        var survivorsInGame = survivorNames.Length;

        var game = Game.Start();

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

        var game = Game.Start();
        game.AddSurvivor(survivorName2);

        Assert.Throws<SurvivorAlreadyExistsInGameException>(() => game.AddSurvivor(survivorName1));
    }

    [Fact]
    public void GivenAValidGame_WhenAllSurvivorsDie_ThenGameEnds()
    {
        var survivorName1 = "player1";
        var survivorName2 = "player2";
        var expectedGameState = GameState.Ended;

        var game = Game.Start();
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
}