using TMPro;
using UnityEngine;

public class GoldUIUpdater : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    private void OnEnable()
    {
        //Debug.Log("[GoldUIUpdater] OnEnable called. Checking references...");
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("[GoldUIUpdater] PlayerDataManager.Instance is null.");
        }
        else if (PlayerDataManager.Instance.playerData == null)
        {
            Debug.LogError("[GoldUIUpdater] PlayerDataManager.Instance.playerData is null.");
        }
        else if (goldText == null)
        {
            Debug.LogError("[GoldUIUpdater] goldText is null.");
        }
        else
        {
            PlayerDataManager.Instance.OnGoldChanged += UpdateGoldText;
            UpdateGoldText(PlayerDataManager.Instance.playerData.gold); // Initial update
        }
    }


    private void OnDisable()
    {
        PlayerDataManager.Instance.OnGoldChanged -= UpdateGoldText;
       // Debug.Log("[GoldUIUpdater] Ondisable called. PlayerDataManager Instance is " + (PlayerDataManager.Instance != null ? "set." : "null."));

    }

    public void UpdateGoldText(int gold)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {gold}";
        }
        else
        {
            Debug.LogError("goldText is not assigned in GoldUIUpdater");
        }
    }

}
