using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextAdventure;

namespace TextAdventure.Tests;

[TestClass]
public class ItemTests
{
    [TestMethod]
    public void ItemName__Returns_Name()
    {
        // Act
        var item = new Item("Sleutel", "Een gouden sleutel.");

        // Assert
        Assert.AreEqual("Sleutel", item.Name);
    }
    
    [TestMethod]
    public void ItemDescription_Returns_Description()
    {
        // Act
        var item = new Item("Sleutel", "Een gouden sleutel.");

        // Assert
        Assert.AreEqual("Een gouden sleutel.", item.Description);
    }
}