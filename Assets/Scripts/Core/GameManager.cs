using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class GameManager
{
    public enum GameState
    {
        PREGAME,
        INWAVE,
        WAVEEND,
        COUNTDOWN,
        GAMEOVER,
        VICTORY
    }
    public GameState state;

    public int countdown;
    private static GameManager theInstance;
    public static GameManager Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new GameManager();
            return theInstance;
        }
    }

    public GameObject player;

    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public EnemySpriteManager enemySpriteManager;
    public PlayerSpriteManager playerSpriteManager;
    public RelicIconManager relicIconManager;
    public SpellBuilder spellBuilder;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }

    public float waveStartTime;
    public int totalDamageDealt;
    public int totalDamageTaken;
    public int wave;
    public UnityEvent onNextWave = new();
    public UnityEvent onWaveStart = new();
    public UnityEvent onWaveEnd = new();

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a, b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }

    private GameManager()
    {
        enemies = new List<GameObject>();
        spellBuilder = new();
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        state = GameState.GAMEOVER;

        // Disable player controls
        if (player != null)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.enabled = false;
            }
        }
    }

    public void Victory()
    {
        Debug.Log("Victory!");
        state = GameState.VICTORY;

        if (player != null)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.enabled = false;
            }
        }
    }

    public void Reset()
    {
        foreach (GameObject enemy in new List<GameObject>(enemies))
        {
            GameObject.Destroy(enemy);
        }
        enemies.Clear();

        state = GameState.PREGAME;
        totalDamageDealt = 0;
        totalDamageTaken = 0;
    }
}
