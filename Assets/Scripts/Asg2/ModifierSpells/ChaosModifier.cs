using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class ChaosModifier : ModifierSpell
{
    private float damageMultiplier = 1.5f;
    private bool modifiersApplied = false;

    public ChaosModifier(Spell baseSpell, SpellCaster owner) : base(baseSpell, owner)
    {
    }

    protected override void InitializeAttributes()
    {
        base.InitializeAttributes();
        attributes.name = "Chaotic";
        attributes.description = "Significantly increased damage, but projectile is spiraling.";
    }

    protected override void ApplyModifiers()
    {
        if (modifiersApplied) return;

        var spellAttributes = GetBaseSpellAttributes();

        if (spellAttributes != null)
        {
            spellAttributes.damageModifiers.Add(new ValueModifier(ModifierType.Multiplicative, damageMultiplier));
            spellAttributes.trajectory = "spiraling";
            modifiersApplied = true;
        }
    }

    public override string GetTrajectory()
    {
        return "spiraling";
    }

    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);

        if (jObject["damage_multiplier"] != null)
        {
            string multExpr = jObject["damage_multiplier"].ToString();

            EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
            int currentWave = GameManager.Instance.wave;

            var vars = new Dictionary<string, float> {
                { "power", owner.spellPower },
                { "wave", currentWave }
            };
            damageMultiplier = FloatRPNEvaluator.Evaluate(multExpr, vars);
        }

        if (jObject["projectile_trajectory"] != null)
        {
            // Make sure we get the trajectory from JSON
            string trajectory = jObject["projectile_trajectory"].ToString();
            var spellAttributes = GetBaseSpellAttributes();
            if (spellAttributes != null)
            {
                spellAttributes.trajectory = trajectory;
            }
        }

        ApplyModifiers();
    }
}