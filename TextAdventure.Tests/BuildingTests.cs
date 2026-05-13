namespace TextAdventure.Tests;

[TestClass]
public class BuildingTests
{
    [TestMethod]
    public void Constructor_ShouldStartInGivenRoom()
    {
        // Arrange
        var start = new Room("Start", "Je staat in het midden van een stoffige kerker.");

        // Act
        var building = new Building(start);

        // Assert
        Assert.AreSame(start, building.CurrentRoom);
        Assert.IsFalse(building.IsGameOver);
        Assert.IsFalse(building.IsWon);
    }

    [TestMethod]
    public void Move_ShouldMoveToTargetRoom_WhenExitExists()
    {
        // Arrange
        var start = new Room("Start", "Je staat in het midden van een stoffige kerker.");
        var treasureRoom = new Room("Schatkamer", "Een kleine kamer met een kist op de grond.");
        start.AddExit(Direction.e, treasureRoom);
        var building = new Building(start);

        // Act
        building.Move(Direction.e);

        // Assert
        Assert.AreSame(treasureRoom, building.CurrentRoom);
        Assert.IsFalse(building.IsGameOver);
        Assert.IsFalse(building.IsWon);
    }

    [TestMethod]
    public void Move_ShouldStayInCurrentRoom_WhenExitDoesNotExist()
    {
        // Arrange
        var start = new Room("Start", "Je staat in het midden van een stoffige kerker.");
        var building = new Building(start);

        // Act
        building.Move(Direction.e);

        // Assert
        Assert.AreSame(start, building.CurrentRoom);
        Assert.IsFalse(building.IsGameOver);
        Assert.IsFalse(building.IsWon);
    }

    [TestMethod]
    public void Move_ShouldSetGameOver_WhenTargetRoomIsDeadly()
    {
        // Arrange
        var start = new Room("Start", "Je staat in het midden van een stoffige kerker.");
        var deadlyRoom = new Room("Dodelijke Gang","Zodra je de kamer binnenstapt, valt het plafond naar beneden!")
        {
            IsDeadly = true
        };
        start.AddExit(Direction.w, deadlyRoom);
        var building = new Building(start);

        // Act
        building.Move(Direction.w);

        // Assert
        Assert.AreSame(deadlyRoom, building.CurrentRoom);
        Assert.IsTrue(building.IsGameOver);
        Assert.IsFalse(building.IsWon);
    }

    [TestMethod]
    public void Move_ShouldNotEnterRoom_WhenRequiredItemIsMissing()
    {
        // Arrange
        var start = new Room("Start", "Je staat in het midden van een stoffige kerker.");
        var exitRoom = new Room("De Uitgang","Gefeliciteerd! Je hebt de weg naar buiten gevonden.")
        {
            IsWin = true,
            RequiredItem = "Sleutel"
        };
        start.AddExit(Direction.n, exitRoom);
        var building = new Building(start);

        // Act
        building.Move(Direction.n);

        // Assert
        Assert.AreSame(start, building.CurrentRoom);
        Assert.IsFalse(building.IsWon);
        Assert.IsFalse(building.IsGameOver);
    }

    [TestMethod]
    public void Move_ShouldEnterRoom_WhenRequiredItemExists()
    {
        // Arrange
        var start = new Room("Start", "Je staat in het midden van een stoffige kerker.");
        var exitRoom = new Room("De Uitgang","Gefeliciteerd! Je hebt de weg naar buiten gevonden.")
        {
            RequiredItem = "Sleutel"
        };
        start.AddExit(Direction.n, exitRoom);
        var building = new Building(start);
        building.Inventory.AddItem(new Item("Sleutel", "Een gouden sleutel."));

        // Act
        building.Move(Direction.n);

        // Assert
        Assert.AreSame(exitRoom, building.CurrentRoom);
        Assert.IsFalse(building.IsGameOver);
    }

