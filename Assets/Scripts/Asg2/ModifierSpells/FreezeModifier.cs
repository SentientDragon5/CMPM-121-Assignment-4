using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class FreezeModifier : ModifierSpell
{
    private float damageMultiplier = 0.85f;
    private float manaMultiplier = 1.1f;
    private bool modifiersApplied = false;

    public FreezeModifier(Spell baseSpell, SpellCaster owner) : base(baseSpell, owner)
    {
    }

    protected override void InitializeAttributes()
    {
        base.InitializeAttributes();
        attributes.name = "Freeze";
        attributes.description = "Slows enemies hit by the projectile.";
    }

    protected override void ApplyModifiers()
    {
        if (modifiersApplied) return;

        var spellAttributes = GetBaseSpellAttributes();

        if (spellAttributes != null)
        {
            spellAttributes.damageModifiers.Add(new ValueModifier(ModifierType.Multiplicative, damageMultiplier));
            spellAttributes.manaCostModifiers.Add(new ValueModifier(ModifierType.Multiplicative, manaMultiplier));
            
            spellAttributes.damageType = "ice";
            
            modifiersApplied = true;
        }
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        last_cast = Time.time;

        // need custom logic to handle freeze effect for arcane spray due to complex cast logic
        if (baseSpell is ArcaneSpray)
        {
            yield return CastArcaneSprayWithFreeze(where, target, team);
        }
        else
        {
            yield return CastSimpleSpellWithFreeze(where, target, team);
        }
    }

    private IEnumerator CastArcaneSprayWithFreeze(Vector3 where, Vector3 target, Hittable.Team team)
    {
        // Copy ArcaneSpray logic but with freeze effect
        Vector3 direction = (target - where).normalized;
        var spellAttributes = GetBaseSpellAttributes();
        int numProjectiles = spellAttributes.GetFinalNumProjectiles();

        float sprayAngle = spellAttributes.spray.HasValue ? spellAttributes.spray.Value * 360f : 30f;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - (sprayAngle / 2);
        float angleIncrement = sprayAngle / (numProjectiles - 1);

        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = startAngle + (angleIncrement * i);
            Vector3 projectileDirection = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0f
            );

            float lifetime = spellAttributes.lifetime.HasValue ? spellAttributes.lifetime.Value : 0.5f;

            GameManager.Instance.projectileManager.CreateProjectile(
                spellAttributes.projectileSprite,
                GetTrajectory(),
                where,
                projectileDirection,
                GetSpeed(),
                OnFreezeHit, 
                lifetime,
                GetSize()
            );

            yield return new WaitForSeconds(0.02f);
        }
    }

    private IEnumerator CastSimpleSpellWithFreeze(Vector3 where, Vector3 target, Hittable.Team team)
    {
        Vector3 direction = target - where;

        GameManager.Instance.projectileManager.CreateProjectile(
            GetIcon(),
            GetTrajectory(),
            where,
            direction,
            GetSpeed(),
            OnFreezeHit,
            lifetime,
            GetSize()
        );

        yield return new WaitForEndOfFrame();
    }

    private void OnFreezeHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            int damage = GetDamage();
            Damage.Type damageType = Damage.TypeFromString("ice");
            other.Damage(new Damage(damage, damageType));
            GameManager.Instance.totalDamageDealt += damage;

            var enemyController = other.owner.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.ApplySlow(2f, 0.3f);
            }
        }
    }

    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);

        if (jObject["damage_multiplier"] != null)
        {
            if (float.TryParse(jObject["damage_multiplier"].ToString(), out float multiplier))
                damageMultiplier = multiplier;
        }

        if (jObject["mana_multiplier"] != null)
        {
            if (float.TryParse(jObject["mana_multiplier"].ToString(), out float multiplier))
                manaMultiplier = multiplier;
        }

        ApplyModifiers();
    }
}