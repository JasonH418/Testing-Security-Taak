using FluentAssertions;
using Reqnroll;

namespace TextAdventure.Specs.StepDefinitions;

[Binding]
public sealed class GameFlowStepDefinitions
{
    private Building _world = null!;

    [Given("een nieuw spel is gestart")]
    public void GivenEenNieuwSpelIsGestart()
    {
        _world = GameSetup.CreateWorld();
    }

    [When("de speler naar het noorden gaat")]
    public void WhenDeSpelerNaarHetNoordenGaat()
    {
        _world.Move(Direction.n);
    }

    [When("de speler naar het oosten gaat")]
    public void WhenDeSpelerNaarHetOostenGaat()
    {
        _world.Move(Direction.e);
    }

    [When("de speler naar het zuiden gaat")]
    public void WhenDeSpelerNaarHetZuidenGaat()
    {
        _world.Move(Direction.s);
    }

    [When("de speler naar het westen gaat")]
    public void WhenDeSpelerNaarHetWestenGaat()
    {
        _world.Move(Direction.w);
    }

    [When("de speler het item {string} oppakt")]
    public void WhenDeSpelerHetItemOppakt(string itemName)
    {
        var item = _world.CurrentRoom.TakeItem(itemName);

        if (item is not null)
        {
            _world.Inventory.AddItem(item);
        }
    }

    [When("de speler vecht")]
    public void WhenDeSpelerVecht()
    {
        _world.Fight();
    }

    [Then("staat de speler in de kamer {string}")]
    public void ThenStaatDeSpelerInDeKamer(string expectedRoomName)
    {
        _world.CurrentRoom.Name.Should().Be(expectedRoomName);
    }

    [Then("heeft de speler het item {string}")]
    public void ThenHeeftDeSpelerHetItem(string itemName)
    {
        _world.Inventory.HasItem(itemName).Should().BeTrue();
    }

    [Then("is het spel afgelopen")]
    public void ThenIsHetSpelAfgelopen()
    {
        _world.IsGameOver.Should().BeTrue();
    }

    [Then("is het spel niet afgelopen")]
    public void ThenIsHetSpelNietAfgelopen()
    {
        _world.IsGameOver.Should().BeFalse();
    }

    [Then("heeft de speler gewonnen")]
    public void ThenHeeftDeSpelerGewonnen()
    {
        _world.IsWon.Should().BeTrue();
    }

    [Then("heeft de speler nog niet gewonnen")]
    public void ThenHeeftDeSpelerNogNietGewonnen()
    {
        _world.IsWon.Should().BeFalse();
    }

    [Then("heeft de speler niet gewonnen")]
    public void ThenHeeftDeSpelerNietGewonnen()
    {
        _world.IsWon.Should().BeFalse();
    }

    [Then("leeft het monster niet meer")]
    public void ThenLeeftHetMonsterNietMeer()
    {
        _world.CurrentRoom.MonsterAlive.Should().BeFalse();
    }
}