using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class where all enemies are spawned and retreated. Enemies
/// can be spawned from spawning areas in between bosses, 
/// spawn points set up for bosses, or points of the environment
/// that are hit by player projectiles.
/// </summary>
public class EnemySpawner : Singleton<EnemySpawner>
{
    public int maxEnemies;
    private int enemyCount;

    public bool canSpawn
    {
        get
        {
            return enemyCount < maxEnemies;
        }
    }

    public void SpawnEnemy (GameObject enemyType, Vector3 position, Quaternion rotation)
    {
        if (enemyCount == maxEnemies)
        {
            return;
        }
        UpdateEnemyCount(1);
        GameObject enemy = Instantiate(enemyType, position, rotation) as GameObject;
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        enemyMovement.Init();
    }

    public void UpdateEnemyCount (int toAdd)
    {
        enemyCount += toAdd;
    }
}
