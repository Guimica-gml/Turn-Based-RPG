using Godot;

public class GameOver : Control
{
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("left_click") && !Global.TransitionManager.InTransition)
		{
			GetTree().Quit();
		}
	}
}
