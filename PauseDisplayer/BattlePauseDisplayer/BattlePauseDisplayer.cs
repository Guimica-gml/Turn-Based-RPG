using Godot;
using Godot.Collections;

public class BattlePauseDisplayer : PauseDisplayer
{
	[Export] public TransitionEffect.Types TransitionType = TransitionEffect.Types.Default;
	[Export] private float _textSpeed = 0.025f;
	
	[Export] public Stats EnemyStats;
	public Stats PlayerStats;
	
	public string UsedItemName = "";
	public string LevelUpMessage = "";
	public Action CurrentAction = null;
	private bool _destroyState = false;
	
	private Dictionary<string, Array<BattleState>> _battleSchemes = new Dictionary<string, Array<BattleState>>();
	
	private string _currentScheme = "";
	private int _currentState = 0;
	
	public bool MouseHoverTextBox { get; private set; }
	
	private Button _inventoryButton;
	private PauseDisplayer _inventory;
	private HBoxContainer _optionsContainer;
	private GridContainer _actionsContainer;
	private RichTextLabel _textLabel;
	private TextureRect _nextMessageArrow;
	private TransitionEffect _transitionEffect;
	private Timer _timer;
	
	private BattleStatsDisplayer _playerStatsDisplayer;
	private BattleStatsDisplayer _enemyStatsDisplayer;
	
	public BattleCharacterDisplayer PlayerDisplayer;
	public BattleCharacterDisplayer EnemyDisplayer;
	
	private PackedScene _actionPuttonPacked = GD.Load<PackedScene>("res://Battles/ActionButton/ActionButton.tscn");
	
	public override void _Ready()
	{
		
		// Getting some resource and node references
		PlayerStats = GD.Load<Stats>("res://Stats/PlayerStats.tres");
		
		_timer = GetNode<Timer>("Timer");
		_inventory = GetNode<PauseDisplayer>("InventoryPauseDisplayer");
		_transitionEffect = GetNode<TransitionEffect>("TransitionEffect");
		_optionsContainer = GetNode<HBoxContainer>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/OptionsContainer");
		_inventoryButton = GetNode<Button>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/OptionsContainer/ItemsButton");
		_actionsContainer = GetNode<GridContainer>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/OptionsContainer/ActionsContainer");
		_textLabel = GetNode<RichTextLabel>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/TextLabel");
		_nextMessageArrow = GetNode<TextureRect>("VBoxContainer/OptionsPanel/MarginContainer/HboxContainer/TextLabel/NextMessageRect");
		
		_playerStatsDisplayer = GetNode<BattleStatsDisplayer>("VBoxContainer/TextureRect/PlayerStatsDisplayer");
		_enemyStatsDisplayer = GetNode<BattleStatsDisplayer>("VBoxContainer/TextureRect/EnemyStatsDisplayer");
		
		PlayerDisplayer = GetNode<BattleCharacterDisplayer>("VBoxContainer/TextureRect/PlayerDisplayer");
		EnemyDisplayer = GetNode<BattleCharacterDisplayer>("VBoxContainer/TextureRect/EnemyDisplayer");
		
		// Setting information about the battle
		_playerStatsDisplayer.Stats = PlayerStats;
		_enemyStatsDisplayer.Stats = EnemyStats;
		
		PlayerDisplayer.Stats = PlayerStats;
		EnemyDisplayer.Stats = EnemyStats;
		
		CreatePlayerActionButtons();
		SetActionButtonsVisibility(false);
		
		// Setting up the battle schemes and states
		_battleSchemes.Add("Start", new Array<BattleState>()
		{
			new BattleUpdateText(this, $"A {EnemyStats.Name} appeared.", nextScheme:"PlayerTurn"),
		});
		
		_battleSchemes.Add("PlayerTurn", new Array<BattleState>()
		{
			new BattleUpdateText(this, "It's your turn."),
			new BattlePlayerSelectAction(this, _actionsContainer, _inventoryButton, _inventory, "Select your action.", startInvisible:false),
			new BattlePerformAction(this, PlayerDisplayer, EnemyDisplayer),
			new BattleUseItem(this),
			new BattleCheckForWinner(this, "PlayerLost", "PlayerWon", nextScheme:"EnemyTurn"),
		});
		
		_battleSchemes.Add("EnemyTurn", new Array<BattleState>()
		{
			new BattleUpdateText(this, $"It's {EnemyStats.Name}'s turn."),
			new BattleEnemySelectAction(this, EnemyStats.Actions),
			new BattlePerformAction(this, EnemyDisplayer, PlayerDisplayer),
			new BattleCheckForWinner(this, "PlayerLost", "PlayerWon", nextScheme:"PlayerTurn"),
		});
		
		_battleSchemes.Add("PlayerWon", new Array<BattleState>()
		{
			new BattleUpdateText(this, $"You defeated {EnemyStats.Name}."),
			new BattleRewardPlayer(this, EnemyStats.GetDropMoney(), EnemyStats.GetXpDrop()),
			new BattlePlayerLevelUp(this),
		});
		
		_battleSchemes.Add("PlayerLost", new Array<BattleState>()
		{
			new BattleUpdateText(this, "You lost."),
		});
		
		// Setting up animtions for fade in and fade out
		Visible = false;
		
		_transitionEffect.Connect("EffectTransition", this, nameof(OnEffectTransition));
		_transitionEffect.Connect("EffectEnded", this, nameof(OnEffectEnded));
		
		_transitionEffect.StartEffect(TransitionType);
	}
	
