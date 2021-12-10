using Godot;
using Godot.Collections;

public class InventoryDisplayer : PanelContainer
{
	[Export] public Inventory Inventory;
	
	private GridContainer _itemsGrid;
	private Label _nameLabel;
	private PackedScene _itemSlotPacked;
	
	public override void _Ready()
	{
		_nameLabel = GetNode<Label>("MarginContainer/VBoxContainer/NameLabel");
		_itemsGrid = GetNode<GridContainer>("MarginContainer/VBoxContainer/ItemsGrid");
		_itemSlotPacked = GD.Load<PackedScene>("res://Inventory/ItemSlot/ItemSlot.tscn");
		
		if (Inventory == null)
		{
			GD.PrintErr("Inventory was not defined in InventoryDisplayer.cs");
			return;
		}
		
		Inventory.Connect("ItemsChanged", this, nameof(UpdateSlots));
		_nameLabel.Text = Inventory.Name;
		CreateSlots();
	}
	
	private void CreateSlots()
	{
		foreach (var item in Inventory.Items)
		{
			var itemSlot = _itemSlotPacked.Instance<ItemSlot>();
			itemSlot.SetDeferred("ItemStats", item);
			itemSlot.Inventory = Inventory;
			_itemsGrid.AddChild(itemSlot);
		}
	}
	
	private void UpdateSlots(Array<ItemStats> items)
	{
		for (var i = 0; i < _itemsGrid.GetChildCount(); ++i)
		{
			var itemSlot = _itemsGrid.GetChild<ItemSlot>(i);
			itemSlot.ItemStats = items[i];
		}
	}
}
