using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory
{
    public Item[] itemContainer;
    public Inventory(int size)
    {
        itemContainer = new Item[size];
    }
    
    public bool InsertItem(Item item)
    {
        for(int i = 0; i < itemContainer.Length; i++)
        {
            if (itemContainer[i] == null) 
            {
                itemContainer[i] = item;
                return true;
            }
        }
        return false;
    }

    public bool RemoveItem(Item item)
    {
        for(int i =0; i < itemContainer.Length; i++)
        {
            if (itemContainer[i] == item)
            {
                itemContainer[i] = null;
                return true;
            }
        }
        return false;
    }

    public bool CheckItem(Item item)
    {
        for(int i =0; i < itemContainer.Length;i++)
        {
            if (itemContainer[i] == item)
            {
                return true;
            }
        }
        return false;
    }
}
