using UnityEngine;

using System.Collections;
using Newtonsoft.Json.Linq;

public class FrostShard : Spell
{
    public FrostShard(SpellCaster owner) : base(owner)
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

        Vector3 direction = target - where;

        GameManager.Instance.projectileManager.CreateProjectile(
            attributes.projectileSprite,
            GetTrajectory(),
            where,
            direction,
            GetSpeed(),
            OnFrostHit,
            lifetime,
            GetSize()
        );

        yield return new WaitForEndOfFrame();
    }

    private void OnFrostHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            int baseDamage = GetDamage();
            int finalDamage = baseDamage;

            if (attributes.critChance.HasValue && attributes.critMultiplier.HasValue)
            {
                if (UnityEngine.Random.value < attributes.critChance.Value)
                {
                    finalDamage = Mathf.RoundToInt(baseDamage * attributes.critMultiplier.Value);
                    Debug.Log($"Critical hit! Dealt {finalDamage} damage instead of {baseDamage}.");
                }
            }

            Damage.Type damageType = Damage.TypeFromString(attributes.damageType);
            other.Damage(new Damage(finalDamage, damageType));
            GameManager.Instance.totalDamageDealt += finalDamage;

            var enemyController = other.owner.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                // Apply slow- 50% speed reduction for 1.5 seconds
                enemyController.ApplySlow(1.5f, 0.5f);
            }
        }
    }

    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);

        if (jObject["crit"] != null)
        {
            JObject critObj = jObject["crit"] as JObject;
            
            if (critObj["chance"] != null)
            {
                if (float.TryParse(critObj["chance"].ToString(), out float chance))
                    attributes.critChance = chance;
            }

            if (critObj["damage_multiplier"] != null)
            {
                if (float.TryParse(critObj["damage_multiplier"].ToString(), out float multiplier))
                    attributes.critMultiplier = multiplier;
            }
        }
    }
}
