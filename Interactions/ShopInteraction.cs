using Godot;
using Godot.Collections;

public class ShopInteraction : Interaction
{
	[Export] public Array<ItemStats> Items = new Array<ItemStats>();
	
	public override void OnTrigger()
	{
		Global.ShopManager.Connect("ShopClosed", this, nameof(OnShopClosed));
		Global.ShopManager.OpenShop(Items);
	}
	
	public override bool CanTrigger()
	{
		return true;
	}
	
	public override void OnEnd()
	{
		Global.ShopManager.Disconnect("ShopClosed", this, nameof(OnShopClosed));
	}
	
	public override Texture GetIcon()
	{
		return GD.Load<Texture>("res://Interactions/Icons/ExclamationMark.png");;
	}
	
	private void OnShopClosed()
	{
		EmitSignal(nameof(Ended));
	}
}
