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

    [SerializeField] private List<Participant> participants;
    
    [SerializeField] private Participant enemy;
    
    [SerializeField] private Participant player;
    
    [SerializeField] private Participant currentParticipant;

    [SerializeField] private HeroStatsUI heroStatsUI;

    [SerializeField] private GameObject heroCommandsUI;

    [SerializeField] private GameObject spellsUI;
    
    [SerializeField] private GameObject itemsUI;

    [SerializeField] private BattleLog battleLog;

    [SerializeField] private ScreenShake screenShake;
        
    [SerializeField] private SoundManager soundManager;

    private BattleState battleState = BattleState.Pending;

    private void Start()
    {
        if (battleState == BattleState.Pending)
        {
            battleState = BattleState.Started;
            
            participants = combatants.Select(combatant => combatant.participant).ToList();
        
            participants = BattleSystem.DetermineTurnOrder(participants);
            
            player = BattleSystem.GetPlayerParticipants(participants)[0];
            
            enemy = BattleSystem.GetEnemyParticipants(participants)[0];

            battleLog.UpdateLog($"A {enemy.name} draws near!\n");

            StartCoroutine(TraverseParticipants());   
        }
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

            if (participant.stats.hitPoints > 0 && !BattleSystem.IsBattleOver(participants) && battleState != BattleState.Ended)
            {

                if (participant.ParticipantType == ParticipantType.Enemy)
                {
                    battleState = BattleState.EnemyTurn; 
                    
                    yield return StartCoroutine(HandleEnemyTurn());
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

            if (enemy.stats.hitPoints <= 0)
            {
                // player won
                
                soundManager.PlaySound(soundManager.battleOver);
                
                var enemyCombatant = BattleUtil.GetMatchingCombatant(enemy, combatants);
                
                enemyCombatant.OnDefeat.Invoke();
                
                battleLog.UpdateLog($"Thou hast done well in defeating the {enemy.name}.\n");

                yield return new WaitForSeconds(0.5f);
                
                // TODO: swap out for single participant call
                var experiencePoints = BattleSystem.DetermineExperiencePoints(new List<Participant>() { enemy });

                battleLog.UpdateLog($"Thy Experience increases by {experiencePoints}.\n");
                
                yield return new WaitForSeconds(0.5f);
                
                // TODO: swap out for single participant call
                var goldPoints = BattleSystem.DetermineGoldPoints(new List<Participant>() { enemy });

                if (goldPoints > 0)
                {
                    battleLog.UpdateLog($"Thy GOLD increases by {goldPoints}.\n");
                    
                    yield return new WaitForSeconds(0.5f);
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
        
        yield return StartCoroutine(HandleAttack(enemy));

        battleState = BattleState.Started;
    }

    private IEnumerator HandleAttack(Participant target)
    {
        var results = BattleSystem.DetermineAttackDamage(currentParticipant, target);

        battleLog.UpdateLog($"{currentParticipant.name} attacks!.\n");

        yield return new WaitForSeconds(0.5f);

        switch (results.attackType)
        {
            case AttackResults.AttackType.Hit:
                battleLog.UpdateLog($"{target.name}'s Hit Points have been reduced by {results.damage}.\n");

                break;
            case AttackResults.AttackType.CriticalHit:
                battleLog.UpdateLog($"Crit!! {target.name} took {results.damage} damage!\n");
                
                break;
            
            case AttackResults.AttackType.Missed:
                battleLog.UpdateLog($"Missed! {target.name} dodged the attack.\n");
                
                soundManager.PlaySound(soundManager.miss);
                
                break;
        }

        if (results.damage > 0) {
            soundManager.PlaySound(soundManager.hit);
            
            var targetCombatant = BattleUtil.GetMatchingCombatant(target, combatants);
        
            targetCombatant.OnHurt.Invoke();

            if (target == player) {
                screenShake.TriggerShake();   
            }
        }

        target.stats.hitPoints -= results.damage;
        
        heroStatsUI.RefreshHeroStats();
        
        yield return new WaitForSeconds(0.5f);
    }
    
    private IEnumerator HandleEnemyTurn()
    {
        yield return new WaitForSeconds(1f);
            
        yield return StartCoroutine(HandleAttack(player));
    }
    
    public void Flee()
    {
        if (battleState == BattleState.PlayerTurn)
        {
            battleLog.UpdateLog($"{currentParticipant.name} started to run away...\n");
            
            // TODO: swap out for single participant call
            StartCoroutine(FleeRoutine(new List<Participant>() { enemy }));
        }
    }

    private IEnumerator FleeRoutine(List<Participant> runningFrom)
    {
        yield return new WaitForSeconds(0.5f);
        
        bool result = BattleSystem.DetermineParticipantFleeing(currentParticipant, enemy, BattleSystem.Random);

        if (!result)
        {
            battleLog.UpdateLog("But the escape route was cut off!\n");
            
            battleState = BattleState.Started;
        }
        else
        {
            soundManager.PlaySound(soundManager.run);
            
            battleLog.UpdateLog($"{currentParticipant.name} escaped!\n");
            
            battleState = BattleState.Ended;
        }
        
        heroCommandsUI.SetActive(false);
    }

    public void SelectSpell()
    {
        if (battleState == BattleState.PlayerTurn)
        {
            // select spell
            
            // battleLog.UpdateLog($"{currentParticipant.name} chanted a spell...\n");
            
            battleState = BattleState.SpellSelect;
        }
    }
    
    public void UseSpell(Spell spell) {
        if (battleState == BattleState.ItemSelect) {
            // item.UseItem(player);
            //
            // ItemsUI.SetActive(false);
            //
            // HeroCommandsUI.SetActive(false);
            //
            // HeroStatsUI.RefreshHeroStats();
            
            soundManager.PlaySound(soundManager.castSpell);
            
            battleLog.UpdateLog($"{currentParticipant.name} chanted a spell...\n");
            
            battleState = BattleState.Started;
        }
    }

    public void SelectItem()
    {
        if (battleState == BattleState.PlayerTurn)
        {
            // select item
            
            // battleLog.UpdateLog($"{currentParticipant.name} used an item...\n");
            
            battleState = BattleState.ItemSelect;
        }
    }

    public void UseItem(Item item) {
        if (battleState == BattleState.ItemSelect) {
            item.UseItem(player);
            
            itemsUI.SetActive(false);
            
            heroCommandsUI.SetActive(false);
            
            heroStatsUI.RefreshHeroStats();
            
            battleLog.UpdateLog($"{currentParticipant.name} used a {item.name}!\n");
            
            battleState = BattleState.Started;
        }
    }

}
