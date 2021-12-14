using Godot;

public class InteractionManager : Node
{
	[Signal] private delegate void InteractionTriggered(Interaction interaction);
	[Signal] private delegate void InteractionEnded(Interaction interaction);
	
	public Interaction Interaction { get; private set; } = null;
	
	public void StartInteraction(Interaction interaction)
	{
		if (Interaction != null) return;
		
		if (interaction == null)
		{
			GD.PrintErr("Trying to start a `null` interaction");
			return;
		}
		
		Interaction = interaction;
		Interaction.Connect("Ended", this, nameof(EndInteraction));
		
		Interaction.OnTrigger();
		EmitSignal(nameof(InteractionTriggered), Interaction);
	}
	
	private void EndInteraction()
	{
		Interaction.OnEnd();
		Interaction.Disconnect("Ended", this, nameof(EndInteraction));
		EmitSignal(nameof(InteractionEnded), Interaction);
		Interaction = null;
	}
}
