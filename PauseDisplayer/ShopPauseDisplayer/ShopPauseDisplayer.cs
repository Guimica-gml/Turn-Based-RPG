using Godot;
using Godot.Collections;

public class ShopPauseDisplayer : PauseDisplayer
{
	[Export] public int ItemsPerPage = 9;
	[Export] public Array<ItemStats> Items = new Array<ItemStats>();
	
	private GridContainer _gridContainer;
	private ShopInfoDisplayer _shopInfoDisplayer;
	
	private PackedScene _shopItemDisplayer = GD.Load<PackedScene>("res://Shops/ShopItemDisplayer/ShopItemDisplayer.tscn");
	
	public override void _Ready()
	{
		_gridContainer = GetNode<GridContainer>("MarginContainer/VBoxContainer/PanelContainer/HBoxContainer/GridContainer");
		_shopInfoDisplayer = GetNode<ShopInfoDisplayer>("MarginContainer/VBoxContainer/PanelContainer/HBoxContainer/ShopInfoDisplayer");
		
		var buttons = new Array<ShopItemDisplayer>(_gridContainer.GetChildren());
		foreach (var button in buttons) button.Connect("ButtonToggled", this, nameof(OnButtonToggled));
		
		CreateShopItemDisplayers();
	}
	
	private void CreateShopItemDisplayers()
	{
		for (var i = 0; i < Items.Count; ++i)
		{
			var shopItemDisplayer = _gridContainer.GetChild<ShopItemDisplayer>(i);
			shopItemDisplayer.ItemStats = Items[i];
		}
	}
	
	private void OnButtonToggled(bool buttonToggled, int buttonIndex)
	{
		if (!buttonToggled)
		{
			_shopInfoDisplayer.ItemStats = null;
			return;
		}
		
		var buttons = new Array<ShopItemDisplayer>(_gridContainer.GetChildren());
		foreach (var button in buttons)
		{
			if (button.GetIndex() != buttonIndex) button.SetButtonPressed(false);
		}
		
		var selectedButton = _gridContainer.GetChild<ShopItemDisplayer>(buttonIndex);
		_shopInfoDisplayer.ItemStats = selectedButton.ItemStats;
	}
}
