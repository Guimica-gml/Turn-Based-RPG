using Godot;
using Godot.Collections;

public class BattleOptionsPanel : PanelContainer
{
	[Signal] private delegate void Interacted();
	[Signal] private delegate void ActionSelected(Action action);
	
	[Export] private NodePath _inventoryPath = "";
	private PauseDisplayer _inventory;
	
	[Export] private float _textSpeed = 0.025f;
	private bool _mouseInsideArea = false;
	
	private HBoxContainer _optionsContainer;
	private RichTextLabel _textLabel;
	private GridContainer _actionsContainer;
	private TextureRect _nextMessageArrow;
	private Timer _timer;
	
	private PackedScene _actionPuttonPacked = GD.Load<PackedScene>("res://Battles/ActionButton/ActionButton.tscn");
	
	public override void _Ready()
	{
		_optionsContainer = GetNode<HBoxContainer>("HboxContainer/OptionsContainer");
		_inventory = GetNode<PauseDisplayer>(_inventoryPath);
		_textLabel = GetNode<RichTextLabel>("HboxContainer/TextLabel");
		_nextMessageArrow = GetNode<TextureRect>("HboxContainer/TextLabel/NextMessageRect");
		_actionsContainer = GetNode<GridContainer>("HboxContainer/OptionsContainer/ActionsContainer");
		_timer = GetNode<Timer>("Timer");
	}
	
	public override void _Input(InputEvent @event)
	{
		if (_mouseInsideArea && @event.IsActionPressed("left_click"))
		{
			if (!(_textLabel.VisibleCharacters >= _textLabel.GetTotalCharacterCount()))
			{
				_textLabel.VisibleCharacters = _textLabel.GetTotalCharacterCount();
				_nextMessageArrow.Visible = true;
				return;
			}
			
			EmitSignal(nameof(Interacted));
		}
	}
	
	public void SetPlayerActionButtons(Array<Action> actions)
	{
		foreach (var action in actions)
		{
			var actionButton = _actionPuttonPacked.Instance<ActionButton>();
			actionButton.Action = action;
			actionButton.Connect("ActionSelected", this, nameof(OnActionButtonSelected));
			_actionsContainer.AddChild(actionButton);
		}
	}
	
	public void SetActionsVisibility(bool visible)
	{
		_nextMessageArrow.Visible = false;
		_optionsContainer.Visible = visible;
	}
	
	public void SetText(string text)
	{
		_timer.Stop();
		
		_nextMessageArrow.Visible = false;
		_textLabel.VisibleCharacters = 0;
		_textLabel.Text = text;
		
		_timer.Start(_textSpeed);
	}
	
	private void OnItemsButtonPressed()
	{
		_inventory.Visible = true;
	}
	
	private void OnActionButtonSelected(Action action)
	{
		EmitSignal(nameof(ActionSelected), action);
	}
	
	private void OnTimerTimeout()
	{
		_textLabel.VisibleCharacters++;
		if (!_optionsContainer.Visible && _textLabel.VisibleCharacters >= _textLabel.GetTotalCharacterCount())
		{
			_nextMessageArrow.Visible = true;
		}
	}
	
	private void OnTextLabelMouseEntered()
	{
		_mouseInsideArea = true;
	}
	
	private void OnTextLabelMouseExited()
	{
		_mouseInsideArea = false;
	}
}
