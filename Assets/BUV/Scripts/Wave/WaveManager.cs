using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject portalPrefab;
    public Transform[] spawnPoints;
    public Transform[] portalSpawnPoints;
    public TextMeshProUGUI victoryText;
    public TextMeshProUGUI gameOverText;
    public Transform player;

    public List<Transform> portals = new List<Transform>();

    private int enemiesSpawned = 0;
    private int maxEnemies = 5;
    private int activePortals = 3;
    private PlayerLife playerLife;

    void Start()
    {
        playerLife = player.GetComponent<PlayerLife>();

        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(false);
        }

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned in the inspector.");
            return;
        }

        if (portalPrefab == null)
        {
            Debug.LogError("Portal Prefab is not assigned in the inspector.");
            return;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned for enemies.");
            return;
        }

        if (portalSpawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned for portals.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player is not assigned in the inspector.");
            return;
        }

        SpawnPortals();
        StartCoroutine(SpawnEnemies());
    }

    void SpawnPortals()
    {
        for (int i = 0; i < activePortals; i++)
        {
            int spawnIndex = Random.Range(0, portalSpawnPoints.Length);
            GameObject portal = Instantiate(portalPrefab, portalSpawnPoints[spawnIndex].position, Quaternion.identity);
            Portal portalScript = portal.GetComponent<Portal>();
            portalScript.SetWaveManager(this);
            portals.Add(portal.transform);
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (enemiesSpawned < maxEnemies)
        {
            SpawnEnemy();
            enemiesSpawned++;
            yield return new WaitForSeconds(10f); // Attendre 10 secondes avant de faire apparaître une nouvelle IA
        }

        Debug.Log("Maximum number of enemies spawned. Stopping.");
    }

    void SpawnEnemy()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        GameObject enemy = Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        enemyAI.SetTarget(player);
        enemyAI.SetWaveManager(this);
        Debug.Log("Enemy spawned at: " + spawnPoints[spawnIndex].position);
    }

    public void PortalDestroyed()
    {
        activePortals--;

        if (activePortals <= 0)
        {
            GameOver();
        }
    }

    public void EnemyDefeated()
    {
        maxEnemies--;

        if (maxEnemies <= 0)
        {
            Victory();
        }
    }

    void Victory()
    {
        Debug.Log("Victory!");
        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(true);
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }
        if (playerLife != null)
        {
            playerLife.SetGameOver();
        }
    }
}
