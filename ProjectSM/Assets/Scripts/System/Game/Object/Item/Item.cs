using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemGrade
{
    Common,
    Classic,
    Masterwork,
    MasterPiece,
    Legend
}
public class Item
{
    public string name;
    public string description;
    public ItemGrade grade;

    public Item(string name, string description, ItemGrade grade)
    {
        this.name = name;
        this.description = description;
        this.grade = grade;
    }
}
