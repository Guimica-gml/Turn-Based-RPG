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

	public override void _Ready()
	{
		_nameLabel = GetNode<Label>("MarginContainer/VBoxContainer/NameLabel");
		_descriptionLabel = GetNode<Label>("MarginContainer/VBoxContainer/DescriptionLabel");

		UpdateDisplayer();
	}

	private void UpdateDisplayer()
	{
		_nameLabel.Text = "";
		_descriptionLabel.Text = "";

		if (ItemStats != null)
		{
			_nameLabel.Text = ItemStats.Name;
			_descriptionLabel.Text = "Description: " + ItemStats.Description;
		}
	}
}
