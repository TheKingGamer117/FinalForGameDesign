using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SwimPointsData", menuName = "ScriptableObjects/SwimPointsData", order = 1)]
public class SwimPointsData : ScriptableObject
{
    public List<Vector3> points;
}
