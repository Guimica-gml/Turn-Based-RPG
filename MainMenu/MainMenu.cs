using Godot;

public class MainMenu : Node2D
{
	[Export] private Interaction _interaction = null;
	[Export] private Interaction _controlsInteraction = null;
	
	private void OnStartButtonPressed()
	{
		if (Global.TransitionManager.InTransition) return;
		Global.InteractionManager.StartInteraction(_interaction);
	}
	
	private void OnControlsButtonPressed()
	{
		if (Global.TransitionManager.InTransition) return;
		Global.InteractionManager.StartInteraction(_controlsInteraction);
	}
	
	private void OnExitButtonPressed()
	{
		if (Global.TransitionManager.InTransition) return;
		GetTree().Quit();
	}
}
