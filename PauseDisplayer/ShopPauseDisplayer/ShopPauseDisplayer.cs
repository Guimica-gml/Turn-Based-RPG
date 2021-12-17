using Godot;
using Godot.Collections;

public class ShopPauseDisplayer : PauseDisplayer
{
	[Export] public Array<ItemStats> Items = new Array<ItemStats>();
	
	private Label _moneyLabel;
	private GridContainer _gridContainer;
	private ShopInfoDisplayer _shopInfoDisplayer;
	
	private Stats _playerStats = null;
	
	public override void _Ready()
	{
		_moneyLabel = GetNode<Label>("MarginContainer/VBoxContainer/NameLabel/MoneyLabel");
		_gridContainer = GetNode<GridContainer>("MarginContainer/VBoxContainer/PanelContainer/HBoxContainer/GridContainer");
		_shopInfoDisplayer = GetNode<ShopInfoDisplayer>("MarginContainer/VBoxContainer/PanelContainer/HBoxContainer/ShopInfoDisplayer");
		
		_playerStats = GD.Load<Stats>("res://Stats/PlayerStats.tres");
		_playerStats.Connect("MoneyChanged", this, nameof(UpdateMoneyLabel));
		UpdateMoneyLabel(_playerStats.Money);
		
		var buttons = new Array<ShopItemDisplayer>(_gridContainer.GetChildren());
		foreach (var button in buttons) button.Connect("ButtonToggled", this, nameof(OnButtonToggled));
		
		UpdateShopItemDisplayers();
	}
	
	private void UpdateShopItemDisplayers()
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
	
	private void UpdateMoneyLabel(int money)
	{
		_moneyLabel.Text = $"Money: {money.ToString()}";
	}
}
