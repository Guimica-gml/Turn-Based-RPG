using Godot;

public class InputPanel : PanelContainer
{
	[Signal] private delegate void CommandEntered(string command);
	
	private LineEdit _lineEdit;
	
	public override void _Ready()
	{
		_lineEdit = GetNode<LineEdit>("HBoxContainer/LineEdit");
		_lineEdit.GrabFocus();
	}
	
	private void OnLineEditTextEntered(string newText)
	{
		if (newText.Empty()) return;
		EmitSignal(nameof(CommandEntered), newText);
		_lineEdit.Text = "";
	}
}
