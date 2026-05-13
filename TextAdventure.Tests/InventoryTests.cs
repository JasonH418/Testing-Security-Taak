namespace TextAdventure.Tests;

[TestClass]
public class InventoryTests
{

    [TestMethod]
    public void HasItem_WhenInventoryIsEmpty_ReturnsFalse()
    {

        // Arrange
        var inventory = new Inventory();
        
        // Act
        var result = inventory.HasItem("Sleutel");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void AddItem_WhenInventoryContainsItem_ReturnsTrue()
    {
        // Arrange
        var inventory = new Inventory();
        var item = new Item("Sleutel", "Een gouden sleutel.");

        // Act
        inventory.AddItem(item);

        // Assert
        Assert.IsTrue(inventory.HasItem("Sleutel"));

    }

    [TestMethod]

    public void HasItem_ShouldBeCaseInsensitive()

    {
        // Arrange
        var inventory = new Inventory();
        var item = new Item("Sleutel", "Een gouden sleutel.");
        inventory.AddItem(item);
        
        // Act + Assert
        Assert.IsTrue(inventory.HasItem("sleutel"));
        Assert.IsTrue(inventory.HasItem("SLEUTEL"));
        Assert.IsTrue(inventory.HasItem("SlEuTeL"));

    }

    [TestMethod]
    public void GetDisplayList_WhenInventoryIsEmpty_ReturnNiets()
    {

        // Arrange
        var inventory = new Inventory();

        // Act
        var result = inventory.GetDisplayList();

        // Assert
        Assert.AreEqual("Niets", result);

    }

    [TestMethod]

    public void GetDisplayList_WhenInventoryHasOneItem_ReturnsOnlyItemName()
    {
        // Arrange
        var inventory = new Inventory();
        var item = new Item("Sleutel", "Een gouden sleutel.");

        // Act
        inventory.AddItem(item);
        var result = inventory.GetDisplayList();

        // Assert
        StringAssert.Contains(result, "Sleutel");

    }
    
    [TestMethod]
    public void GetDisplayList__WhenInventoryHasMultipleItems_ReturnsMultipleItemNamesSeperatedWithCommas()
    {
        // Arrange
        var inventory = new Inventory();
        var itemOne = new Item("Sleutel", "Een gouden sleutel.");
        var itemTwo = new Item("Koekje", "Een Lekkere koekje.");
        inventory.AddItem(itemOne);
        inventory.AddItem(itemTwo);
        
        // Act
        var result = inventory.GetDisplayList();

        // Assert
        StringAssert.Contains(result, "Sleutel, Koekje");

    }
    
    
}