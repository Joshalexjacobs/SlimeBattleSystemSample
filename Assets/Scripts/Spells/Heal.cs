using System.Collections.Generic;
using SlimeBattleSystem;
using Random = UnityEngine.Random;

public class Heal : Spell
{
  
  public int recoveryAmount = 17;
  
  public override List<Participant> DetermineTarget(List<Participant> participants) {
    return BattleSystem.GetPlayerParticipants(participants);
  }

  public override void UseSpell(List<Participant> targets, BattleLog battleLog) {
    foreach (var participant in targets) {
      var randomizedRecoveryAmount = (int)(recoveryAmount * Random.Range(0.6f, 1f));
      
      participant.stats.hitPoints += randomizedRecoveryAmount;

      battleLog.UpdateLog($"{participant.name} chanted {name} and recovered {randomizedRecoveryAmount} hit points!\n");
      
      if (participant.stats.hitPoints > participant.stats.maxHitPoints) {
        participant.stats.hitPoints = participant.stats.maxHitPoints;
      }
    }
  }
  
}
