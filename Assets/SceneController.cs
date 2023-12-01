using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public PlayerStats playerStats;

    private void OnEnable()
    {
        if (playerStats != null)
        {
            StartCoroutine(SetPlayerStatsAfterDelay(1f));
        }
    }

    private IEnumerator SetPlayerStatsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerStats.SetHealthAndOxygenToMax();
    }
}
