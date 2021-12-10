using Godot;

public class BattlePlayerLostState : State
{
	[Export] private NodePath _battleScenarioPath = "";
	private BattleScenario _battleScenario;
	
	public override void StateReady()
	{
		_battleScenario = GetNode<BattleScenario>(_battleScenarioPath);
		_battleScenario.SetBattleText("You lost.");
		_battleScenario.SetNextMessageArrowVisibility(true);
	}
	
	public override void StateProcess(float delta)
	{
		if (_battleScenario.MouseHoverTextBox && !Global.TransitionManager.InTransition && Input.IsActionJustPressed("left_click"))
		{
			Global.TransitionManager.Connect("SceneChanged", this, nameof(Destroy));
			Global.TransitionManager.ChangeSceneTo("res://GameOver/GameOver.tscn");
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
