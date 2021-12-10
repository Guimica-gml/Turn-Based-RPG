using Godot;

public class PopupScreen : Control
{
	private VBoxContainer _popupCountainer;
	private PackedScene _popupPacked = GD.Load<PackedScene>("res://Popups/Popup.tscn");
	
	private Inventory _playerInventory = null;
	
	public override void _Ready()
	{
		_popupCountainer = GetNode<VBoxContainer>("PopupContainer");
		
		_playerInventory = GD.Load<Inventory>("res://Inventory/PlayerInventory.tres");
		_playerInventory.Connect("ItemAdded", this, nameof(OnItemAdded));
	}
	
	public void AddPopup(ItemStats item)
	{
		var popup = _popupPacked.Instance<Popup>();
		popup.ItemStats = item;
		_popupCountainer.AddChild(popup);
	}
	
	private void OnItemAdded(ItemStats item)
	{
		AddPopup(item);
	}
}
