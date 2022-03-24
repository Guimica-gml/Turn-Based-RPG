using Godot;

public class BattleStatsDisplayer : PanelContainer
{
	private Stats _stats = null;
	[Export] public Stats Stats
	{
		get => _stats;
		set
		{
			_stats = value;
			UpdateInfo();
		}
	}

	[Export] private bool _showXp = true;

	private Label _nameLabel;
	private Label _levelLabel;
	private Label _hpLabel;
	private StatBar _hpBar;
	private StatBar _xpBar;

	public override void _Ready()
	{
		_nameLabel = GetNode<Label>("VBoxContainer/HBoxContainer/NameLabel");
		_levelLabel = GetNode<Label>("VBoxContainer/HBoxContainer/LevelLabel");
		_hpLabel = GetNode<Label>("VBoxContainer/HpBar/HpLabel");
		_hpBar = GetNode<StatBar>("VBoxContainer/HpBar");
		_xpBar = GetNode<StatBar>("VBoxContainer/XpBar");

		_hpLabel.Visible = true;
		if (!_showXp) _xpBar.Visible = false;
	}

	private void UpdateInfo()
	{
		if (Stats == null)
		{
			GD.PrintErr("Stats was not defined in BattleStatsDisplayer.cs");
			return;
		}

		Stats.Connect("HpChanged", this, nameof(UpdateHpBar));
		Stats.Connect("XpChanged", this, nameof(UpdateXpBar));
		Stats.Connect("LevelChanged", this, nameof(UpdateLevel));

		UpdateHpBar(Stats.Hp);
		UpdateXpBar(Stats.Xp);
		UpdateLevel(Stats.Level);

		_nameLabel.Text = Stats.Name;
	}

	private void UpdateHpBar(float hp)
	{
		_hpBar.UpdateBar(hp / Stats.MaxHp);
		_hpLabel.Text = $"{hp}/{Stats.MaxHp}";
	}

	private void UpdateXpBar(int xp)
	{
		_xpBar.UpdateBar((float) xp / (float) Stats.XpToNextLevel());
	}

	private void UpdateLevel(int level, string message = "")
	{
		_levelLabel.Text = $"Lv{level}";
	}
}
