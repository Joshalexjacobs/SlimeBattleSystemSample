using System;
using System.Collections.Generic;
using SlimeBattleSystem;
using UnityEngine;
using UnityEngine.Events;

public class Combatant : MonoBehaviour
{
    
    public Participant participant;

    public List<DroppableItem> droppableItems;

    public List<Item> items = new List<Item>();

    public UnityEvent OnHurt;
    
    public UnityEvent OnDefeat;

}

[Serializable]
public class DroppableItem {

    public Item item;

    public int chanceToDrop = 1;

}