    [TestMethod]
    public void Move_ShouldSetIsWon_WhenTargetRoomIsWinRoomAndRequiredItemExists()
    {
        // Arrange
        var start = new Room("Start", "Je staat in het midden van een stoffige kerker.");
        var exitRoom = new Room("De Uitgang", "Gefeliciteerd! Je hebt de weg naar buiten gevonden.")
        {
            IsWin = true,
            RequiredItem = "Sleutel"
        };
        start.AddExit(Direction.n, exitRoom);
        var building = new Building(start);
        building.Inventory.AddItem(new Item("Sleutel", "Een gouden sleutel."));

        // Act
        building.Move(Direction.n);

        // Assert
        Assert.AreSame(exitRoom, building.CurrentRoom);
        Assert.IsTrue(building.IsWon);
        Assert.IsFalse(building.IsGameOver);
    }

    [TestMethod]
    public void Move_ShouldSetGameOver_WhenTryingToLeaveRoomWithAliveMonster()
    {
        // Arrange
        var monsterRoom = new Room("Monsterkamer", "Een donker hol dat naar rottend vlees stinkt.")
        {
            MonsterAlive = true
        };
        var basement = new Room("Kelder", "Het is hier koud en vochtig.");
        monsterRoom.AddExit(Direction.n, basement);
        var building = new Building(monsterRoom);

        // Act
        building.Move(Direction.n);

        // Assert
        Assert.AreSame(monsterRoom, building.CurrentRoom);
        Assert.IsTrue(building.IsGameOver);
        Assert.IsFalse(building.IsWon);
    }

    [TestMethod]
    public void Move_ShouldAllowLeavingMonsterRoom_WhenMonsterIsDead()
    {
        // Arrange
        var monsterRoom = new Room(
            "Monsterkamer",
            "Een donker hol dat naar rottend vlees stinkt."
        )
        {
            MonsterAlive = false
        };
        var basement = new Room("Kelder", "Het is hier koud en vochtig.");
        monsterRoom.AddExit(Direction.n, basement);
        var building = new Building(monsterRoom);

        // Act
        building.Move(Direction.n);

        // Assert
        Assert.AreSame(basement, building.CurrentRoom);
        Assert.IsFalse(building.IsGameOver);
        Assert.IsFalse(building.IsWon);
    }

    [TestMethod]
    public void Fight_ShouldSetGameOver_WhenMonsterAliveAndPlayerHasNoSword()
    {
        // Arrange
        var monsterRoom = new Room("Monsterkamer", "Een donker hol dat naar rottend vlees stinkt.")
        {
            MonsterAlive = true
        };
        var building = new Building(monsterRoom);

        // Act
        building.Fight();

        // Assert
        Assert.IsTrue(building.IsGameOver);
        Assert.IsTrue(building.CurrentRoom.MonsterAlive);
        Assert.IsFalse(building.IsWon);
    }

    [TestMethod]
    public void Fight_ShouldKillMonster_WhenMonsterAliveAndPlayerHasSword()
    {
        // Arrange
        var monsterRoom = new Room("Monsterkamer", "Een donker hol dat naar rottend vlees stinkt.")
        {
            MonsterAlive = true
        };
        var building = new Building(monsterRoom);
        building.Inventory.AddItem(new Item("Zwaard", "Een vlijmscherp zwaard."));

        // Act
        building.Fight();

        // Assert
        Assert.IsFalse(building.CurrentRoom.MonsterAlive);
        Assert.IsFalse(building.IsGameOver);
        Assert.IsFalse(building.IsWon);
    }

    [TestMethod]
    public void Fight_ShouldDoNothing_WhenCurrentRoomHasNoMonster()
    {
        // Arrange
        var basement = new Room("Kelder", "Het is hier koud en vochtig.");
        var building = new Building(basement);

        // Act
        building.Fight();

        // Assert
        Assert.AreSame(basement, building.CurrentRoom);
        Assert.IsFalse(building.CurrentRoom.MonsterAlive);
        Assert.IsFalse(building.IsGameOver);
        Assert.IsFalse(building.IsWon);
    }
}