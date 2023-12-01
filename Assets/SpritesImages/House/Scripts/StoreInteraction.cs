using UnityEngine;

public class StoreInteraction : MonoBehaviour
{
    public GameObject shopGameObject; // Assign this in the inspector

    public void OnStoreClicked()
    {
        if (shopGameObject != null)
        {
            shopGameObject.SetActive(true); // Activate the shop
        }
        else
        {
            Debug.LogError("Shop GameObject is not assigned.");
        }
    }
}
