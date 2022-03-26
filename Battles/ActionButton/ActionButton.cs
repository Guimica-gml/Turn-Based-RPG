using Godot;

public class ActionButton : CenterContainer
{
	[Signal] private delegate void ActionSelected(Action action);

	[Export] public Action Action = null;

	private Button _button;
	private Label _nameLabel;
	private Label _ppLabel;

	public override void _Ready()
	{
		_button = GetNode<Button>("Button");
		_nameLabel = GetNode<Label>("Button/VBoxContainer/NameLabel");
		_ppLabel = GetNode<Label>("Button/VBoxContainer/PPLabel");

		if (Action != null)
		{
			UpdateButton(Action.PP);
			Action.Connect("PPChanged", this, nameof(OnActionPPChanged));
		}
		else
		{
			_button.Text = "";
			_button.Disabled = true;
		}
	}

	private void UpdateButton(int pp)
	{
		_nameLabel.Text = $"{Action.Name}";
		_ppLabel.Text = $"({pp}/{Action.MaxPP})";
		_button.Disabled = (pp <= 0);
	}

	private void OnActionPPChanged(int pp)
	{
		UpdateButton(pp);
	}

	private void OnButtonPressed()
	{
		EmitSignal(nameof(ActionSelected), Action);
	}
}
