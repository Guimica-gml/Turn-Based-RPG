using Godot;
using Godot.Collections;

public class Inventory : Resource
{
	[Signal] private delegate void ItemsChanged(Array<ItemStats> items);
	[Signal] private delegate void KeyItemsChanged(Array<ItemStats> keyItems);
	[Signal] private delegate void ItemRemoved(string itemName);
	[Signal] private delegate void ItemAdded(ItemStats item);
	
	[Export] public int Size
	{
		get => Items.Count;
		set => Items.Resize(value);
	}
	
	public Array<ItemStats> KeyItems
	{
		get => _keyItems;
		set
		{
			_keyItems = value;
			EmitSignal(nameof(KeyItemsChanged), KeyItems);
		}
	}
	private Array<ItemStats> _keyItems = new Array<ItemStats>();
	
	public Array<ItemStats> Items
	{
		get => _items;
		private set
		{
			_items = value;
			EmitSignal(nameof(ItemsChanged), Items);
		}
	}
	private Array<ItemStats> _items = new Array<ItemStats>();
	
	public ItemStats SetItem(ItemStats item, int index)
	{
		var saveItem = Items[index];
		Items[index] = item;
		Items = Items;
		return saveItem;
	}
	
	public bool AddItem(ItemStats item)
	{
		item = item.Duplicate() as ItemStats;
		
		if (item.KeyItem)
			return AddKeyItem(item);
		if (item.Stackable)
			return AddStackableItem(item);
		return AddNonStackableItem(item);
	}
	
	private bool AddKeyItem(ItemStats item)
	{
		KeyItems.Add(item);
		EmitSignal(nameof(ItemAdded), item);
		KeyItems = KeyItems;
		return true;
	}
	
	private bool AddStackableItem(ItemStats item)
	{
		var itemInInventory = FindItems(item.Name);
		
		if (itemInInventory.Count > 0)
		{
			itemInInventory[0].Amount += item.Amount;
			Items = Items;
			EmitSignal(nameof(ItemAdded), item);
			return true;
		}
		
		return AddNonStackableItem(item);
	}
	
	private bool AddNonStackableItem(ItemStats item)
	{
		if (IsFull()) return false;
	
		var emptySlot = Items.IndexOf(null);
		Items[emptySlot] = item;
		EmitSignal(nameof(ItemAdded), item);
		Items = Items;
		
		return true;
	}
	
	public bool RemoveItem(int index, int amountToRemove)
	{
		if (!CheckItemAmount(index, amountToRemove))
		{
			GD.PrintErr("Trying to remove nonexistent items");
			return false;
		}
		
		var itemName = Items[index].Name;
		
		if (Items[index].Amount <= amountToRemove)
			Items[index] = null;
		else
			Items[index].Amount -= amountToRemove;
		
		Items = Items;
		EmitSignal(nameof(ItemRemoved), itemName);
		return true;
	}
	
	public bool RemoveItemByName(string itemName, int amountToRemove)
	{
		if (!CheckItemAmountByName(itemName, amountToRemove))
		{
			GD.PrintErr("Trying to remove nonexistent items");
			return false;
		}
		
		var itemsToRemove = FindItemIndexes(itemName);
		var amountRemoved = 0;
		
		foreach (int index in itemsToRemove)
		{
			var itemAmount = Items[index].Amount;
			RemoveItem(index, (int) Mathf.Min(amountToRemove - amountRemoved, itemAmount));
			var newAmount = (Items[index] != null) ? Items[index].Amount : 0;
			amountRemoved += itemAmount - newAmount;
			if (amountRemoved >= amountToRemove) break;
		}
		
		Items = Items;
		EmitSignal(nameof(ItemRemoved), itemName);
		return true;
	}
	
	public bool IsFull()
	{
		return (!Items.Contains(null));
	}
	
	public bool CheckItemAmount(int itemIndex, int expectedAmount)
	{
		return (Items[itemIndex] != null && Items[itemIndex].Amount >= expectedAmount);
	}
	
	public bool CheckItemAmountByName(string itemName, int expectedAmount)
	{
		int amount = 0;
		
		foreach (var item in FindItems(itemName))
		{
			amount += item.Amount;
		}
		
		return (amount >= expectedAmount);
	}
	
	public Array<int> FindItemIndexes(string itemName)
	{
		Array<int> itemsList = new Array<int>();
		
		for (var i = 0; i < Items.Count; ++i)
		{
			if (Items[i] == null || Items[i].Name != itemName) continue;
			itemsList.Add(i);
		}
		
		return itemsList;
	}
	
	public Array<ItemStats> FindItems(string itemName)
	{
		var indexes = FindItemIndexes(itemName);
		var items = new Array<ItemStats>();
		
		foreach (int index in indexes)
		{
			items.Add(Items[index]);
		}
		
		return items;
	}
	
	public void GroupItem(string itemName)
	{
		var indexes = FindItemIndexes(itemName);
		if (indexes.Count <= 1 || !Items[indexes[0]].Stackable) return;
		
		for (var i = 1; i < indexes.Count; ++i)
		{
			var index = indexes[i];
			Items[indexes[0]].Amount += Items[index].Amount;
			RemoveItem(index, Items[index].Amount);
		}
		
		Items = Items;
	}
	
	public void GroupAllItems()
	{
		foreach (var item in Items)
		{
			if (item == null || !item.Stackable) continue;
			GroupItem(item.Name);
		}
	}
	
	public bool HasItem(string itemName)
	{
		var hasItem = false;
		
		foreach (var item in Items)
		{
			if (item == null || item.Name != itemName) continue;
			hasItem = true;
			break;
		}
		
		return hasItem;
	}
	
	public bool HasKeyItem(string itemName)
	{
		var hasKeyItem = false;
		
		foreach (var item in KeyItems)
		{
			if (item == null || item.Name != itemName) continue;
			hasKeyItem = true;
			break;
		}
		
		return hasKeyItem;
	}
}
