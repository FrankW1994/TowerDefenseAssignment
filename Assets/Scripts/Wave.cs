using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Wave
{
    public GameObject[] enemyPrefab; 
    public int spawnCount;          
    public float spawnInterval; 
}
