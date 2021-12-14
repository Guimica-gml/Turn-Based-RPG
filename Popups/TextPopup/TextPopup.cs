using Godot;

public class TextPopup : GenericPopup
{
	[Export] public string Text = null;
	[Export] public Color Color = Colors.White;
	
	private Label _label;
	
	public override void _Ready()
	{
		base._Ready();
		
		_label = GetNode<Label>("Label");
		_label.Text = Text;
		_label.Modulate = Color;
	}
}
