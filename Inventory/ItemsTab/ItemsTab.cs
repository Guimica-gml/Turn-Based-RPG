using Godot;
using Godot.Collections;

public class ItemsTab : Tabs
{
	[Export] private bool _displayKeyItems = false;
	[Export] private NodePath _itemInfoDisplayerPath = "";

	public Inventory Inventory
	{
		get => _inventory;
		set
		{
			DeleteSlots();
			_inventory = value;
			CreateSlots();
		}
	}
	[Export] private Inventory _inventory = null;

	public ItemInfoDisplayer ItemInfoDisplayer;

	private GridContainer _itemsGrid;
	private PackedScene _itemSlotPacked = GD.Load<PackedScene>("res://Inventory/ItemSlot/ItemSlot.tscn");

	public override void _Ready()
	{
		_itemsGrid = GetNode<GridContainer>("MarginContainer/ScrollContainer/ItemsGrid");
		if (_itemInfoDisplayerPath != "") ItemInfoDisplayer = GetNode<ItemInfoDisplayer>(_itemInfoDisplayerPath);
	}

	private void CreateSlots()
	{
		var items = (!_displayKeyItems) ? Inventory.Items : Inventory.KeyItems;

		foreach (var item in items)
		{
			var itemSlot = _itemSlotPacked.Instance<ItemSlot>();
			itemSlot.SetDeferred("ItemStats", item);
			itemSlot.Inventory = Inventory;
			itemSlot.ItemInfoDisplayer = ItemInfoDisplayer;
			_itemsGrid.AddChild(itemSlot);
		}

		if (!_displayKeyItems && !Inventory.IsConnected("ItemsChanged", this, nameof(UpdateSlots)))
		{
			Inventory.Connect("ItemsChanged", this, nameof(UpdateSlots));
		}
	}

	private void DeleteSlots()
	{
		var itemSlots = new Array<ItemSlot>(_itemsGrid.GetChildren());

		foreach (var itemSlot in itemSlots)
		{
			itemSlot.QueueFree();
		}

		if (Inventory != null && Inventory.IsConnected("ItemsChanged", this, nameof(UpdateSlots)))
		{
			Inventory.Connect("ItemsChanged", this, nameof(UpdateSlots));
		}
	}

	private void UpdateSlots(Array<ItemStats> items)
	{
		for (var i = 0; i < items.Count; ++i)
		{
			var itemSlot = _itemsGrid.GetChild<ItemSlot>(i);
			itemSlot.ItemStats = Inventory.Items[i];
		}
	}
}
