using System.Collections.Generic;
using SlimeBattleSystem;
using UnityEngine;

public class BattleController : MonoBehaviour
{

    [SerializeField] private List<Participant> participants;

    public void StartBattle()
    {
        participants = BattleSystem.DetermineTurnOrder(participants);
    }

}
