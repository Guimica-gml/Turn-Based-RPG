using Godot;
using Godot.Collections;

public class InventoryContainer : TabContainer
{
	[Export] public Inventory Inventory;
	[Export] private NodePath _itemInfoDisplayerPath = "";

	public override void _Ready()
	{
		if (Inventory == null)
		{
			GD.PrintErr("Inventory was not defined in InventoryDisplayer.cs");
			return;
		}

		var tabs = new Array<ItemsTab>(GetChildren());

		foreach (var tab in tabs)
		{
			tab.ItemInfoDisplayer = GetNode<ItemInfoDisplayer>(_itemInfoDisplayerPath);
			tab.Inventory = Inventory;
		}
	}
}
