namespace TextAdventure.Tests;

[TestClass]
public class GameIntegrationTests
{    
    [TestMethod]
    public void Move_From_Start_To_Schatkamer_Integration_Test()
    {
        // Arrange
        var world = GameSetup.CreateWorld();

        // Act
        world.Move(Direction.e);

        // Assert
        Assert.AreEqual("Schatkamer", world.CurrentRoom.Name);
    }

    [TestMethod]
    public void Take_Item_From_Room_Adds_To_Inventory_Integration_Test()
    {
        // Arrange
        var world = GameSetup.CreateWorld();
        world.Move(Direction.e);

        // Act
        var item = world.CurrentRoom.TakeItem("Sleutel");
        if (item != null) world.Inventory.AddItem(item);

        // Assert
        Assert.IsTrue(world.Inventory.HasItem("Sleutel"));
    }

    [TestMethod]
    public void Move_To_Exit_Without_Key_Is_Blocked_Integration_Test()
    {
        // Arrange
        var world = GameSetup.CreateWorld();

        // Act
        world.Move(Direction.n);

        // Assert
        Assert.AreEqual("Start", world.CurrentRoom.Name);
        Assert.IsFalse(world.IsWon);
    }

    [TestMethod]
    public void Move_To_Exit_With_Key_Wins_Game_Integration_Test()
    {
        // Arrange
        var world = GameSetup.CreateWorld();
        world.Move(Direction.e);
        var item = world.CurrentRoom.TakeItem("Sleutel");
        if (item != null) world.Inventory.AddItem(item);
        world.Move(Direction.w);

        // Act
        world.Move(Direction.n);

        // Assert
        Assert.IsTrue(world.IsWon);
    }

    [TestMethod]
    public void Move_To_Deadly_Room_Sets_GameOver_Integration_Test()
    {
        // Arrange
        var world = GameSetup.CreateWorld();

        // Act
        world.Move(Direction.w);

        // Assert
        Assert.IsTrue(world.IsGameOver);
    }

    [TestMethod]
    public void Fight_Without_Sword_Sets_GameOver_Integration_Test()
    {
        // Arrange
        var world = GameSetup.CreateWorld();
        world.Move(Direction.s);
        world.Move(Direction.s);

        // Act
        world.Fight();

        // Assert
        Assert.IsTrue(world.IsGameOver);
    }

    [TestMethod]
    public void Fight_With_Sword_Kills_Monster_Integration_Test()
    {
        // Arrange
        var world = GameSetup.CreateWorld();
        world.Move(Direction.s);
        var zwaard = world.CurrentRoom.TakeItem("Zwaard");
        if (zwaard != null) world.Inventory.AddItem(zwaard);
        world.Move(Direction.s);

        // Act
        world.Fight();

        // Assert
        Assert.IsFalse(world.CurrentRoom.MonsterAlive);
        Assert.IsFalse(world.IsGameOver);
    }

    [TestMethod]
    public void Move_Away_From_Monster_Sets_GameOver_Integration_Test()
    {
        // Arrange
        var world = GameSetup.CreateWorld();
        world.Move(Direction.s);
        world.Move(Direction.s);

        // Act
        world.Move(Direction.n);

        // Assert
        Assert.IsTrue(world.IsGameOver);
    }

    [TestMethod]
    public void Admin_Can_Move_To_Exit_Without_Key_Integration_Test()
    {
        // Arrange
        var world = GameSetup.CreateWorld(isAdmin: true);

        // Act
        world.Move(Direction.n);

        // Assert
        Assert.IsTrue(world.IsWon);
    }

    [TestMethod]
    public void Full_Game_Flow_Integration_Test()
    {
        // Arrange
        var world = GameSetup.CreateWorld();

        // Act
        world.Move(Direction.s);
        var zwaard = world.CurrentRoom.TakeItem("Zwaard");
        if (zwaard != null) world.Inventory.AddItem(zwaard);

        world.Move(Direction.s);
        world.Fight();
        world.Move(Direction.n);
        world.Move(Direction.n);

        world.Move(Direction.e);
        var sleutel = world.CurrentRoom.TakeItem("Sleutel");
        if (sleutel != null) world.Inventory.AddItem(sleutel);

        world.Move(Direction.w);
        world.Move(Direction.n);

        // Assert
        Assert.IsTrue(world.IsWon);
        Assert.IsFalse(world.IsGameOver);
    }
}