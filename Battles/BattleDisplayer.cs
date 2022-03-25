using Godot;
using Godot.Collections;

public class BattleDisplayer : PauseDisplayer
{
	private enum Turns { None, Player, Enemy }

	[Export] private TransitionTypes _transitionType = TransitionTypes.Default;

	[Export] public Stats EnemyStats;
	private Stats _playerStats;

	private Turns _currentTurn = Turns.None;
	private bool _destroyState = false;

	private string _levelUpMessage = "";
	private string _usedItemName = "";

	private Inventory _playerInventory;

	private PauseDisplayer _inventoryDisplayer;
	private BattleOptionsPanel _optionsPanel;
	private TransitionEffect _transitionEffect;

	private BattleStatsDisplayer _playerStatsDisplayer;
	private BattleStatsDisplayer _enemyStatsDisplayer;

	private BattleCharacterDisplayer _playerDisplayer;
	private BattleCharacterDisplayer _enemyDisplayer;

	public override void _Ready()
	{
		// Getting some resource and node references
		_playerStats = GD.Load<Stats>("res://Stats/PlayerStats.tres");
		_playerInventory = GD.Load<Inventory>("res://Inventory/PlayerInventory.tres");

		_inventoryDisplayer = GetNode<PauseDisplayer>("InventoryDisplayer");
		_transitionEffect = GetNode<TransitionEffect>("TransitionEffect");
		_optionsPanel = GetNode<BattleOptionsPanel>("VBoxContainer/OptionsPanel");

		_playerStatsDisplayer = GetNode<BattleStatsDisplayer>("VBoxContainer/Background/PlayerStatsDisplayer");
		_enemyStatsDisplayer = GetNode<BattleStatsDisplayer>("VBoxContainer/Background/EnemyStatsDisplayer");

		_playerDisplayer = GetNode<BattleCharacterDisplayer>("VBoxContainer/Background/PlayerDisplayer");
		_enemyDisplayer = GetNode<BattleCharacterDisplayer>("VBoxContainer/Background/EnemyDisplayer");

		// Setting information about the battle
		_playerStatsDisplayer.Stats = _playerStats;
		_enemyStatsDisplayer.Stats = EnemyStats;

		_playerDisplayer.Stats = _playerStats;
		_enemyDisplayer.Stats = EnemyStats;

		_optionsPanel.SetPlayerActionButtons(_playerStats.Actions);

		_playerStats.Connect("LevelChanged", this, "OnPlayerStatsLevelChanged");
		_playerInventory.Connect("ItemRemoved", this, "OnPlayerInventoryItemRemoved");

		// Setting up animtions for fade in and fade out
		Visible = false;

		_transitionEffect.Connect("EffectTransition", this, nameof(OnEffectTransition));
		_transitionEffect.Connect("EffectEnded", this, nameof(OnEffectEnded));

		_transitionEffect.StartEffect(_transitionType);
	}

	public override void _Input(InputEvent @event)
	{
		if (_inventoryDisplayer.Visible && @event.IsActionPressed("ui_pause"))
			_inventoryDisplayer.Visible = false;

		@event.Dispose();
	}

	public async void StartBattle()
	{
		_optionsPanel.SetActionsVisibility(false);
		_optionsPanel.SetText($"A {EnemyStats.Name} appeared.");
		await ToSignal(_optionsPanel, "Interacted");
		PlayerTurn();
	}

	public async void PlayerTurn()
	{
		_currentTurn = Turns.Player;

		_optionsPanel.SetText("Select your action.");
		await ToSignal(_optionsPanel, "TextDisplayed");

		_optionsPanel.SetActionsVisibility(true);
		var vars = await ToSignal(_optionsPanel, "ActionSelected");
		var action = vars[0] as Action;

		if (_playerDisplayer.Active || _enemyDisplayer.Active) return;

		_optionsPanel.SetActionsVisibility(false);
		_optionsPanel.SetText($"You selected action {action.Name}.", () => !_playerDisplayer.Active && !_enemyDisplayer.Active);
		_optionsPanel.InputEnabled = false;

		if (!action.Heal)
		{
			_enemyDisplayer.ApplyAction(action, _playerDisplayer.Stats);
			await ToSignal(_enemyDisplayer, "AnimEnded");
		}
		else
		{
			_playerDisplayer.ApplyAction(action);
			await ToSignal(_playerDisplayer, "AnimEnded");
		}

		_optionsPanel.InputEnabled = true;
		await ToSignal(_optionsPanel, "Interacted");

		CheckWinner();
	}

