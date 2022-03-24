using Godot;

public class PopupManager : CanvasLayer
{
	public PopupScreen PopupScreen { get; private set; }

	private PackedScene _popupScreenPacked = GD.Load<PackedScene>("res://Popups/Screen/PopupScreen.tscn");

	public override void _Ready()
	{
		PopupScreen = _popupScreenPacked.Instance<PopupScreen>();
		AddChild(PopupScreen);
	}

	public void AddPopup(GenericPopup popup)
	{
		PopupScreen.AddPopup(popup);
	}
}
