using System.Collections.Generic;
using UnityEngine;

public class UniqueLayerOrderByType : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bodyRenderer;  // Reference to the SpriteRenderer of the fish's body
    [SerializeField] private string fishType;  // The type of fish this is, e.g., "Dolphin"

    // Holds the next order in layer for each fish type
    private static Dictionary<string, int> nextOrderInLayerByType = new Dictionary<string, int>();

    private void Start()
    {
        // Initialize the next order in layer for this fish type if it hasn't been initialized yet
        if (!nextOrderInLayerByType.ContainsKey(fishType))
        {
            nextOrderInLayerByType[fishType] = 5;
        }

        // Get the next order in layer for this fish type
        int nextOrderInLayer = nextOrderInLayerByType[fishType];

        // Check if bodyRenderer is assigned
        if (bodyRenderer == null)
        {
            Debug.LogError("bodyRenderer is not assigned in the inspector.");
            return;
        }

        // Assign unique sorting orders
        bodyRenderer.sortingOrder = nextOrderInLayer;

        // Increment the next order in layer for this fish type
        nextOrderInLayerByType[fishType] = nextOrderInLayer + 5;
    }
}
