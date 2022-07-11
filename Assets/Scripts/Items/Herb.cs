using SlimeBattleSystem;

public class Herb : Item {

  public int recoveryAmount = 15;
  
  public override void UseItem (Participant target, BattleLog battleLog) {
    target.Stats.HitPoints += recoveryAmount;
    
    battleLog.UpdateLog($"{target.Name} used a {name} and recovered {recoveryAmount} hit points!\n");

    if (target.Stats.HitPoints > target.Stats.MaxHitPoints) {
      target.Stats.HitPoints = target.Stats.MaxHitPoints;
    }
  }
  
}