	public async void UseItem()
	{
		_optionsPanel.SetActionsVisibility(false);
		_inventoryDisplayer.Visible = false;

		_optionsPanel.SetText($"You used item '{_usedItemName}'.");
		await ToSignal(_optionsPanel, "Interacted");

		_usedItemName = "";
		CheckWinner();
	}

	public async void EnemyTurn()
	{
		_currentTurn = Turns.Enemy;

		_optionsPanel.SetText($"It's {EnemyStats.Name}'s turn.");
		await ToSignal(_optionsPanel, "Interacted");

		var action = EnemyStats.BattleAI?.GetAction(EnemyStats);

		if (action == null)
		{
			GD.PrintErr("No battle AI set in enemy stats");
			return;
		}

		_optionsPanel.SetText($"{EnemyStats.Name} used action {action.Name}.", () => !_playerDisplayer.Active && !_enemyDisplayer.Active);
		_optionsPanel.InputEnabled = false;

		if (!action.Heal)
		{
			_playerDisplayer.ApplyAction(action, _enemyDisplayer.Stats);
			await ToSignal(_playerDisplayer, "AnimEnded");
		}
		else
		{
			_enemyDisplayer.ApplyAction(action);
			await ToSignal(_enemyDisplayer, "AnimEnded");
		}

		_optionsPanel.InputEnabled = true;
		await ToSignal(_optionsPanel, "Interacted");

		CheckWinner();
	}

	public void CheckWinner()
	{
		if (_playerStats.Hp <= 0)
		{
			PlayerLost();
		}
		else if (EnemyStats.Hp <= 0)
		{
			PlayerWon();
		}
		else if (_currentTurn == Turns.Player)
		{
			EnemyTurn();
		}
		else if (_currentTurn == Turns.Enemy)
		{
			PlayerTurn();
		}
	}

	public async void PlayerWon()
	{
		_optionsPanel.SetText("You won.");
		await ToSignal(_optionsPanel, "Interacted");

		var moneyDrop = EnemyStats.GetMoneyDrop();
		var xpDrop = EnemyStats.GetXpDrop();

		_playerStats.Money = moneyDrop;
		_playerStats.Xp += xpDrop;

		_optionsPanel.SetText($"You gained {xpDrop} xp and {moneyDrop} ruppies.");
		await ToSignal(_optionsPanel, "Interacted");

		if (_levelUpMessage != "")
		{
			_optionsPanel.SetText($"You leveled up! \n{_levelUpMessage}");
			await ToSignal(_optionsPanel, "Interacted");
			_levelUpMessage = "";
		}

		EndBattle();
	}

	public async void PlayerLost()
	{
		_optionsPanel.SetText("You lost.");
		await ToSignal(_optionsPanel, "Interacted");

		Global.TransitionManager.ChangeSceneTo("res://GameOver/GameOver.tscn", transitionType:_transitionType);
		await ToSignal(Global.TransitionManager, "SceneChanged");

		Destroy();
	}

	public void EndBattle()
	{
		if (_transitionEffect.Active) return;
		_destroyState = true;
		_transitionEffect.StartEffect(_transitionType);
	}

	public void Destroy()
	{
		EmitSignal(nameof(Close));
		QueueFree();
	}

	private void OnPlayerInventoryItemRemoved(string itemName)
	{
		_usedItemName = itemName;
		UseItem();
	}

	private void OnPlayerStatsLevelChanged(int level, string message)
	{
		_levelUpMessage = message;
	}

	private void OnEffectTransition()
	{
		Visible = !_destroyState;
	}

	private void OnEffectEnded()
	{
		if (!_destroyState)
		{
			StartBattle();
			return;
		}

		Destroy();
	}
}
