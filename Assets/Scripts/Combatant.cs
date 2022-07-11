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
        
    public List<Spell> spells = new List<Spell>();

    public UnityEvent OnHurt;
    
    public UnityEvent OnDefeat;
    
    public virtual CombatantAction DetermineAction() {
        // override me
        
        return new CombatantAction(CombatantActionType.Attack);
    }

    public virtual Spell GetSpell() {
        // override me
        
        return null;
    }

}

[Serializable]
public class DroppableItem {

    public Item item;

    public int chanceToDrop = 1;

}

[Serializable]
public enum CombatantActionType {
    Attack,
    Item,
    Spell,
    Flee
}

[Serializable]
public class CombatantAction {
    public CombatantActionType Type;

    public CombatantAction() { }
    
    public CombatantAction(CombatantActionType type) {
        Type = type;
    }
}