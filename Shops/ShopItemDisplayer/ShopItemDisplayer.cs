using Godot;

public class ShopItemDisplayer : CenterContainer
{
	[Signal] private delegate void ButtonToggled(bool buttonToggled, int buttonIndex);
	
	[Export] public ItemStats ItemStats
	{
		get => _itemStats;
		set
		{
			_itemStats = value;
			UpdateSlot();
		}
	}
	private ItemStats _itemStats;
	
	private TextureRect _textureRect;
	private Button _button;
	
	public override void _Ready()
	{
		_textureRect = GetNode<TextureRect>("Button/TextureRect");
		_button = GetNode<Button>("Button");
		
		UpdateSlot();
	}
	
	private void UpdateSlot()
	{
		if (ItemStats != null)
		{
			_textureRect.Texture = ItemStats.Texture;
		}
		else
		{
			_textureRect.Texture = null;
		}
	}
	
	public void SetButtonPressed(bool pressed)
	{
		_button.Pressed = pressed;
	}
	
	private void OnButtonToggled(bool toggled)
	{
		EmitSignal(nameof(ButtonToggled), toggled, GetIndex());
	}
}
