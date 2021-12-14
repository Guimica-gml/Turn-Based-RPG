using Godot;

public class PopupScreen : Control
{
	private VBoxContainer _popupCountainer;
	private PackedScene _itemPopupPacked = GD.Load<PackedScene>("res://Popups/ItemPopup/ItemPopup.tscn");
	
	private Inventory _playerInventory = null;
	
	public override void _Ready()
	{
		_popupCountainer = GetNode<VBoxContainer>("PopupContainer");
		
		_playerInventory = GD.Load<Inventory>("res://Inventory/PlayerInventory.tres");
		_playerInventory.Connect("ItemAdded", this, nameof(OnItemAdded));
	}
	
	public void AddPopup(GenericPopup popup)
	{
		_popupCountainer.AddChild(popup);
	}
	
	private void OnItemAdded(ItemStats item)
	{
		var itemPopup = _itemPopupPacked.Instance<ItemPopup>();
		itemPopup.ItemStats = item;
		AddPopup(itemPopup);
	}
}
