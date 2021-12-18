using Godot;

public class PauseDisplayer : Control
{
	[Signal] protected delegate void Close();
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_pause")) EmitSignal(nameof(Close));
		@event.Dispose();
	}
}
