using Godot;

public class StatKeyValue : HBoxContainer
{
	[Export] public string KeyName = "";
	
	[Export] private string _value = "";
	public string Value
	{
		get => _value;
		set
		{
			_value = value;
			UpdateValue();
		}
	}
	
	private Label _keyLabel;
	private Label _valueLabel;
	
	public override void _Ready()
	{
		_keyLabel = GetNode<Label>("KeyLabel");
		_valueLabel = GetNode<Label>("ValueLabel");
		
		_keyLabel.Text = KeyName;
		UpdateValue();
	}
	
	private void UpdateValue()
	{
		_valueLabel.Text = Value;
	}
}
