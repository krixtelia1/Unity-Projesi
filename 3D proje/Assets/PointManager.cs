using System;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public List<Transform> pointLocations;

    [SerializeField] Puan pointPrefab;

    public static Action OnPointCollected;

    private int totalPoint;



    private void Start()
    {
        SpawnPoints();
    }

    private void IncreasePoint()
    { 
        totalPoint++; 
    }

    private void OnEnable()
    {
        OnPointCollected += IncreasePoint;
    }

    private void OnDisable()
    {
        OnPointCollected-= IncreasePoint;
    }

    public void SpawnPoints()
    {
        foreach (var location in pointLocations)
        {
            Instantiate(pointPrefab, location);
        }
    }


}
