using Godot;

public class BattleCheckForWinnerState : State
{
	[Export] private NodePath _battleScenarioPath = "";
	private BattleScenario _battleScenario;
	
	public override void StateReady()
	{
		_battleScenario = GetNode<BattleScenario>(_battleScenarioPath);
		
		if (_battleScenario.PlayerDisplayer.Stats.Hp <= 0)
		{
			EmitSignal(nameof(RequestState), "PlayerLost");
		}
		else if (_battleScenario.EnemyDisplayer.Stats.Hp <= 0)
		{
			EmitSignal(nameof(RequestState), "PlayerWin");
		}
		else
		{
			var nextState = (_battleScenario.CurrentTurn == BattleScenario.Turns.Player) ? "EnemyTurn" : "PlayerTurn";
			EmitSignal(nameof(RequestState), nextState);
		}
	}
	
	public override void StateProcess(float delta)
	{
		return;
	}
	
	public override void StateFree()
	{
		_battleScenario = null;
	}
}
