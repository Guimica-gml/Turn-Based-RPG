using Godot;
using Godot.Collections;

public class InputPanel : PanelContainer
{
	[Signal] private delegate void CommandEntered(string command);

	private LineEdit _lineEdit;

	private int inputSelected = 0;
	private Array<string> oldInputs = new Array<string>() { "" };

	public override void _Ready()
	{
		_lineEdit = GetNode<LineEdit>("HBoxContainer/LineEdit");
		_lineEdit.GrabFocus();
	}

	public override void _Input(InputEvent @event)
	{
		var input = System.Convert.ToInt16(@event.IsActionPressed("ui_up")) - System.Convert.ToInt16(@event.IsActionPressed("ui_down"));
		inputSelected += input;
		inputSelected = Mathf.Clamp(inputSelected, 0, oldInputs.Count - 1);

		if (input != 0)
		{
			_lineEdit.Text = oldInputs[inputSelected];
			_lineEdit.GrabFocus();
		}

		@event.Dispose();
	}

	private void OnLineEditTextChanged(string newText)
	{
		if (inputSelected != 0) return;
		oldInputs[0] = newText;
	}

	private void OnLineEditTextEntered(string newText)
	{
		if (newText.Empty()) return;

		oldInputs.Insert(1, newText);
		if (oldInputs.Count > 20) oldInputs.RemoveAt(20);

		EmitSignal(nameof(CommandEntered), newText);

		inputSelected = 0;
		oldInputs[0] = "";
		_lineEdit.Text = "";
	}
}
