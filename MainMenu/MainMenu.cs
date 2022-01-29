using Godot;

public class MainMenu : Node2D
{
	[Export] private Interaction _interaction = null;
	
	private void OnStartButtonPressed()
	{
		if (Global.TransitionManager.InTransition) return;
		Global.InteractionManager.StartInteraction(_interaction);
	}
	
	private void OnExitButtonPressed()
	{
		if (Global.TransitionManager.InTransition) return;
		GetTree().Quit();
	}
}
