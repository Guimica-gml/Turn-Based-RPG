using Godot;

public class InteractionManager : Node
{
	[Signal] private delegate void InteractionTriggered(Interaction interaction);
	[Signal] private delegate void InteractionEnded(Interaction interaction);
	
	public Interaction Interaction = null;
	
	public void StartInteraction(Interaction interaction)
	{
		if (Interaction != null)
		{
			GD.PrintErr("Trying to start an interaction while another interaction is already happening");
			return;
		}
		
		if (interaction == null)
		{
			GD.PrintErr("Trying to start a `null` interaction");
			return;
		}
		
		Interaction = interaction;
		Interaction.Connect("Ended", this, nameof(EndInteraction));
		
		Interaction.OnInteractionTrigger();
		EmitSignal(nameof(InteractionTriggered), Interaction);
	}
	
	private void EndInteraction()
	{
		Interaction.OnInteractionEnd();
		Interaction.Disconnect("Ended", this, nameof(EndInteraction));
		EmitSignal(nameof(InteractionEnded), Interaction);
		Interaction = null;
	}
}
