using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyType;
    public int amount;
}

public class EnemySpawnZone : MonoBehaviour
{
    public Renderer newRenderer;
    public Collider newCollider;
    public Transform newTransform;
    private float radius;

    void Start()
    {
        newRenderer = GetComponent<Renderer>();
        newCollider = GetComponent<Collider>();
        newTransform = GetComponent<Transform>();
        //newRenderer.enabled = false;
        radius = newTransform.localScale.x;
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        if (EnemySpawner.instance.canSpawn)
        {
            newCollider.enabled = false;
            SpawnEnemies();
        }
    }

    public EnemySpawnInfo[] enemySpawns;

    void SpawnEnemies ()
    {
        foreach (EnemySpawnInfo spawnInfo in enemySpawns)
        {
            for (int i = 0; i < spawnInfo.amount * 2; i++)
            {
                SpawnEnemy(spawnInfo.enemyType);
            }
        }
    }

    public LayerMask terrainMask;

    void SpawnEnemy (GameObject enemyType)
    {
        Vector2 randomPointInCircle = Random.insideUnitCircle * (radius * (1f/4f));
        Vector3 randomSpawnPoint = new Vector3(randomPointInCircle.x, 0, randomPointInCircle.y);
        randomSpawnPoint += Player.instance.positionController.transform.position;
        Vector3 rayOrigin = Player.instance.positionController.transform.position + Vector3.up * 100000f;
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, terrainMask))
        {
            randomSpawnPoint.y = hit.point.y;
            EnemySpawner.instance.SpawnEnemy(enemyType, randomSpawnPoint, Quaternion.identity);
        }
        else
        {
            return;
        }
    }
}
