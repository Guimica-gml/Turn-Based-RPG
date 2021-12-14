using Godot;
using Godot.Collections;
using System;

public class Stats : Resource
{
	[Signal] private delegate void HpChanged(int hp);
	[Signal] private delegate void XpChanged(int xp);
	[Signal] private delegate void AttackChanged(int attack);
	[Signal] private delegate void DefenseChanged(int defense);
	[Signal] private delegate void MoneyChanged(int money);
	[Signal] private delegate void LevelChanged(int level, string message);
	[Signal] private delegate void ZeroHp();
	
	public int AttackBoost = 0;
	public int DefenseBoost = 0;
	
	[Export] public string Name = "";
	[Export] public Texture SpriteSheet = null;
	[Export(PropertyHint.Range, "1, 100")] private int _baseLevel = 1;
	[Export] private int _baseHp = 1;
	[Export] private int _baseMaxHp = 1;
	[Export] private int _baseAttack = 1;
	[Export] private int _baseDefense = 1;
	[Export] private int _baseXpWhenDefeated = 0;
	[Export] private int _baseMoney = 0;
	
	[Export] private int _baseMinDropMoney = 1;
	[Export] private int _baseMaxDropMoney = 1;
	
	[Export] public Array<Action> Actions = new Array<Action>();
	
	private int _baseXp = 0;
	
	public int Money
	{
		get => _baseMoney;
		set
		{
			_baseMoney = Mathf.Max(0, value);
			EmitSignal(nameof(MoneyChanged), Money);
		}
	}
	
	public int Hp
	{
		get => _baseHp;
		set
		{
			_baseHp = value;
			EmitSignal(nameof(HpChanged), Hp);
			if (_baseHp <= 0) EmitSignal(nameof(ZeroHp));
		}
	}
	
	public int MaxHp
	{
		get => (int) _baseMaxHp + 8 * (Level / 2);
		set => _baseMaxHp = value;
	}
	
	public int Xp
	{
		get => _baseXp;
		set
		{
			_baseXp = value;
			
			var xpToNextLevel = XpToNextLevel();
			if (_baseXp >= xpToNextLevel)
			{
				Level += (int) (_baseXp / xpToNextLevel);
				_baseXp -= xpToNextLevel;
			}
			
			EmitSignal(nameof(XpChanged), Xp);
		}
	}
	
	public int Level
	{
		get => _baseLevel;
		set
		{
			var maxHpBefore = MaxHp;
			var levelBefore = Level;
			var attackBefore = Attack;
			var defenseBefore = Defense;
			
			_baseLevel = Mathf.Clamp(value, 1, 100);
			var message = $"Level: {levelBefore} -- {Level} \nHp: {maxHpBefore} -- {MaxHp} \nAttack: {attackBefore} -- {Attack} \nDefense: {defenseBefore} -- {Defense} \n";
			EmitSignal(nameof(LevelChanged), Level, message);
		}
	}
	
	public int Attack
	{
		get
		{
			ConnectToBattleSystem();
			return (int) _baseAttack + 10 * (Level / 2) + AttackBoost;
		}
		set
		{
			_baseAttack = value;
			EmitSignal(nameof(AttackChanged), _baseAttack);
		}
	}
	
	public int Defense
	{
		get
		{
			ConnectToBattleSystem();
			return (int) _baseDefense + 5 * (Level / 2) + DefenseBoost;
		}
		set
		{
			_baseDefense = value;
			EmitSignal(nameof(DefenseChanged), _baseDefense);
		}
	}
	
	public int XpWhenDefeated
	{
		get => (int) _baseXpWhenDefeated + (Level * 20);
		set => _baseXpWhenDefeated = value;
	}
	
	public int MinDropMoney
	{
		get => (int) _baseMinDropMoney + (Level * 20);
		set => _baseMinDropMoney = value;
	}
	
	public int MaxDropMoney
	{
		get => (int) _baseMaxDropMoney + (Level * 20);
		set => _baseMaxDropMoney = value;
	}
	
	public int GetDropMoney()
	{
		var random = new Random();
		return random.Next(MinDropMoney, MaxDropMoney);
	}
	
	public int XpToNextLevel()
	{
		return (int) (100 * Mathf.Pow(Level, 1.2f));
	}
	
	public void Heal(int amount)
	{
		Hp = Mathf.Min(Hp + amount, MaxHp);
	}
	
	public void AttackTarget(Stats target, Action action)
	{
		target.Hurt(action.Value + ((int) Attack / 15));
	}
	
	public void Hurt(int damage)
	{
		var realDamage = Mathf.Max(1, (int) (damage - Defense / 15));
		Hp = Mathf.Max(0, Hp - realDamage);
	}
	
	private void ConnectToBattleSystem()
	{
		if (Global.BattleManager == null || Global.BattleManager.IsConnected("BattleEnded", this, nameof(OnBattleEnded))) return;
		Global.BattleManager.Connect("BattleEnded", this, nameof(OnBattleEnded));
	}
	
	private void OnBattleEnded()
	{
		AttackBoost = 0;
		DefenseBoost = 0;
	}
}
