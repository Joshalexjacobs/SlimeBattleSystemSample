using System.Collections.Generic;
using SlimeBattleSystem;
using UnityEngine;

public class Spell : MonoBehaviour {
    
    public string name;

    public int mpCost;

    public virtual List<Participant> DetermineTarget(List<Participant> participants) 
    {
        return participants;
    }
    
    public virtual void UseSpell(List<Participant> targets, BattleLog battleLog) { }
}
