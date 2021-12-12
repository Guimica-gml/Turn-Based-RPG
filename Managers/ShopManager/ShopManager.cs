using Godot;
using Godot.Collections;

public class ShopManager : Node
{
	[Signal] private delegate void ShopOpened();
	[Signal] private delegate void ShopClosed();
	
	private PackedScene _shopDisplayerPacked = GD.Load<PackedScene>("res://PauseDisplayer/ShopPauseDisplayer/ShopPauseDisplayer.tscn");
	
	public void OpenShop(Array<ItemStats> items)
	{
		var shopDisplayer = _shopDisplayerPacked.Instance<ShopPauseDisplayer>();
		shopDisplayer.Items = items;
		shopDisplayer.Connect("Close", this, nameof(OnShopClosed));
		
		Global.Manager.PauseGame(shopDisplayer);
		EmitSignal(nameof(ShopOpened));
	}
	
	private void OnShopClosed()
	{
		EmitSignal(nameof(ShopClosed));
	}
}
