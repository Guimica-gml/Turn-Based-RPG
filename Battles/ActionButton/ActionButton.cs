using Godot;

public class ActionButton : CenterContainer
{
	[Signal] private delegate void ActionSelected(Action action);
	
	[Export] public Action Action = null;
	
	private Button _button;
	
	public override void _Ready()
	{
		_button = GetNode<Button>("Button");
		
		if (Action != null)
		{
			_button.Text = Action.Name;
		}
		else
		{
			_button.Text = "";
			_button.Disabled = true;
		}
	}
	
	private void OnButtonPressed()
	{
		EmitSignal(nameof(ActionSelected), Action);
	}
}
