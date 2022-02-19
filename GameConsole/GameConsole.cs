using Godot;

public class GameConsole : PauseDisplayer
{
	private OutputPanel _outputPanel;
	
	public override void _Ready()
	{
		_outputPanel = GetNode<OutputPanel>("CenterContainer/PanelContainer/VBoxContainer/OutputPanel");
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_programmer_access"))
		{
			EmitSignal(nameof(Close));
		}
		
		@event.Dispose();
	}
	
	private string ProcessCommand(string command)
	{
		return $"[color=gray]{command}[/color]";
	}
	
	private void OnInputPanelCommandEntered(string command)
	{
		var output = ProcessCommand(command);
		_outputPanel.AddOutputLine(output);
	}
}
