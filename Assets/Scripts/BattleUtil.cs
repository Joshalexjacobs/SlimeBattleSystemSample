using System.Collections.Generic;
using System.Linq;
using SlimeBattleSystem;

public static class BattleUtil 
{

  public static Combatant GetMatchingCombatant(Participant participant, List<Combatant> combatants) {
    var result = combatants.Where((combatant) => combatant.participant == participant).ToArray();
    
    return result.Length > 0 ? result[0] : null;
  }

}
