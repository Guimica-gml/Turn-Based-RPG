using Godot;

public class InteractionManager : Node
{
	[Signal] private delegate void InteractionTriggered(Interaction interaction);
	[Signal] private delegate void InteractionEnded(Interaction interaction);
	
	public bool InInteraction { get; private set; } = false;
	private Interaction _interaction = null;
	
	public void StartInteraction(Interaction interaction)
	{
		if (_interaction != null) return;
		InInteraction = true;
		
		_interaction = interaction;
		_interaction.Connect("Ended", this, nameof(EndInteraction));
		
		_interaction.OnTrigger();
		EmitSignal(nameof(InteractionTriggered), _interaction);
	}
	
	private void EndInteraction()
	{
		_interaction.OnEnd();
		_interaction.Disconnect("Ended", this, nameof(EndInteraction));
		EmitSignal(nameof(InteractionEnded), _interaction);
		
		_interaction = null;
		InInteraction = false;
	}
}
