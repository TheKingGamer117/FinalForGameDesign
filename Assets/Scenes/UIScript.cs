using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject shopGameObject; // Reference to the shop GameObject
    public GameObject[] buttonsToHide; // Array to hold references to buttons

    void Update()
    {
        // Loop through each button and set its visibility
        foreach (var button in buttonsToHide)
        {
            if (button != null)
            {
                button.SetActive(!shopGameObject.activeSelf);
            }
        }
    }
}
