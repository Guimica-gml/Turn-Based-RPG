using Godot;

public class BattlePauseDisplayer : PauseDisplayer
{
	[Export] public TransitionEffect.Types TransitionType = TransitionEffect.Types.Default;
	
	public enum Turns { None, Player, Enemy }
	public Turns CurrentTurn = Turns.None;
	
	private bool _destroyState = false;
	
	public bool MouseHoverTextBox { get; private set; }
	public Stats EnemyStats;
	
	private Stats _playerStats;
	private HBoxContainer _optionsContainer;
	private GridContainer _actionsContainer;
	private RichTextLabel _textLabel;
	private TextureRect _nextMessageArrow;
	private TransitionEffect _transitionEffect;
	
	private BattleStatsDisplayer _playerStatsDisplayer;
	private BattleStatsDisplayer _enemyStatsDisplayer;
	
	public BattleCharacterDisplayer PlayerDisplayer;
	public BattleCharacterDisplayer EnemyDisplayer;
	
	private PackedScene _actionPuttonPacked = GD.Load<PackedScene>("res://Battles/ActionButton/ActionButton.tscn");
	
	public override void _Ready()
	{
		_playerStats = GD.Load<Stats>("res://Stats/PlayerStats.tres");
		_transitionEffect = GetNode<TransitionEffect>("TransitionEffect");
		_optionsContainer = GetNode<HBoxContainer>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/OptionsContainer");
		_actionsContainer = GetNode<GridContainer>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/OptionsContainer/ActionsContainer");
		_textLabel = GetNode<RichTextLabel>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/TextLabel");
		_nextMessageArrow = GetNode<TextureRect>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/TextLabel/NextMessageRect");
		
		_playerStatsDisplayer = GetNode<BattleStatsDisplayer>("VBoxContainer/TextureRect/PlayerStatsDisplayer");
		_enemyStatsDisplayer = GetNode<BattleStatsDisplayer>("VBoxContainer/TextureRect/EnemyStatsDisplayer");
		
		PlayerDisplayer = GetNode<BattleCharacterDisplayer>("VBoxContainer/TextureRect/PlayerDisplayer");
		EnemyDisplayer = GetNode<BattleCharacterDisplayer>("VBoxContainer/TextureRect/EnemyDisplayer");
		
		// Setting up animtions for fade in and fade out
		
		Visible = false;
		
		_transitionEffect.Connect("EffectTransition", this, nameof(OnEffectTransition));
		_transitionEffect.Connect("EffectEnded", this, nameof(OnEffectEnded));
		
		_transitionEffect.StartEffect(TransitionType);
		
		// Setting information about the battle

		_playerStatsDisplayer.Stats = _playerStats;
		_enemyStatsDisplayer.Stats = EnemyStats;
		
		PlayerDisplayer.Stats = _playerStats;
		EnemyDisplayer.Stats = EnemyStats;
		
		CreatePlayerActionButtons();
	}
	
	public override void _Process(float delta)
	{
		return; // This must be empty, do not dare removing this function
	}
	
	public void EndBattle()
	{
		_destroyState = true;
		_transitionEffect.StartEffect(TransitionType);
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
	
	private void OnTextLabelMouseEntered()
	{
		MouseHoverTextBox = true;
	}
	
	private void OnTextLabelMouseExited()
	{
		MouseHoverTextBox = false;
	}
	
	private void OnEffectTransition()
	{
		Visible = !_destroyState;
	}
	
	private void OnEffectEnded()
	{
		if (!_destroyState) return;
		Destroy();
	}
}
