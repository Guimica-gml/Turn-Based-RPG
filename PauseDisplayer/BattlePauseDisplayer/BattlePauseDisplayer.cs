using Godot;

public class BattlePauseDisplayer : PauseDisplayer
{
	public enum Turns { None, Player, Enemy }
	public Turns CurrentTurn = Turns.None;
	
	public bool MouseHoverTextBox { get; private set; }
	public Stats EnemyStats;
	
	private Stats _playerStats;
	private AnimationPlayer _animationPlayer;
	private HBoxContainer _optionsContainer;
	private GridContainer _actionsContainer;
	private RichTextLabel _textLabel;
	private TextureRect _nextMessageArrow;
	
	private BattleStatsDisplayer _playerStatsDisplayer;
	private BattleStatsDisplayer _enemyStatsDisplayer;
	
	public BattleCharacterDisplayer PlayerDisplayer;
	public BattleCharacterDisplayer EnemyDisplayer;
	
	private PackedScene _actionPuttonPacked = GD.Load<PackedScene>("res://Battles/ActionButton/ActionButton.tscn");
	
	public override void _Ready()
	{
		_playerStats = GD.Load<Stats>("res://Stats/PlayerStats.tres");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_optionsContainer = GetNode<HBoxContainer>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/OptionsContainer");
		_actionsContainer = GetNode<GridContainer>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/OptionsContainer/ActionsContainer");
		_textLabel = GetNode<RichTextLabel>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/TextLabel");
		_nextMessageArrow = GetNode<TextureRect>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/TextLabel/NextMessageRect");
		
		_playerStatsDisplayer = GetNode<BattleStatsDisplayer>("VBoxContainer/TextureRect/PlayerStatsDisplayer");
		_enemyStatsDisplayer = GetNode<BattleStatsDisplayer>("VBoxContainer/TextureRect/EnemyStatsDisplayer");
		
		PlayerDisplayer = GetNode<BattleCharacterDisplayer>("VBoxContainer/TextureRect/PlayerDisplayer");
		EnemyDisplayer = GetNode<BattleCharacterDisplayer>("VBoxContainer/TextureRect/EnemyDisplayer");
		
		_playerStatsDisplayer.Stats = _playerStats;
		_enemyStatsDisplayer.Stats = EnemyStats;
		
		PlayerDisplayer.Stats = _playerStats;
		EnemyDisplayer.Stats = EnemyStats;
		
		CreatePlayerActionButtons();
	}
	
	public override void _Input(InputEvent @event)
	{
		return; // This must be empty, do not dare removing this function
	}
	
	public void EndBattle()
	{
		_animationPlayer.Play("FadeOut");
	}
	
	public void SetBattleText(string text)
	{
		_textLabel.Text = text;
	}
	
	public void SetNextMessageArrowVisibility(bool visible)
	{
		_nextMessageArrow.Visible = visible;
	}
	
	public void SetActionButtonsVisibility(bool visible)
	{
		_optionsContainer.Visible = visible;
	}
	
	public void Destroy()
	{
		EmitSignal(nameof(Close));
		QueueFree();
	}
	
	private void CreatePlayerActionButtons()
	{
		foreach (var action in _playerStats.Actions)
		{
			var actionButton = _actionPuttonPacked.Instance<ActionButton>();
			actionButton.Action = action;
			_actionsContainer.AddChild(actionButton);
		}
	}
	
	private void OnAnimationPlayerAnimationFinished(string animName)
	{
		if (animName != "FadeOut") return;
		Destroy();
	}
	
	private void OnTextLabelMouseEntered()
	{
		MouseHoverTextBox = true;
	}
	
	private void OnTextLabelMouseExited()
	{
		MouseHoverTextBox = false;
	}
}
