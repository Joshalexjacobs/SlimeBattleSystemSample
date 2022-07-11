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
        heroNameText.text = playerParticipant.Name;
        
        levelText.text = playerParticipant.Stats.Level.ToString();
        
        hpText.text = playerParticipant.Stats.HitPoints.ToString();
        
        mpText.text = playerParticipant.Stats.MagicPoints.ToString();
        
        gpText.text = playerParticipant.GoldPoints.ToString();
        
        xpText.text = playerParticipant.ExperiencePoints.ToString();
    }
    
}
