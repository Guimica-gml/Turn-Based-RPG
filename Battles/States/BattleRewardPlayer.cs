using Godot;

public class BattleRewardPlayer : BattleUpdateText
{
	public int MoneyAmount = 0;
	public int XpAmount = 0;
	
	public BattleRewardPlayer(BattlePauseDisplayer battlePauseDisplayer, int moneyAmount, int xpAmount, string nextScheme = "") : base(battlePauseDisplayer, nextScheme:nextScheme)
	{
		MoneyAmount = moneyAmount;
		XpAmount = xpAmount;
		
		Text = $"You gained {MoneyAmount} rupies and {XpAmount} points of xp.";
	}
	
	public override void OnReady()
	{
		base.OnReady();
		
		BattlePauseDisplayer.PlayerStats.Connect("LevelChanged", this, nameof(OnPlayerStatsLevelChanged));
		
		BattlePauseDisplayer.PlayerStats.Xp += XpAmount;
		BattlePauseDisplayer.PlayerStats.Money += MoneyAmount;
	}
	
	public override void OnFinish()
	{
		BattlePauseDisplayer.PlayerStats.Disconnect("LevelChanged", this, nameof(OnPlayerStatsLevelChanged));
	}
	
	private void OnPlayerStatsLevelChanged(int level, string message)
	{
		BattlePauseDisplayer.LevelUpMessage = message;
	}
}
