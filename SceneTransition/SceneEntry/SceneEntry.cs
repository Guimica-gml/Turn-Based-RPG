using Godot;

public class SceneEntry : Node2D
{
	[Export] public string Indentifier = "";
	
	private ColorRect _colorRect;
	
	public override void _Ready()
	{
		// The ColorRect is just there for visual representation, we can just queue free it
		_colorRect = GetNode<ColorRect>("ColorRect");
		_colorRect.QueueFree();
	}
}
