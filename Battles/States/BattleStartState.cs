using Godot;

public class BattleStartState : State
{
	[Export] private NodePath _battleScenarioPath = "";
	private BattleScenario _battleScenario;
	
	public override void StateReady()
	{
		_battleScenario = GetNode<BattleScenario>(_battleScenarioPath);
		
		_battleScenario.SetActionButtonsVisibility(false);
		_battleScenario.SetNextMessageArrowVisibility(true);
		_battleScenario.SetBattleText($"A {_battleScenario.EnemyDisplayer.Stats.Name} appeared.");
	}
	
	public override void StateProcess(float delta)
	{
		if (_battleScenario.MouseHoverTextBox && Input.IsActionJustPressed("left_click"))
		{
			EmitSignal(nameof(RequestState), "PlayerTurn");
		}
	}
	
	public override void StateFree()
	{
		_battleScenario = null;
	}
}
