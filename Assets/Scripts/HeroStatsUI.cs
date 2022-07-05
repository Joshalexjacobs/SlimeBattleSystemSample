using System;
using UnityEngine;
using UnityEngine.UI;

public class HeroStatsUI : MonoBehaviour
{

    [SerializeField] private Text heroNameText;
    
    [SerializeField] private Text levelText;
    
    [SerializeField] private Text hpText;
    
    [SerializeField] private Text mpText;
    
    [SerializeField] private Text gpText;
    
    [SerializeField] private Text xpText;

    [SerializeField] private Combatant playerCombatant;

    private void Start()
    {
        RefreshHeroStats();
    }

    public void RefreshHeroStats()
    {
        heroNameText.text = playerCombatant.participant.name;
        
        // levelText.text = playerCombatant.participant.stats;
        
        hpText.text = playerCombatant.participant.stats.hitPoints.ToString();
        
        mpText.text = playerCombatant.participant.stats.magicPoints.ToString();
        
        gpText.text = playerCombatant.participant.goldPoints.ToString();
        
        xpText.text = playerCombatant.participant.experiencePoints.ToString();
    }
    
}
