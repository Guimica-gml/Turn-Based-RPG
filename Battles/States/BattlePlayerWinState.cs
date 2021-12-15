using Godot;

public class BattlePlayerWinState : State
{
	[Export] private NodePath _battleScenarioPath = "";
	private BattlePauseDisplayer _battleScenario;
	
	private bool _done = false;
	private bool _leveledUp = false;
	private string _levelingUpMessage = "";
	
	public override void StateReady()
	{
		GD.Randomize();
		
		_battleScenario = GetNode<BattlePauseDisplayer>(_battleScenarioPath);
		
		var enemyStats = _battleScenario.EnemyDisplayer.Stats;
		var dropMoney = enemyStats.GetDropMoney();
		
		_battleScenario.SetNextMessageArrowVisibility(true);
		_battleScenario.SetBattleText($"You defeated {enemyStats.Name}. \nYou gained {enemyStats.XpWhenDefeated} xp and ${dropMoney} rupies.");
		
		_battleScenario.PlayerDisplayer.Stats.Connect("LevelChanged", this, nameof(OnPlayerLevelChanged));
		_battleScenario.PlayerDisplayer.Stats.Xp += enemyStats.XpWhenDefeated;
		_battleScenario.PlayerDisplayer.Stats.Money += dropMoney;
	}
	
	public override void StateProcess(float delta)
	{
		if (!_done && _battleScenario.MouseHoverTextBox && Input.IsActionJustPressed("left_click"))
		{
			if (_leveledUp)
			{
				_battleScenario.SetBattleText(_levelingUpMessage);
				_leveledUp = false;
				return;
			}
			
			_battleScenario.SetNextMessageArrowVisibility(false);
			_battleScenario.EndBattle();
			_done = true;
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
