using Godot;

public class StatsDisplayer : PanelContainer
{
	[Export] public Stats Stats;
	
	private StatKeyValue _hpKeyValue;
	private StatKeyValue _attackKeyValue;
	private StatKeyValue _defenseKeyValue;
	private StatKeyValue _levelKeyValue;
	private StatKeyValue _xpKeyValue;
	
	public override void _Ready()
	{
		_hpKeyValue = GetNode<StatKeyValue>("MarginContainer/VBoxContainer/VBoxContainer/HpStat");
		_attackKeyValue = GetNode<StatKeyValue>("MarginContainer/VBoxContainer/VBoxContainer/AttackStat");
		_defenseKeyValue = GetNode<StatKeyValue>("MarginContainer/VBoxContainer/VBoxContainer/DefenseStat");
		_levelKeyValue = GetNode<StatKeyValue>("MarginContainer/VBoxContainer/VBoxContainer/LevelStat");
		_xpKeyValue = GetNode<StatKeyValue>("MarginContainer/VBoxContainer/VBoxContainer/XpStat");
		
		if (Stats == null)
		{
			GD.PrintErr("Stats was not defined in StatsDisplayer.cs");
			return;
		}
		
		Stats.Connect("HpChanged", this, nameof(UpdateHp));
		Stats.Connect("AttackChanged", this, nameof(UpdateAttack));
		Stats.Connect("DefenseChanged", this, nameof(UpdateDefense));
		Stats.Connect("LevelChanged", this, nameof(UpdateLevel));
		Stats.Connect("XpChanged", this, nameof(UpdateXp));
		
		UpdateHp(Stats.Hp);
		UpdateAttack(Stats.Attack);
		UpdateDefense(Stats.Defense);
		UpdateLevel(Stats.Level);
		UpdateXp(Stats.Xp);
	}
	
	private void UpdateHp(int hp)
	{
		_hpKeyValue.Value = hp.ToString() + "/" + Stats.MaxHp.ToString();
	}
	
	private void UpdateAttack(int attack)
	{
		_attackKeyValue.Value = attack.ToString();
	}
	
	private void UpdateDefense(int defense)
	{
		_defenseKeyValue.Value = defense.ToString();
	}
	
	private void UpdateLevel(int level, string message = "")
	{
		_levelKeyValue.Value = level.ToString();
	}
	
	private void UpdateXp(int xp)
	{
		_xpKeyValue.Value = xp.ToString() + "/" + Stats.XpToNextLevel().ToString();
	}
}
