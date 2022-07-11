using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SlimeBattleSystem;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    
    private enum BattleState
    {
        Pending,
        Started,
        EnemyTurn,
        PlayerTurn,
        SpellSelect,
        ItemSelect,
        Ended
    }

    [SerializeField] private List<Combatant> combatants;

    private List<Participant> participants;

    private Combatant enemyCombatant;

    private Combatant playerCombatant;

    private Participant currentParticipant;

    [SerializeField] private HeroStatsUI heroStatsUI;

    [SerializeField] private GameObject heroCommandsUI;

    [SerializeField] private GameObject spellsUI;
    
    [SerializeField] private GameObject itemsUI;

    [SerializeField] private BattleLog battleLog;

    [SerializeField] private ScreenShake screenShake;
        
    [SerializeField] private SoundManager soundManager;

    private BattleState battleState = BattleState.Pending;

    private void Awake()
    {
        if (battleState == BattleState.Pending)
        {
            battleState = BattleState.Started;
            
            participants = combatants.Select(combatant => combatant.participant).ToList();
        
            participants = BattleSystem.DetermineTurnOrder(participants);

            enemyCombatant = BattleUtil.GetMatchingCombatant(BattleSystem.GetEnemyParticipants(participants)[0], combatants);
                
            playerCombatant = BattleUtil.GetMatchingCombatant(GetPlayerParticipant(), combatants);

            battleLog.UpdateLog($"A {enemyCombatant.participant.Name} draws near!\n");

            StartCoroutine(TraverseParticipants());   
        }
    }

    public Participant GetPlayerParticipant() {
        return BattleSystem.GetPlayerParticipants(participants)[0];
    }

    private readonly List<BattleState> pendingBattleStates = new () {
        BattleState.PlayerTurn,
        BattleState.SpellSelect,
        BattleState.ItemSelect
    };

    private IEnumerator TraverseParticipants()
    {
        yield return new WaitForSeconds(0.5f);
        
        foreach (var participant in participants)
        {
            currentParticipant = participant;

            if (participant.Stats.HitPoints > 0 && !BattleSystem.IsBattleOver(participants) && battleState != BattleState.Ended)
            {

                if (participant.ParticipantType == ParticipantType.Enemy)
                {
                    battleState = BattleState.EnemyTurn;

                    var action = enemyCombatant.DetermineAction();

                    switch (action.Type) {
                        case CombatantActionType.Attack:
                            yield return StartCoroutine(HandleEnemyTurn());
                            
                            break;
                        case CombatantActionType.Item:
                            // handle enemy using an item
                            
                            break;
                        case CombatantActionType.Spell:
                            // handle enemy chanting a spell
                            
                            break;
                        case CombatantActionType.Flee:
                            // handle enemy fleeing
                            
                            break;
                    }
                } else if (participant.ParticipantType == ParticipantType.Player)
                {
                    battleState = BattleState.PlayerTurn;
                    
                    battleLog.UpdateLog("Command?\n");
                    
                    heroCommandsUI.SetActive(true);

                    yield return new WaitUntil(() => 
                        !pendingBattleStates.Contains(battleState)
                    );
                }   
            }

            yield return new WaitForSeconds(0.5f);
        }

        if (BattleSystem.IsBattleOver(participants))
        {
            battleState = BattleState.Ended;
            
            soundManager.StopSound(soundManager.battleMusic);

            if (enemyCombatant.participant.Stats.HitPoints <= 0)
            {
                // player won
                
                soundManager.PlaySound(soundManager.battleOver);

                enemyCombatant.OnDefeat.Invoke();
                
                battleLog.UpdateLog($"Thou hast done well in defeating the {enemyCombatant.participant.Name}.\n");

                yield return new WaitForSeconds(0.5f);
                
                var experiencePoints = BattleSystem.DetermineExperiencePoints(enemyCombatant.participant);

                battleLog.UpdateLog($"Thy Experience increases by {experiencePoints}.\n");
                
                yield return new WaitForSeconds(0.5f);
                
                var goldPoints = BattleSystem.DetermineGoldPoints(enemyCombatant.participant);

                if (goldPoints > 0)
                {
                    battleLog.UpdateLog($"Thy GOLD increases by {goldPoints}.\n");
                    
                    yield return new WaitForSeconds(0.5f);
                }

                Dictionary<Item, int> droppableItems = new Dictionary<Item, int>(); 
                
                foreach (var enemyCombatantDroppableItem in enemyCombatant.droppableItems) {
                    droppableItems.Add(enemyCombatantDroppableItem.item, enemyCombatantDroppableItem.chanceToDrop);
                }
                
                var itemsDropped = BattleSystem.DetermineItemsDropped<Item>(droppableItems);
                
                foreach (Item item in itemsDropped) {
                    battleLog.UpdateLog($"{enemyCombatant.participant.Name} dropped a {item.name}!\n");
                    
                    playerCombatant.items.Add(item);
                }
            }
            else
            {
                // player lost
                
                battleLog.UpdateLog("The player has fainted!\n");
            }
        }
        else if (battleState != BattleState.Ended)
        {
            StartCoroutine(TraverseParticipants());
        }
    }

    public void AttackEnemy()
    {
        if (battleState == BattleState.PlayerTurn)
        {
            StartCoroutine(AttackEnemyRoutine());
        }
    }

    private IEnumerator AttackEnemyRoutine()
    {
        heroCommandsUI.SetActive(false);
        
        yield return StartCoroutine(HandleAttack(enemyCombatant.participant));

        battleState = BattleState.Started;
    }

    private IEnumerator HandleAttack(Participant target)
    {
        var results = BattleSystem.DetermineAttackDamage(currentParticipant, target);

        battleLog.UpdateLog($"{currentParticipant.Name} attacks!.\n");

        yield return new WaitForSeconds(0.5f);

        switch (results.Type)
        {
            case AttackResult.AttackType.Hit:
                battleLog.UpdateLog($"{target.Name}'s Hit Points have been reduced by {results.Damage}.\n");

                break;
            case AttackResult.AttackType.CriticalHit:
                battleLog.UpdateLog($"Critical hit!!!\n");
                
                battleLog.UpdateLog($"{target.Name}'s Hit Points have been reduced by {results.Damage}.\n");
                
                break;
            
            case AttackResult.AttackType.Missed:
                battleLog.UpdateLog($"Missed! {target.Name} dodged the attack.\n");
                
                soundManager.PlaySound(soundManager.miss);
                
                break;
        }

        if (results.Damage > 0) {
            soundManager.PlaySound(soundManager.hit);
            
            var targetCombatant = BattleUtil.GetMatchingCombatant(target, combatants);
        
            targetCombatant.OnHurt.Invoke();

            if (target == playerCombatant.participant) {
                screenShake.TriggerShake();   
            }
        }

        target.InflictDamage(results.Damage);
        
        heroStatsUI.RefreshHeroStats();
        
        yield return new WaitForSeconds(0.5f);
    }
    
    private IEnumerator HandleEnemyTurn()
    {
        yield return new WaitForSeconds(1f);
            
        yield return StartCoroutine(HandleAttack(playerCombatant.participant));
    }
    
    public void Flee()
    {
        if (battleState == BattleState.PlayerTurn)
        {
            battleLog.UpdateLog($"{currentParticipant.Name} started to run away...\n");

            StartCoroutine(FleeRoutine());
        }
    }

    private IEnumerator FleeRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        
        bool result = BattleSystem.DetermineParticipantFleeing(currentParticipant, enemyCombatant.participant);

        if (!result)
        {
            battleLog.UpdateLog("But the escape route was cut off!\n");
            
            battleState = BattleState.Started;
        }
        else
        {
            soundManager.PlaySound(soundManager.run);
            
            battleLog.UpdateLog($"{currentParticipant.Name} escaped!\n");
            
            battleState = BattleState.Ended;
        }
        
        heroCommandsUI.SetActive(false);
    }

    public void SelectSpell()
    {
        if (battleState == BattleState.PlayerTurn)
        {
            battleState = BattleState.SpellSelect;
        }
    }
    
    public void UseSpell(Spell spell) {
        if (battleState == BattleState.SpellSelect 
            && currentParticipant.Stats.MagicPoints > spell.mpCost) {
            currentParticipant.Stats.MagicPoints -= spell.mpCost;
            
            var targets = spell.DetermineTarget(participants);
            
            spell.UseSpell(targets, battleLog);
            
            spellsUI.SetActive(false);

            heroCommandsUI.SetActive(false);
            
            heroStatsUI.RefreshHeroStats();
            
            soundManager.PlaySound(soundManager.castSpell);
            
            battleState = BattleState.Started;
        }
    }

    public void SelectItem()
    {
        if (battleState == BattleState.PlayerTurn)
        {
            battleState = BattleState.ItemSelect;
        }
    }

    public void UseItem(Item item) {
        if (battleState == BattleState.ItemSelect) {
            item.UseItem(playerCombatant.participant, battleLog);
            
            itemsUI.SetActive(false);
            
            heroCommandsUI.SetActive(false);
            
            heroStatsUI.RefreshHeroStats();
            
            battleState = BattleState.Started;
        }
    }

    private void Update() {
        if (battleState is BattleState.ItemSelect or BattleState.SpellSelect 
            && Input.GetKeyDown(KeyCode.Escape)) {
            itemsUI.SetActive(false);
            
            spellsUI.SetActive(false);

            battleState = BattleState.PlayerTurn;
        }
    }
}
