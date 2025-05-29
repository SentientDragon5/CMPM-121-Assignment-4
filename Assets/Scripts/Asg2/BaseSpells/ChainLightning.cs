// Create a new file called ChainLightning.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class ChainLightning : Spell
{
    public ChainLightning(SpellCaster owner) : base(owner)
    {
    }
    protected override void InitializeAttributes()
    {
        attributes = new SpellAttributes();
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        last_cast = Time.time;

        GameManager.Instance.projectileManager.CreateProjectile(
            attributes.projectileSprite,
            GetTrajectory(),
            where,
            target - where,
            GetSpeed(),
            (hit, pos) => OnInitialHit(hit, pos),
            lifetime,
            GetSize()
        );

        yield return new WaitForEndOfFrame();
    }

    private void OnInitialHit(Hittable hit, Vector3 pos)
    {
        if (hit == null || hit.team == team) return;

        // Deal damage
        int damage = GetDamage();
        hit.Damage(new Damage(damage, Damage.TypeFromString(attributes.damageType)));
        GameManager.Instance.totalDamageDealt += damage;

        // Get chain parameters
        int chainCount = attributes.chainCount.HasValue ? attributes.chainCount.Value : 3;
        float chainRange = attributes.chainRange.HasValue ? attributes.chainRange.Value : 5f;
        float damageDecay = attributes.chainDamageDecay.HasValue ? attributes.chainDamageDecay.Value : 0.7f;

        // Start chain process
        StartChainToNextTarget(hit.owner, pos, damage, chainCount, chainRange, damageDecay);
    }

    private void StartChainToNextTarget(GameObject currentTarget, Vector3 pos, int currentDamage, int chainsRemaining, float chainRange, float damageDecay)
    {
        if (chainsRemaining <= 0) return;

        GameObject playerObj = GameManager.Instance.player;
        if (playerObj != null)
        {
            MonoBehaviour coroutineRunner = playerObj.GetComponent<MonoBehaviour>();
            if (coroutineRunner != null)
            {
                coroutineRunner.StartCoroutine(ChainToNextTarget(currentTarget, pos, currentDamage, chainsRemaining, chainRange, damageDecay));
            }
        }
    }

    private IEnumerator ChainToNextTarget(GameObject currentTarget, Vector3 pos, int currentDamage, int chainsRemaining, float chainRange, float damageDecay)
    {
        GameObject nearestEnemy = null;
        float nearestDistance = chainRange;

        foreach (GameObject enemyObj in GameObject.FindGameObjectsWithTag("unit"))
        {
            if (enemyObj == currentTarget) continue;

            EnemyController enemy = enemyObj.GetComponent<EnemyController>();
            if (enemy == null || enemy.hp == null || enemy.hp.team == team) continue;

            float dist = Vector3.Distance(pos, enemy.transform.position);
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestEnemy = enemy.gameObject;
            }
        }

        if (nearestEnemy != null)
        {
            yield return new WaitForSeconds(0.2f); // Delay between chains

            // Visual lightning effect between positions
            CreateLightningEffect(pos, nearestEnemy.transform.position);

            // Calculate reduced damage for next chain
            int nextDamage = Mathf.RoundToInt(currentDamage * damageDecay);

            // Hit the next target
            var enemyController = nearestEnemy.GetComponent<EnemyController>();
            if (enemyController != null && enemyController.hp != null)
            {
                // Deal damage to the chained target
                enemyController.hp.Damage(new Damage(nextDamage, Damage.TypeFromString(attributes.damageType)));
                GameManager.Instance.totalDamageDealt += nextDamage;

                // Continue chain (with one less remaining)
                StartChainToNextTarget(nearestEnemy, nearestEnemy.transform.position, nextDamage, chainsRemaining - 1, chainRange, damageDecay);
            }
        }
    }

    private void CreateLightningEffect(Vector3 start, Vector3 end)
    {
        // Create a line renderer for the lightning effect
        GameObject lineObj = new GameObject("ChainLightning");
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        // Basic lightning material (white/blue)
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.material.color = new Color(0.6f, 0.8f, 1.0f);

        // Destroy after a short time
        GameObject.Destroy(lineObj, 0.3f);
    }

    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);

        if (jObject["chain_count"] != null)
        {
            if (int.TryParse(jObject["chain_count"].ToString(), out int chainCount))
                attributes.chainCount = chainCount;
        }

        if (jObject["chain_damage_decay"] != null)
        {
            if (float.TryParse(jObject["chain_damage_decay"].ToString(), out float decay))
                attributes.chainDamageDecay = decay;
        }

        if (jObject["chain_range"] != null)
        {
            if (float.TryParse(jObject["chain_range"].ToString(), out float range))
                attributes.chainRange = range;
        }
    }
}