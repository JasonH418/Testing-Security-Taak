namespace TextAdventure.Tests;

[TestClass]
public class RoomTests
{
    [TestMethod]
    public void AddExit_ShouldAddExitToRoom()
    {
        // Arrange
        var startRoom = new Room("Start", "Je staat in de startkamer.");
        var nextRoom = new Room("Gang", "Een donkere gang.");

        // Act
        startRoom.AddExit(Direction.n, nextRoom);

        // Assert
        Assert.IsTrue(startRoom.Exits.ContainsKey(Direction.n));
        Assert.AreSame(nextRoom, startRoom.Exits[Direction.n]);
    }

    [TestMethod]
    public void TakeItem_ShouldReturnItem_WhenItemExists()
    {
        // Arrange
        var room = new Room("Schatkamer", "Een kamer met een sleutel.");
        var item = new Item("Sleutel", "Een gouden sleutel.");
        room.AddItem(item);

        // Act
        var result = room.TakeItem("Sleutel");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreSame(item, result);
    }

    [TestMethod]
    public void TakeItem_ShouldRemoveItemFromRoom_WhenItemIsTaken()
    {
        // Arrange
        var room = new Room("Schatkamer", "Een kamer met een sleutel.");
        var item = new Item("Sleutel", "Een gouden sleutel.");
        room.AddItem(item);

        // Act
        var firstResult = room.TakeItem("Sleutel");
        var secondResult = room.TakeItem("Sleutel");

        // Assert
        Assert.IsNotNull(firstResult);
        Assert.IsNull(secondResult);
    }

    [TestMethod]
    public void TakeItem_ShouldReturnNull_WhenItemDoesNotExist()
    {
        // Arrange
        var room = new Room("Lege kamer", "Hier ligt niets.");

        // Act
        var result = room.TakeItem("Sleutel");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void TakeItem_ShouldBeCaseInsensitive()
    {
        // Arrange
        var room = new Room("Schatkamer", "Een kamer met een sleutel.");
        var item = new Item("Sleutel", "Een gouden sleutel.");
        room.AddItem(item);

        // Act
        var result = room.TakeItem("sleutel");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreSame(item, result);
    }
}