using Godot;

public class ItemSlot : CenterContainer
{
	public ItemStats ItemStats
	{
		get => _itemStats;
		set
		{
			_itemStats = value;
			UpdateSlot();
		}
	}
	[Export] private ItemStats _itemStats;
	
	[Export] private NodePath _itemInfoDisplayerPath = "";
	
	public Inventory Inventory;
	public ItemInfoDisplayer ItemInfoDisplayer;
	
	private TextureRect _textureRect;
	private Label _nameLabel;
	private Label _amountLabel;
	private Button _button;
	
	public override void _Ready()
	{
		_button = GetNode<Button>("Button");
		_textureRect = GetNode<TextureRect>("Button/TextureRect");
		_nameLabel = GetNode<Label>("Button/NameLabel");
		_amountLabel = GetNode<Label>("Button/AmountLabel");
		if (_itemInfoDisplayerPath != "") ItemInfoDisplayer = GetNode<ItemInfoDisplayer>(_itemInfoDisplayerPath);
		
		_button.FocusMode = FocusModeEnum.None;
		UpdateSlot();
	}
	
	private void UpdateSlot()
	{
		if (ItemStats != null)
		{
			_textureRect.Texture = ItemStats.Texture;
			_nameLabel.Text = ItemStats.Name;
			
			if (!ItemStats.KeyItem)
			{
				_amountLabel.Text = ItemStats.Amount.ToString();
				_button.Modulate = (_itemStats.UseJustInBattle && !Global.BattleManager.Active) ? new Color(1f, 1f, 1f, 0.5f) : new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				_amountLabel.Text = "";
				_button.SelfModulate = new Color(1f, 1f, 1f, 0.5f);
				_button.Modulate = new Color(1f, 1f, 1f, 1f);
			}
		}
		else
		{
			_textureRect.Texture = null;
			_nameLabel.Text = "";
			_amountLabel.Text = "";
			_button.Modulate = new Color(1f, 1f, 1f, 0.5f);
		}
	}
	
	private void OnButtonPressed()
	{
		if (ItemStats == null || ItemStats.KeyItem) return;
		if (ItemStats.UseJustInBattle && !Global.BattleManager.Active) return;
		
		ItemStats.Use();
		Inventory.RemoveItem(GetIndex(), 1);
		
		if (ItemInfoDisplayer == null || ItemStats != null) return;
		ItemInfoDisplayer.ItemStats = null;
	}
	
	private void OnButtonMouseEntered()
	{
		if (ItemInfoDisplayer == null || ItemStats == null) return;
		ItemInfoDisplayer.ItemStats = ItemStats;
	}
	
	private void OnButtonMouseExited()
	{
		if (ItemInfoDisplayer == null || ItemStats == null) return;
		ItemInfoDisplayer.ItemStats = null;
	}
}
