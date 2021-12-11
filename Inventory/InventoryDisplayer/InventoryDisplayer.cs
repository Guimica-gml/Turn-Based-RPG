using Godot;

public class InventoryDisplayer : TabContainer
{
	[Export] public Inventory Inventory;
	[Export] private NodePath _itemInfoDisplayerPath = "";
	
	private ItemsTab _itemsTab;
	private ItemsTab _keyItemsTab;
	
	public override void _Ready()
	{
		_itemsTab = GetNode<ItemsTab>("Items");
		_keyItemsTab = GetNode<ItemsTab>("KeyItems");
		
		if (Inventory == null)
		{
			GD.PrintErr("Inventory was not defined in InventoryDisplayer.cs");
			return;
		}
		
		_itemsTab.ItemInfoDisplayer = GetNode<ItemInfoDisplayer>(_itemInfoDisplayerPath);
		_itemsTab.Inventory = Inventory;
		
		_keyItemsTab.ItemInfoDisplayer = GetNode<ItemInfoDisplayer>(_itemInfoDisplayerPath);
		_keyItemsTab.Inventory = Inventory;
	}
}
