using Godot;

public class BattleManager : Node
{
	[Signal] private delegate void BattleStarted();
	[Signal] private delegate void BattleEnded();
	
	public bool Active { get; private set; } = false;
	
	private Enemy _currentEnemy = null;
	
	private BattleDisplayer _battleDisplayer = null;
	private PackedScene _battleScenarioPacked = GD.Load<PackedScene>("res://Battles/BattleDisplayer.tscn");
	
	public void StartBattle(Enemy enemy)
	{
		Active = true;
		
		_battleDisplayer = _battleScenarioPacked.Instance<BattleDisplayer>();
		_battleDisplayer.EnemyStats = enemy.Stats;
		_battleDisplayer.Connect("Close", this, nameof(OnBattleEnded));
		
		Global.Manager.PauseGame(_battleDisplayer);
		
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
		
		Active = false;
		EmitSignal(nameof(BattleEnded));
	}
}
