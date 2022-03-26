using Godot;
using Godot.Collections;

public class Stats : MyResource
{
	[Signal] private delegate void HpChanged(int hp);
	[Signal] private delegate void XpChanged(int xp);
	[Signal] private delegate void AttackChanged(int attack);
	[Signal] private delegate void DefenseChanged(int defense);
	[Signal] private delegate void MoneyChanged(int money);
	[Signal] private delegate void LevelChanged(int level, string message);
	[Signal] private delegate void ZeroHp();

	[Export] public string Name = "";
	[Export] public Texture SpriteSheet = null;

	[Export(PropertyHint.Range, "1, 100")] private int _baseLevel = 1;
	[Export] private int _baseMaxHp = 1;
	[Export] private int _baseAttack = 1;
	[Export] private int _baseDefense = 1;
	[Export] private int _money = 0;

	[Export] private int _baseMinXpDrop = 0;
	[Export] private int _baseMaxXpDrop = 0;

	[Export] private int _baseMinDropMoney = 1;
	[Export] private int _baseMaxDropMoney = 1;

	[Export] public Array<Action> Actions = new Array<Action>() { null, null, null, null };
	[Export] public EnemyAI BattleAI = null;

	private float _hp = 1.0f;
	private int _baseXp = 0;

	private readonly Dictionary<string, Dictionary> _boosts = new Dictionary<string, Dictionary>()
	{
		{ nameof(Attack) , new Dictionary() { { "Perm", 0 }, { "Temp", 0 }, { "Signal", nameof(AttackChanged)  } } },
		{ nameof(Defense), new Dictionary() { { "Perm", 0 }, { "Temp", 0 }, { "Signal", nameof(DefenseChanged) } } },
	};

	public int Money
	{
		get => _money;
		set
		{
			_money = Mathf.Max(0, value);
			EmitSignal(nameof(MoneyChanged), Money);
		}
	}

	public float Hp
	{
		get => (int) (_hp * MaxHp);
		set
		{
			_hp = Mathf.Clamp(value, 0f, 1f);
			EmitSignal(nameof(HpChanged), Hp);
			if (Hp <= 0f) EmitSignal(nameof(ZeroHp));
		}
	}

	public int MaxHp
	{
		get => (int) (_baseMaxHp + Mathf.Pow(1.8f, Level - 1));
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
				Level += _baseXp / xpToNextLevel;
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
			var levelBefore = Level;
			var maxHpBefore = MaxHp;
			var attackBefore = GetStatWithoutTempBoost(nameof(Attack));
			var defenseBefore = GetStatWithoutTempBoost(nameof(Defense));

			_baseLevel = Mathf.Clamp(value, 1, 100);

			var attack = GetStatWithoutTempBoost(nameof(Attack));
			var defense = GetStatWithoutTempBoost(nameof(Defense));

			var message = string.Join(
				System.Environment.NewLine,
				$"Level: {levelBefore} -- {Level}",
				$"Hp: {maxHpBefore} -- {MaxHp}",
				$"Attack: {attackBefore} -- {attack}",
				$"Defense: {defenseBefore} -- {defense}"
			);

			EmitSignal(nameof(LevelChanged), Level, message);
		}
	}

	public int Attack
	{
		get => (int) (_baseAttack + Mathf.Pow(1.9f, Level - 1)) + GetStatBoost(nameof(Attack)) + GetStatBoost(nameof(Attack), temp:true);
	}

	public int Defense
	{
		get => (int) (_baseDefense + Mathf.Pow(1.5f, Level - 1)) + GetStatBoost(nameof(Defense)) + GetStatBoost(nameof(Defense), temp:true);
	}

	public override void _Init()
	{
		// Connecting to the battle system
		Global.BattleManager.Connect("BattleEnded", this, nameof(OnBattleEnded));

		// Making sure all resources are unique
		for (var i = 0; i < Actions.Count; ++i)
		{
			Actions[i] = Actions[i]?.Duplicate() as Action;
		}
	}

	public int GetStatWithoutBoost(string stat)
	{
		if (!_boosts.ContainsKey(stat)) return -1;
		return ((int) Get(stat)) - (int) _boosts[stat]["Temp"] - (int) _boosts[stat]["Perm"];
	}

	public int GetStatWithoutTempBoost(string stat)
	{
		if (!_boosts.ContainsKey(stat)) return -1;
		return ((int) Get(stat)) - (int) _boosts[stat]["Temp"];
	}

	public int GetStatBoost(string statName, bool temp = false)
	{
		var state = (temp) ? "Temp" : "Perm";
		return (int) _boosts[statName][state];
	}

	public void IncreaseStatBoost(string stat, int amount, bool temp)
	{
		if (!_boosts.ContainsKey(stat)) return;

		var state = (temp) ? "Temp" : "Perm";
		_boosts[stat][state] = (int) _boosts[stat][state] + amount;

		EmitSignal((string) _boosts[stat]["Signal"], Get(stat));
	}

	public int GetXpDrop()
	{
		int minXpDrop = _baseMinXpDrop + ((Level - 1) * 20);
		int maxXpDrop = _baseMaxXpDrop + ((Level - 1) * 20);

		return (int) GD.RandRange(minXpDrop, maxXpDrop);
	}

	public int GetMoneyDrop()
	{
		int minDropMoney = _baseMinDropMoney + ((Level - 1) * 20);
		int maxDropMoney = _baseMaxDropMoney + ((Level - 1) * 20);

		return (int) GD.RandRange(minDropMoney, maxDropMoney);
	}

	public int XpToNextLevel()
	{
		return (int) (100 * Mathf.Pow(Level, 1.2f));
	}

	public void Heal(int amount)
	{
		Hp = (float) (Mathf.Min(Hp + amount, MaxHp) / MaxHp);
	}

	public void AttackTarget(Stats target, Action action)
	{
		target.Hurt(action.Value + (int) ((Attack / 15) * GD.RandRange(0.85f, 1f)));
	}

	public void Hurt(int damage)
	{
		var realDamage = Mathf.Max(1, damage - Defense / 15);
		Hp = (float) (Mathf.Max(0, Hp - realDamage) / MaxHp);
	}

	public bool HasActions()
	{
		var numberOfActions = 0;

		foreach (Action action in Actions)
		{
			if (action == null) continue;
			numberOfActions += action.PP;
		}

		return (numberOfActions > 0);
	}

	private void OnBattleEnded()
	{
		// Reseting temporary boosts
		var keys = new Array<string>(_boosts.Keys);
		foreach (var key in keys) _boosts[key]["Temp"] = 0;

		// Reseting the PPs
		foreach (var action in Actions)
		{
			if (action == null) continue;
			action.PP = action.MaxPP;
		}
	}
}
