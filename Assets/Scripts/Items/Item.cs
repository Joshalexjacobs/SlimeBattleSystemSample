using SlimeBattleSystem;
using UnityEngine;

public class Item : MonoBehaviour {

    public string name;
    
    public virtual void UseItem(Participant target) { }
    
}
