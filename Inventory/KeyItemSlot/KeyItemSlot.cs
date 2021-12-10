using Godot;

public class KeyItemSlot : HBoxContainer
{
	private ItemStats _itemStats;
	[Export] public ItemStats ItemStats
	{
		get => _itemStats;
		set
		{
			_itemStats = value;
			UpdateSlot();
		}
	}
	
	private TextureRect _textureRect;
	private Label _label;
	
	public override void _Ready()
	{
		_textureRect = GetNode<TextureRect>("TextureRect");
		_label = GetNode<Label>("Label");
		
		UpdateSlot();
	}
	
	private void UpdateSlot()
	{
		if (ItemStats != null)
		{
			_textureRect.Texture = ItemStats.Texture;
			_label.Text = ItemStats.Name;
		}
		else
		{
			_textureRect.Texture = null;
			_label.Text = "";
		}
	}
}
