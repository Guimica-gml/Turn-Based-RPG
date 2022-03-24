using Godot;

public class InteractableArea : Area2D
{
	[Export] private Interaction _interaction = null;

	public bool CanTrigger()
	{
		return _interaction.CanTrigger();
	}

	public Texture GetIcon()
	{
		return _interaction.GetIcon();
	}

	public void Interact()
	{
		Global.InteractionManager.StartInteraction(_interaction);
	}
}
