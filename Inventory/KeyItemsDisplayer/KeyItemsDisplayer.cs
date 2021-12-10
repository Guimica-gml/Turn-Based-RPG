using Godot;
using Godot.Collections;

public class KeyItemsDisplayer : PanelContainer
{
	[Export] private Inventory _inventory;
	
	private VBoxContainer _keyItemsContainer;
	
	public override void _Ready()
	{
		_keyItemsContainer = GetNode<VBoxContainer>("MarginContainer/VBoxContainer/KeyitemsContainer");
		_inventory.Connect("KeyItemsChanged", this, nameof(UpdateSlots));
		UpdateSlots(_inventory.KeyItems);
	}
	
	private void UpdateSlots(Array<ItemStats> keyItems)
	{
		var keyItemSlots = new Array<KeyItemSlot>(_keyItemsContainer.GetChildren());
		
		for (var i = 0; i < keyItems.Count; ++i)
		{
			keyItemSlots[i].ItemStats = keyItems[i];
		}
	}
}
