using Godot;

public class PauseDisplayer : Control
{
	[Signal] protected delegate void Close();
	
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("ui_pause")) EmitSignal(nameof(Close));
	}
}
