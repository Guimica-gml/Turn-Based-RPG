using Godot;

public class Popup : PanelContainer
{
	[Export] public ItemStats ItemStats = null;
	
	private TextureRect _textureRect;
	private Label _label;
	private AnimationPlayer _animationPlayer;
	
	public override void _Ready()
	{
		_textureRect = GetNode<TextureRect>("HBoxContainer/TextureRect");
		_label = GetNode<Label>("HBoxContainer/Label");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		
		if (ItemStats == null)
		{
			GD.Print("ItemStats was not defined in Popup.cs");
			return;
		}
		
		_textureRect.Texture = ItemStats.Texture;
		_label.Text = ItemStats.Name;
	}
	
	private void OnTimerTimeout()
	{
		_animationPlayer.Play("FadeOut");
	}
}
