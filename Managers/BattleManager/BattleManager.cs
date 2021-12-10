using Godot;

public class BattleManager : CanvasLayer
{
	[Signal] private delegate void BattleStarted();
	[Signal] private delegate void BattleEnded();
	
	public bool InBattle = false;
	
	private Enemy _currentEnemy = null;
	
	private BattleScenario _battleScenario = null;
	private PackedScene _battleScenarioPacked = GD.Load<PackedScene>("res://Battles/BattleScenario.tscn");
	
	public void StartBattle(Enemy enemy)
	{
		if (enemy.Stats == null)
		{
			GD.PrintErr("Trying to start a battle without an enemy");
			return;
		}
		
		if (InBattle || Global.InteractionManager.Interaction != null) return;
		GetTree().Paused = true;
		InBattle = true;
		
		_battleScenario = _battleScenarioPacked.Instance<BattleScenario>();
		_battleScenario.EnemyStats = enemy.Stats;
		_battleScenario.Connect("Ended", this, nameof(OnBattleEnded));
		AddChild(_battleScenario);
		
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
		_battleScenario = null;
		GetTree().Paused = false;
		EmitSignal(nameof(BattleEnded));
	}
}
