using System;
using Godot;
using Godot.Collections;

public class BattleOptionsPanel : PanelContainer
{
	[Signal] private delegate void Interacted();
	[Signal] private delegate void TextDisplayed();
	[Signal] private delegate void ActionSelected(Action action);
	
	public bool EnableInput = true;
	
	[Export] private NodePath _inventoryPath = "";
	private PauseDisplayer _inventory;
	
	[Export] private float _textSpeed = 0.025f;
	private bool _mouseInsideArea = false;
	
	private Func<bool> _optionalCondition = null;
	
	private HBoxContainer _optionsContainer;
	private RichTextLabel _textLabel;
	private GridContainer _actionsContainer;
	private TextureRect _nextMessageArrow;
	private Timer _timer;
	
	private PackedScene _actionPuttonPacked = GD.Load<PackedScene>("res://Battles/ActionButton/ActionButton.tscn");
	
	public override void _Ready()
	{
		_inventory = GetNode<PauseDisplayer>(_inventoryPath);
		_optionsContainer = GetNode<HBoxContainer>("HboxContainer/OptionsContainer");
		_textLabel = GetNode<RichTextLabel>("HboxContainer/TextLabel");
		_nextMessageArrow = GetNode<TextureRect>("HboxContainer/TextLabel/NextMessageRect");
		_actionsContainer = GetNode<GridContainer>("HboxContainer/OptionsContainer/ActionsContainer");
		_timer = GetNode<Timer>("Timer");
	}
	
	public override void _Input(InputEvent @event)
	{
		if (!EnableInput) return;
		
		if (_mouseInsideArea && @event.IsActionPressed("left_click"))
		{
			if (!(_textLabel.VisibleCharacters >= _textLabel.GetTotalCharacterCount()))
			{
				_textLabel.VisibleCharacters = _textLabel.GetTotalCharacterCount();
				ShowText();
				return;
			}
			
			_nextMessageArrow.Visible = false;
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
	
	public void SetText(string text, Func<bool> condition = null)
	{
		_timer.Stop();
		
		_optionalCondition = condition;
		
		_nextMessageArrow.Visible = false;
		_textLabel.VisibleCharacters = 0;
		_textLabel.Text = text;
		
		_timer.Start(_textSpeed);
	}
	
	private void ShowText()
	{
		if (_optionalCondition == null || _optionalCondition())
		{
			_nextMessageArrow.Visible = true;
			EmitSignal(nameof(TextDisplayed));
			_timer.Stop();
		}
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
			ShowText();
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