	private void UpdateState(string schemeName, int stateIndex)
	{
		if (_battleSchemes.ContainsKey(_currentScheme))
		{
			_battleSchemes[_currentScheme][_currentState].OnFinish();
			_battleSchemes[_currentScheme][_currentState].Disconnect("Finished", this, nameof(OnBattleStateFinished));
		}
		
		_currentScheme = schemeName;
		_currentState = stateIndex;
		
		_battleSchemes[_currentScheme][_currentState].Connect("Finished", this, nameof(OnBattleStateFinished));
		_battleSchemes[_currentScheme][_currentState].OnReady();
	}
	
	public override void _Process(float delta)
	{
		if (!_battleSchemes.ContainsKey(_currentScheme)) return;
		_battleSchemes[_currentScheme][_currentState].OnProcess(delta);
		
		if (MouseHoverTextBox && !CanChangeText() && Input.IsActionJustPressed("left_click"))
			_textLabel.VisibleCharacters = _textLabel.GetTotalCharacterCount();
	}
	
	public override void _Input(InputEvent @event)
	{
		// This must be empty, do not dare removing this function
		@event.Dispose();
	}
	
	public void EndBattle()
	{
		if (_transitionEffect.Active) return;
		_destroyState = true;
		_transitionEffect.StartEffect(TransitionType);
	}
	
	public bool CanChangeText()
	{
		return (_textLabel.VisibleCharacters >= _textLabel.GetTotalCharacterCount());
	}
	
	public void SetBattleText(string text, bool startInvisible = true)
	{
		_timer.Stop();
		
		_nextMessageArrow.Visible = false;
		if (startInvisible) _textLabel.VisibleCharacters = 0;
		_textLabel.Text = text;
		
		_timer.Start(_textSpeed);
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
		foreach (var action in PlayerStats.Actions)
		{
			var actionButton = _actionPuttonPacked.Instance<ActionButton>();
			actionButton.Action = action;
			_actionsContainer.AddChild(actionButton);
		}
	}
	
	private void OnBattleStateFinished(string nextScheme)
	{
		if (nextScheme != "")
		{
			UpdateState(nextScheme, 0);
			return;
		}
		
		// The end of the battle
		if (_battleSchemes[_currentScheme].Count <= (_currentState + 1))
		{
			EndBattle();
			return;
		}
		
		UpdateState(_currentScheme, _currentState + 1);
	}
	
	private void OnTextLabelMouseEntered()
	{
		MouseHoverTextBox = true;
	}
	
	private void OnTextLabelMouseExited()
	{
		MouseHoverTextBox = false;
	}
	
	private async void OnEffectTransition()
	{
		Visible = !_destroyState;
		
		if (!_destroyState)
		{
			await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
			
			// Starting the first state
			UpdateState("Start", 0);
		}
	}
	
	private void OnEffectEnded()
	{
		if (!_destroyState) return;
		Destroy();
	}
	
	private void OnTimerTimeout()
	{
		_textLabel.VisibleCharacters++;
		if (!_optionsContainer.Visible && _battleSchemes[_currentScheme][_currentState].CanShowNextStateArrow()) _nextMessageArrow.Visible = true;
	}
}
