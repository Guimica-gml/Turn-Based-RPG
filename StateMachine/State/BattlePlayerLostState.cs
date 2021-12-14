using Godot;

public class BattlePlayerLostState : State
{
	[Export] private NodePath _battleScenarioPath = "";
	private BattlePauseDisplayer _battleScenario;
	
	public override void StateReady()
	{
		_battleScenario = GetNode<BattlePauseDisplayer>(_battleScenarioPath);
		_battleScenario.SetBattleText("You lost.");
		_battleScenario.SetNextMessageArrowVisibility(true);
	}
	
	public override void StateProcess(float delta)
	{
		if (_battleScenario.MouseHoverTextBox && !Global.TransitionManager.InTransition && Input.IsActionJustPressed("left_click"))
		{
			_battleScenario.SetNextMessageArrowVisibility(false);
			
			Global.TransitionManager.Connect("SceneChanged", this, nameof(Destroy));
			Global.TransitionManager.ChangeSceneTo("res://GameOver/GameOver.tscn", "none", TransitionEffect.Types.FromCenter);
		}
	}
	
	private void Destroy()
	{
		_battleScenario.Destroy();
	}
	
	public override void StateFree()
	{
		_battleScenario = null;
	}
}
