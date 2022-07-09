using System.Collections.Generic;
using CandyCoded;
using SlimeBattleSystem;
using UnityEngine;

public class Hurt : Spell
{
  
  public int damageAmount = 12;
  
  public override List<Participant> DetermineTarget(List<Participant> participants) {
    return new List<Participant>() { BattleSystem.GetEnemyParticipants(participants).Random() };
  }

  public override void UseSpell(List<Participant> targets, BattleLog battleLog) {
    foreach (var participant in targets) {
      var randomizedDamageAmount = (int)(damageAmount * Random.Range(0.6f, 1f));

      participant.stats.hitPoints -= randomizedDamageAmount;

      battleLog.UpdateLog($"{participant.name} chanted {name} and dealt {randomizedDamageAmount} damage to {participant.name}!\n");
    }
  }
}
