using Godot;

public class ControlsMenu : Node2D
{
	[Export] private Interaction _interaction = null;
	
	private void OnControlsButtonPressed()
	{
		if (Global.TransitionManager.Active) return;
		Global.InteractionManager.StartInteraction(_interaction);
	}
}
