using SlimeBattleSystem;
using UnityEngine;
using UnityEngine.Events;

public class Combatant : MonoBehaviour
{
    
    public Participant participant;

    public UnityEvent OnHurt;
    
    public UnityEvent OnDefeat;

}
