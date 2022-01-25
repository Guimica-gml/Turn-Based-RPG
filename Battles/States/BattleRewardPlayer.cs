using Godot;

public class BattleRewardPlayer : BattleUpdateText
{
	public int MoneyAmount = 0;
	public int XpAmount = 0;
	
	public BattleRewardPlayer(BattleDisplayer battleDisplayer, int moneyAmount, int xpAmount, string nextScheme = "") : base(battleDisplayer, nextScheme:nextScheme)
	{
		MoneyAmount = moneyAmount;
		XpAmount = xpAmount;
		
		Text = $"You gained {MoneyAmount} rupies and {XpAmount} points of xp.";
	}
	
	public override void OnReady()
	{
		base.OnReady();
		
		BattleDisplayer.PlayerStats.Connect("LevelChanged", this, nameof(OnPlayerStatsLevelChanged));
		
		BattleDisplayer.PlayerStats.Xp += XpAmount;
		BattleDisplayer.PlayerStats.Money += MoneyAmount;
	}
	
	public override void OnFinish()
	{
		BattleDisplayer.PlayerStats.Disconnect("LevelChanged", this, nameof(OnPlayerStatsLevelChanged));
	}
	
	private void OnPlayerStatsLevelChanged(int level, string message)
	{
		BattleDisplayer.LevelUpMessage = message;
	}
}
