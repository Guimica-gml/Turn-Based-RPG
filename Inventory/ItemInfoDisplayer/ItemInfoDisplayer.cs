using Godot;

public class ItemInfoDisplayer : PanelContainer
{
	public ItemStats ItemStats
	{
		get => _itemStats;
		set
		{
			_itemStats = value;
			UpdateDisplayer();
		}
	}
	[Export] private ItemStats _itemStats = null;
	
	private Label _nameLabel;
	private Label _descriptionLabel;
	private AnimationPlayer _animationPlayer;
	
	public override void _Ready()
	{
		_nameLabel = GetNode<Label>("MarginContainer/VBoxContainer/NameLabel");
		_descriptionLabel = GetNode<Label>("MarginContainer/VBoxContainer/DescriptionLabel");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
	
	private void UpdateDisplayer()
	{
		if (_animationPlayer.IsPlaying()) _animationPlayer.Stop();
		
		if (ItemStats != null)
		{
			_nameLabel.Text = ItemStats.Name;
			_descriptionLabel.Text = "Description: " + ItemStats.Description;
			_animationPlayer.Play("FadeIn");
		}
		else
		{
			_animationPlayer.Play("FadeOut");
		}
	}
}
