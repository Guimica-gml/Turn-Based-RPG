using Godot;

public class BattlePlayerWinState : State
{
	[Export] private NodePath _battleScenarioPath = "";
	private BattleScenario _battleScenario;
	
	private bool _leveledUp = false;
	private string _levelingUpMessage = "";
	
	public override void StateReady()
	{
		_battleScenario = GetNode<BattleScenario>(_battleScenarioPath);
		_battleScenario.SetNextMessageArrowVisibility(true);
		_battleScenario.SetBattleText($"You defeated {_battleScenario.EnemyDisplayer.Stats.Name}. \nYou gained {_battleScenario.EnemyDisplayer.Stats.XpWhenDefeated} xp.");
		
		_battleScenario.PlayerDisplayer.Stats.Connect("LevelChanged", this, nameof(OnPlayerLevelChanged));
		_battleScenario.PlayerDisplayer.Stats.Xp += _battleScenario.EnemyDisplayer.Stats.XpWhenDefeated;
	}
	
	public override void StateProcess(float delta)
	{
		if (_battleScenario.MouseHoverTextBox && Input.IsActionJustPressed("left_click"))
		{
			if (_leveledUp)
			{
				_battleScenario.SetBattleText(_levelingUpMessage);
				_leveledUp = false;
				return;
			}
			
			_battleScenario.EndBattle();
		}
	}
	
	public override void StateFree()
	{
		_battleScenario = null;
	}
	
	private void OnPlayerLevelChanged(int level, string message)
	{
		_leveledUp = true;
		_levelingUpMessage = "You leveled Up! \n" + message;
	}
}
