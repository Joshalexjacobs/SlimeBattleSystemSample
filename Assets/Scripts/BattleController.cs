using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CandyCoded;
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
        Ended
    }

    [SerializeField] private List<Combatant> combatants;

    [SerializeField] private List<Participant> participants;

    [SerializeField] private List<Participant> enemyParticipants;
        
    [SerializeField] private List<Participant> playerParticipants;

    [SerializeField] private Participant currentParticipant;

    [SerializeField] private HeroStatsUI HeroStatsUI;

    private bool _waitingForPlayerInput = false;

    [SerializeField] private BattleLog battleLog;

    private BattleState _battleState = BattleState.Pending;

    private void Start()
    {
        StartEncounter();
    }

    public void StartEncounter()
    {
        if (_battleState == BattleState.Pending)
        {
            _battleState = BattleState.Started;
            
            participants = combatants.Select(combatant => combatant.participant).ToList();
        
            participants = BattleSystem.DetermineTurnOrder(participants);
            
            playerParticipants = BattleSystem.GetPlayerParticipants(participants);

            enemyParticipants = BattleSystem.GetEnemyParticipants(participants);

            battleLog.UpdateLog($"A {enemyParticipants[0].name} draws near!\n");

            StartCoroutine(TraverseParticipants());   
        }
    }

    private IEnumerator TraverseParticipants()
    {
        yield return new WaitForSeconds(0.5f);
        
        foreach (var participant in participants)
        {
            currentParticipant = participant;

            if (participant.stats.hitPoints > 0 && !BattleSystem.IsBattleOver(participants))
            {

                if (participant.ParticipantType == ParticipantType.Enemy)
                {
                    _battleState = BattleState.EnemyTurn; 
                    
                    yield return StartCoroutine(HandleEnemyTurn(participant));
                } else if (participant.ParticipantType == ParticipantType.Player)
                {
                    _battleState = BattleState.PlayerTurn;
                    
                    battleLog.UpdateLog("Command?\n");
                    
                    _waitingForPlayerInput = true;
        
                    yield return new WaitUntil(() => !_waitingForPlayerInput);
                }   
            }

            yield return new WaitForSeconds(0.5f);
        }

        if (BattleSystem.IsBattleOver(participants))
        {
            _battleState = BattleState.Ended;

            if (BattleSystem.GetNumberOfRemainingParticipants(enemyParticipants) <= 0)
            {
                // player won
                
                battleLog.UpdateLog($"Thou hast done well in defeating the {enemyParticipants[0].name}.\n");

                yield return new WaitForSeconds(0.5f);
                
                var experiencePoints = BattleSystem.DetermineExperiencePoints(enemyParticipants);

                battleLog.UpdateLog($"Thy Experience increases by {experiencePoints}.\n");
                
                yield return new WaitForSeconds(0.5f);
                
                var goldPoints = BattleSystem.DetermineGoldPoints(enemyParticipants);

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
        else
        {
            StartCoroutine(TraverseParticipants());
        }
    }

    public void AttackEnemy()
    {
        if (_battleState == BattleState.PlayerTurn)
        {
            StartCoroutine(AttackEnemyRoutine());
        }
    }

    private IEnumerator AttackEnemyRoutine()
    {
        var target = enemyParticipants.Random();

        yield return StartCoroutine(HandleAttack(target));

        _battleState = BattleState.Started;
        
        _waitingForPlayerInput = false;
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
                
                break;
        }
        
        target.stats.hitPoints -= results.damage;
        
        HeroStatsUI.RefreshHeroStats();
        
        yield return new WaitForSeconds(0.5f);
    }
    
    private IEnumerator HandleEnemyTurn(Participant enemy)
    {
        yield return new WaitForSeconds(1f);

        var target = playerParticipants.Random();
            
        yield return StartCoroutine(HandleAttack(target));
    }
    
    public void Flee()
    {
        if (_battleState == BattleState.PlayerTurn)
        {
            battleLog.UpdateLog($"{currentParticipant.name} started to run away...\n");
            
            StartCoroutine(FleeRoutine(enemyParticipants));
        }
    }

    private IEnumerator FleeRoutine(List<Participant> runningFrom)
    {
        yield return new WaitForSeconds(0.5f);
        
        bool result = BattleSystem.DetermineParticipantFleeing(currentParticipant, 
            runningFrom.Where(participant => participant.stats.hitPoints > 0).ToList());

        if (!result)
        {
            battleLog.UpdateLog("But the escape route was cut off!\n");
            
            _battleState = BattleState.Started;
        }
        else
        {
            battleLog.UpdateLog($"{currentParticipant.name} escaped!\n");
            
            _battleState = BattleState.Ended;
        }
        
        _waitingForPlayerInput = false;
    }

    private void CastSpell()
    {
        if (_battleState == BattleState.PlayerTurn)
        {
            // select spell
            
            battleLog.UpdateLog($"{currentParticipant.name} chanted a spell...\n");
            
            _battleState = BattleState.Started;
        }
    }

    private void UseItem()
    {
        if (_battleState == BattleState.PlayerTurn)
        {
            // select item
            
            battleLog.UpdateLog($"{currentParticipant.name} used an item...\n");
            
            _battleState = BattleState.Started;
        }
    }

}
