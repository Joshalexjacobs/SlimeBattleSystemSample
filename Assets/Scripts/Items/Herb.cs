using SlimeBattleSystem;

public class Herb : Item {

  private HerbItem HerbItem = new HerbItem();

  public override void UseItem(Participant target) {
    HerbItem.UseItem(target);
  }

}

public class HerbItem : SlimeBattleSystem.Item {

  public int recoveryAmount = 15;
  
  public override void UseItem(Participant target) {
    target.stats.hitPoints += recoveryAmount;

    // if (target.stats.hitPoints > target.stats.maxHitPoints) {
    //   target.stats.hitPoints = target.stats.maxHitPoints;
    // }
  }

}