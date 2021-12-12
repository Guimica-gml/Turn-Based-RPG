using Godot;
using System;
using Godot.Collections;

public class BattleEnemyTurnState : State
{
	[Export] private NodePath _battleScenarioPath = "";
	private BattlePauseDisplayer _battleScenario;
	
	private bool _performedAction = false;
	private bool _performingAction = false;
	private BattleCharacterDisplayer _infoDisplayer = null;
	
	public override void StateReady()
	{
		GD.Randomize();
		
		_battleScenario = GetNode<BattlePauseDisplayer>(_battleScenarioPath);
		
		_battleScenario.CurrentTurn = BattlePauseDisplayer.Turns.Enemy;
		_battleScenario.SetNextMessageArrowVisibility(true);
		_battleScenario.SetBattleText($"{_battleScenario.EnemyDisplayer.Stats.Name}'s turn");
	}
	
	public override void StateProcess(float delta)
	{
		if (!_battleScenario.MouseHoverTextBox || _performingAction || !Input.IsActionJustPressed("left_click")) return;
		
		if (_performedAction)
		{
			EmitSignal(nameof(RequestState), "CheckForWinner");
		}
		else
		{
			_battleScenario.SetNextMessageArrowVisibility(false);
			
			var action = GetAction();
			
			if (action.Heal)
			{
				_battleScenario.EnemyDisplayer.ApplyAction(action);
				_infoDisplayer = _battleScenario.EnemyDisplayer;
			}
			else
			{
				_battleScenario.PlayerDisplayer.ApplyAction(action, _battleScenario.EnemyDisplayer.Stats);
				_infoDisplayer = _battleScenario.PlayerDisplayer;
			}
			
			_infoDisplayer.Connect("AnimEnded", this, nameof(OnAnimEnded));
			_battleScenario.SetBattleText($"{_battleScenario.EnemyDisplayer.Stats.Name} used {action.Name}.");
			_performingAction = true;
		}
	}
	
	public override void StateFree()
	{
		_battleScenario = null;
		_performedAction = false;
	}
	
	private Action GetAction()
	{
		var random = new Random();
		var enemyStats = _battleScenario.EnemyDisplayer.Stats;
		var healActions = new Array<Action>();
		var attackActions = new Array<Action>();
		
		foreach (var action in enemyStats.Actions)
		{
			if (action == null) continue;
			
			if (action.Heal)
			{
				healActions.Add(action);
			}
			else
			{
				attackActions.Add(action);
			}
		}
		
		if (enemyStats.Hp < enemyStats.MaxHp && random.Next(0, 100) > 70)
		{
			return healActions[random.Next(0, healActions.Count)];
		}
		
		return attackActions[random.Next(0, attackActions.Count)];
	}
	
	private void OnAnimEnded()
	{
		_performingAction = false;
		_performedAction = true;
		_battleScenario.SetNextMessageArrowVisibility(true);
		_infoDisplayer.Disconnect("AnimEnded", this, nameof(OnAnimEnded));
		_infoDisplayer = null;
	}
}
