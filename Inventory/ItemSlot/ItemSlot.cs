using Godot;

public class ItemSlot : CenterContainer
{
	[Export] private ItemStats _itemStats;
	public ItemStats ItemStats
	{
		get => _itemStats;
		set
		{
			_itemStats = value;
			UpdateSlot();
		}
	}
	
	public Inventory Inventory;
	
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
		
		_button.FocusMode = FocusModeEnum.None;
		UpdateSlot();
	}
	
	private void UpdateSlot()
	{
		if (ItemStats != null)
		{
			_textureRect.Texture = ItemStats.Texture;
			_nameLabel.Text = ItemStats.Name;
			_amountLabel.Text = ItemStats.Amount.ToString();
			
			_button.Modulate = (_itemStats.UseJustInBattle && !Global.BattleManager.InBattle) ? new Color(1f, 1f, 1f, 0.5f) : new Color(1f, 1f, 1f, 1f);
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
		if (ItemStats == null) return;
		if (_itemStats.UseJustInBattle && !Global.BattleManager.InBattle) return;
		
		ItemStats.Use();
		Inventory.RemoveItem(GetIndex(), 1);
	}
}
