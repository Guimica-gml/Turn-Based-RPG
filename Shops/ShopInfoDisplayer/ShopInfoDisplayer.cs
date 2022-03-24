using Godot;

public class ShopInfoDisplayer : CenterContainer
{
	public ItemStats ItemStats
	{
		get => _itemStats;
		set
		{
			_itemStats = value;
			UpdateInfo();
		}
	}
	[Export] private ItemStats _itemStats = null;

	private Label _nameLabel;
	private Label _descriptionLabel;
	private Label _priceLabel;
	private TextureRect _textureRect;
	private VBoxContainer _vBoxContainer;

	private Inventory _playerInventory;
	private Stats _playerStats;

	private PackedScene _textPopupPacked = GD.Load<PackedScene>("res://Popups/TextPopup/TextPopup.tscn");

	public override void _Ready()
	{
		_playerInventory = GD.Load<Inventory>("res://Inventory/PlayerInventory.tres");
		_playerStats = GD.Load<Stats>("res://Stats/PlayerStats.tres");

		_nameLabel = GetNode<Label>("PanelContainer/VBoxContainer/NameLabel");
		_descriptionLabel = GetNode<Label>("PanelContainer/VBoxContainer/DescriptionLabel");
		_priceLabel = GetNode<Label>("PanelContainer/VBoxContainer/MarginContainer/HBoxContainer/PriceLabel");
		_textureRect = GetNode<TextureRect>("PanelContainer/VBoxContainer/TextureRect");
		_vBoxContainer = GetNode<VBoxContainer>("PanelContainer/VBoxContainer");

		UpdateInfo();
	}

	private void UpdateInfo()
	{
		if (ItemStats != null)
		{
			_vBoxContainer.Visible = true;
			_nameLabel.Text = ItemStats.Name;
			_textureRect.Texture = ItemStats.Texture;
			_descriptionLabel.Text = "Description: " + ItemStats.Description;
			_priceLabel.Text = "Price: " + ItemStats.Price.ToString();
		}
		else
		{
			_vBoxContainer.Visible = false;
		}
	}

	private void CreateTextPopup(string text)
	{
		var textPopup = _textPopupPacked.Instance<TextPopup>();
		textPopup.Text = text;
		textPopup.Color = Colors.DarkRed;
		Global.PopupManager.AddPopup(textPopup);
	}

	private void OnBuyButtonPressed()
	{
		if (_playerStats.Money < ItemStats.Price)
		{
			CreateTextPopup("Not enough money");
			return;
		}

		var addedItem = _playerInventory.AddItem(ItemStats.Duplicate() as ItemStats);
		if (!addedItem)
		{
			CreateTextPopup("Not enough space in inventory");
			return;
		}

		_playerStats.Money -= ItemStats.Price;
	}
}
