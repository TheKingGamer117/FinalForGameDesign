using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class FishSwimPointMaster : MonoBehaviour
{
    public int gridRows = 5;
    public int gridColumns = 5;
    public float areaWidth = 100f;  // Width of the area along the X-axis
    public float areaHeight = 50f;  // Height of the area along the Y-axis
    public SwimPointsData swimPointsData;

    void Start()
    {
        LoadSwimPoints();
    }

    public void SaveSwimPointsToScriptableObject(SwimPointsData data)
    {
        GenerateSwimPoints();
        data.points = new List<Vector3>();

        foreach (Transform child in transform)
        {
            data.points.Add(child.position);
        }

        // Save the ScriptableObject
#if UNITY_EDITOR
        EditorUtility.SetDirty(data);
#endif
    }

    void GenerateSwimPoints()
    {
        float rowSpacing = areaHeight / gridRows;
        float columnSpacing = areaWidth / gridColumns;
        int pointsCreated = 0;

        for (int row = 0; row < gridRows; row++)
        {
            for (int column = 0; column < gridColumns; column++)
            {
                Vector3 point = new Vector3(
                    transform.position.x - areaWidth / 2 + column * columnSpacing,
                    transform.position.y - areaHeight / 2 + row * rowSpacing,
                    transform.position.z // Keep z-axis fixed for 2D
                );

                if (IsPointOnNavMesh(point))
                {
                    CreateSwimPoint(row * gridColumns + column, point);
                    pointsCreated++;
                }
                else
                {
                    //Debug.Log("Point not on NavMesh: " + point);
                }
            }
        }

        //Debug.Log("Total Swim Points Created: " + pointsCreated);
    }

    void CreateSwimPoint(int index, Vector3 position)
    {
        GameObject swimPoint = new GameObject("SwimPoint_" + index);
        swimPoint.transform.position = position;
        swimPoint.transform.parent = this.transform;
    }

    bool IsPointOnNavMesh(Vector3 point)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point, out hit, 1.0f, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaWidth, areaHeight, 1));
    }

    void LoadSwimPoints()
    {
        if (swimPointsData != null)
        {
            for (int i = 0; i < swimPointsData.points.Count; i++)
            {
                CreateSwimPoint(i, swimPointsData.points[i]);
            }
        }
        else
        {
            Debug.LogError("SwimPointsData not set.");
        }
    }

    public void SavePoints()
    {
        SaveSwimPointsToScriptableObject(swimPointsData);
    }

}