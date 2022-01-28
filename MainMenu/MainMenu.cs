using Godot;

public class MainMenu : Node2D
{
	[Export] private Interaction interaction = null;
	
	private void OnStartButtonPressed()
	{
		if (Global.TransitionManager.InTransition) return;
		Global.InteractionManager.StartInteraction(interaction);
	}
	
	private void OnExitButtonPressed()
	{
		if (Global.TransitionManager.InTransition) return;
		GetTree().Quit();
	}
}
