using SlimeBattleSystem;
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
    
    [SerializeField] private BattleController battleController;
    
    private Participant playerParticipant;

    private void Start() {
        playerParticipant = battleController.GetPlayerParticipant();
        
        RefreshHeroStats();
    }

    public void RefreshHeroStats()
    {
        heroNameText.text = playerParticipant.name;
        
        levelText.text = playerParticipant.stats.level.ToString();
        
        hpText.text = playerParticipant.stats.hitPoints.ToString();
        
        mpText.text = playerParticipant.stats.magicPoints.ToString();
        
        gpText.text = playerParticipant.goldPoints.ToString();
        
        xpText.text = playerParticipant.experiencePoints.ToString();
    }
    
}
