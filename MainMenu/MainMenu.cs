using Godot;

public class MainMenu : Control
{
	[Export] private Interaction interaction = null;
	
	public override void _Ready()
	{
		Global.Manager.InMenu = true;
	}
	
	private void OnStartButtonPressed()
	{
		if (Global.TransitionManager.InTransition) return;
		Global.InteractionManager.StartInteraction(interaction);
		Global.Manager.InMenu = false;
	}
	
	private void OnExitButtonPressed()
	{
		if (Global.TransitionManager.InTransition) return;
		GetTree().Quit();
	}
}
