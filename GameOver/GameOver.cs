using Godot;

public class GameOver : Control
{
	public override void _Ready()
	{
		Global.Manager.InMenu = true;
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("left_click") && !Global.TransitionManager.InTransition)
		{
			GetTree().Quit();
		}
	}
}
