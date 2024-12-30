using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float gameTime;
    public Text timerText; // Timer text
    public Text surplusEnemyText; // Remaining enemies text
    public List<GameObject> enemyList; // Enemy list
    public float enemySpawnInterval; // Enemy spawn interval
    public Transform[] enemySpawnPoints; // Enemy spawn points

    public GameObject gameOverPanel; // Game over panel
    public Text gameOverText; // Game over text

    public int enemyCount; // Total number of enemies

    private float _lastSpawnTime; // Time of last enemy spawn
    private int _currentEnemyCount; // Current number of spawned enemies
    private int _surplusEnemyCount; // Remaining enemies count
    private void Awake()
    {
        Instance = this;
    }

public void Start()
{
    surplusEnemyText.text = "Enemy Remaining: " + enemyCount;
    timerText.text = "Time: " + gameTime.ToString("F2");
    _surplusEnemyCount = enemyCount;
}

private void Update()
{
    if (Time.timeScale == 0)
    {
        return;
    }
    // Timer logic
    gameTime -= Time.deltaTime;
    timerText.text = "Time: " + gameTime.ToString("F2");
    if (gameTime <= 0)
    {
        GameOver("Time's up! Game over!");
    }
    
    // Spawn enemies
    if (Time.time - _lastSpawnTime > enemySpawnInterval && _currentEnemyCount < enemyCount)
    {
        _lastSpawnTime = Time.time;
        _currentEnemyCount++;
        // Randomly spawn an enemy
        int index = UnityEngine.Random.Range(0, enemySpawnPoints.Length);
        Instantiate(enemyList[UnityEngine.Random.Range(0, enemyList.Count)], enemySpawnPoints[index].position, Quaternion.identity);
    }

    // Press Esc to return to the start menu
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("StartScene");
    }
}

// Enemy defeated
public void EnemyDead()
{
    _surplusEnemyCount--;
    surplusEnemyText.text = "Enemy Remaining: " + _surplusEnemyCount;
    if (_surplusEnemyCount <= 0)
    {
        GameOver("All enemies defeated! Victory!");
    }
}

// Return to the start menu
public void Restart()
{
    Time.timeScale = 1;
    SceneManager.LoadScene("StartScene");
}

// End the game
public void GameOver(string reason)
{
    Time.timeScale = 0;
    gameOverPanel.SetActive(true);
    gameOverText.text = reason;
}

}
