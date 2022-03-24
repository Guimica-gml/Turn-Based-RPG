using Godot;

public class StatBar : Control
{
	[Export] private Color _color = new Color(1f, 1f, 1f, 1f);

	private ColorRect _bar;

	public override void _Ready()
	{
		_bar = GetNode<ColorRect>("Bar");
		_bar.Color = _color;
	}

	public void UpdateBar(float value)
	{
		value = Mathf.Clamp(value, 0f, 1f);
		_bar.RectScale = new Vector2(value, 1f);
	}
}
