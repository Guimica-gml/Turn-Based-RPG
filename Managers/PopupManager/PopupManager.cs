using Godot;

public class PopupManager : CanvasLayer
{
	public PopupScreen PopupScreen;
	
	private PackedScene _popupScreenPacked = GD.Load<PackedScene>("res://Popups/Screen/PopupScreen.tscn");
	
	public override void _Ready()
	{
		PopupScreen = _popupScreenPacked.Instance<PopupScreen>();
		AddChild(PopupScreen);
	}
	
	public void AddPopup(Popup popup)
	{
		if (PopupScreen == null)
		{
			GD.Print("PopupScreen was not set properly in PopupManager.cs");
			return;
		}
		
		PopupScreen.AddPopup(popup);
	}
}
