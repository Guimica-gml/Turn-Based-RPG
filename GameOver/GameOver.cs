using Godot;

public class GameOver : Node2D
{
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("left_click") && !Global.TransitionManager.InTransition)
		{
			GetTree().Quit();
		}
		
		@event.Dispose();
	}
}
