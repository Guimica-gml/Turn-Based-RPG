using Godot;

public class ItemPopup : Popup
{
	[Export] public ItemStats ItemStats = null;
	
	private TextureRect _textureRect;
	private Label _label;
	
	public override void _Ready()
	{
		base._Ready();
		
		_textureRect = GetNode<TextureRect>("HBoxContainer/TextureRect");
		_label = GetNode<Label>("HBoxContainer/Label");
		
		if (ItemStats == null)
		{
			GD.Print("ItemStats was not defined in ItemPopup.cs");
			return;
		}
		
		_textureRect.Texture = ItemStats.Texture;
		_label.Text = ItemStats.Name;
	}
}
