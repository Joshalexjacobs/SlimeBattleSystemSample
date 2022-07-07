using SlimeBattleSystem;

public class Herb : Item {

  public int recoveryAmount = 15;
  
  public override void UseItem (Participant target, BattleLog battleLog) {
    target.stats.hitPoints += recoveryAmount;
    
    battleLog.UpdateLog($"{target.name} used a {name} and recovered {recoveryAmount} hit points!\n");

    if (target.stats.hitPoints > target.stats.maxHitPoints) {
      target.stats.hitPoints = target.stats.maxHitPoints;
    }
  }
  
}
