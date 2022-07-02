using UnityEngine;

public class TestScript : MonoBehaviour
{
    private void Awake()
    {
        SlimeBattleSystem.BattleSystem.SetRandomizationSeed("102");
    }
}
