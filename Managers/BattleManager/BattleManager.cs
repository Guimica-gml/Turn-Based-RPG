using Godot;

public class BattleManager : Node
{
	[Signal] private delegate void BattleStarted();
	[Signal] private delegate void BattleEnded();
	
	public bool InBattle { get; private set; } = false;
	
	private Enemy _currentEnemy = null;
	
	private BattlePauseDisplayer _battleScenario = null;
	private PackedScene _battleScenarioPacked = GD.Load<PackedScene>("res://PauseDisplayer/BattlePauseDisplayer/BattlePauseDisplayer.tscn");
	
	public void StartBattle(Enemy enemy)
	{
		InBattle = true;
		
		_battleScenario = _battleScenarioPacked.Instance<BattlePauseDisplayer>();
		_battleScenario.EnemyStats = enemy.Stats;
		_battleScenario.Connect("Close", this, nameof(OnBattleEnded));
		
		Global.Manager.PauseGame(_battleScenario);
		
		_currentEnemy = enemy;
		EmitSignal(nameof(BattleStarted));
	}
	
	private void OnBattleEnded()
	{
		if (IsInstanceValid(_currentEnemy))
		{
			_currentEnemy.QueueFree();
			_currentEnemy = null;
		}
		
		InBattle = false;
		EmitSignal(nameof(BattleEnded));
	}
}
