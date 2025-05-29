using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] spawnPoints;
    public Transform enemyParent;

    /// <summary>
    /// Find the spawn point by the name. This uses the lowercase name of the enum, that should match in levels.json
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public SpawnPoint FindSpawnPoint(string name) => spawnPoints.ToList().Find((SpawnPoint s) => s.StringName == name);

    public int level;
    public int wave;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        level = 0;
        wave = 0; // waves starts at 0
        var levels = DataLoader.Instance.levels;
        var yPos = 130;
        foreach (var level in levels)
        {
            GameObject selector = Instantiate(button, level_selector.transform);
            selector.transform.localPosition = new Vector3(0, yPos);
            yPos -= 50;
            selector.GetComponent<MenuSelectorController>().spawner = this;
            selector.GetComponent<MenuSelectorController>().SetLevel(level.name);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartLevel(string levelname)
    {
        level_selector.gameObject.SetActive(false);
        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        level = DataLoader.Instance.FindLevelIndex(levelname);
        StartCoroutine(SpawnWave());
    }

    public void NextWave()
    {
        wave++;

        if (DataLoader.Instance.levels[level].waves.HasValue &&
            wave > DataLoader.Instance.levels[level].waves.Value)
        {
            // Can only win in non endless mode
            if (DataLoader.Instance.levels[level].name != "Endless")
            {
                GameManager.Instance.Victory();
                return;
            }
        }

        StartCoroutine(SpawnWave());

        GameManager.Instance.wave = wave;
        GameManager.Instance.onNextWave.Invoke();
    }



    IEnumerator SpawnWave()
    {
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        GameManager.Instance.waveStartTime = Time.time;
        GameManager.Instance.totalDamageDealt = 0;
        GameManager.Instance.totalDamageTaken = 0;
        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }
        GameManager.Instance.onWaveStart.Invoke();
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        var levels = DataLoader.Instance.levels;
        var spawns = levels[level].spawns;

        for (int s = 0; s < spawns.Count; s++)
        {
            var spawn = spawns[s];
            Dictionary<string, int> rpnArgs = new();
            rpnArgs.Add("wave", wave);

            int count = spawn.EvalCount(rpnArgs);

            Enemy enemy = DataLoader.Instance.FindEnemy(spawn.enemy);

            rpnArgs.Add("base", enemy.hp);
            int hp = spawn.EvalHp(rpnArgs);

            rpnArgs.Remove("base");
            rpnArgs.Add("base", enemy.damage);
            int damage = spawn.EvalDamage(rpnArgs);

            List<bool> delays = new List<bool>();
            for (int i = 0; i < spawn.sequence.Count; i++)
            {
                for (int j = 0; j < spawn.sequence[i] - 1; j++)
                    delays.Add(false);
                delays.Add(true);
            }

            for (int i = 0; i < count; ++i)
            {
                string location = spawn.Locations[i % spawn.Locations.Count];

                bool delayed = delays[i % delays.Count];

                yield return SpawnEnemy(enemy, location, delayed, hp, damage);
            }
        }

        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
        GameManager.Instance.onWaveEnd.Invoke();
    }

    const float spawnDelay = 0.5f;

    IEnumerator SpawnEnemy(Enemy enemyInfo, string location, bool delayed, int? hpOverride=null, int? damageOverride=null, int? speedOverride=null)
    {
        SpawnPoint spawn_point = location == "random" ? spawnPoints[Random.Range(0, spawnPoints.Length)] : FindSpawnPoint(location);
        Vector2 offset = Random.insideUnitCircle * 1.8f;
                
        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
        SpawnEnemyAtPosition(enemyInfo, initial_position, hpOverride, damageOverride, speedOverride);

        if (delayed)
            yield return new WaitForSeconds(spawnDelay);
        else
            yield return null;
    }

    public GameObject SpawnEnemyAtPosition(Enemy enemyInfo, Vector3 initial_position, int? hpOverride=null, int? damageOverride=null, int? speedOverride=null)
    {
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity, enemyParent);
        new_enemy.name = "Enemy ("+ enemyInfo.name +")";

        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(enemyInfo.sprite);
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(hpOverride==null ? enemyInfo.hp : (int)hpOverride, Hittable.Team.MONSTERS, new_enemy);
        en.speed = speedOverride==null? enemyInfo.speed : (int)speedOverride;
        en.damage = damageOverride ==null? enemyInfo.damage : (int)damageOverride;
        en.child = enemyInfo.child;
        en.childNum = enemyInfo.childNum;
        en.childWhen = enemyInfo.childWhen;
        GameManager.Instance.AddEnemy(new_enemy);

        return new_enemy;
    }
}
