using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class SlugModifier : ModifierSpell
{
    private float speedMultiplier = 0.4f;
    private float damageMultiplier = 2f;
    private bool modifiersApplied = false;

    public SlugModifier(Spell baseSpell, SpellCaster owner) : base(baseSpell, owner)
    {
    }

    protected override void InitializeAttributes()
    {
        base.InitializeAttributes();
        attributes.name = "Slug";
        attributes.description = "Significantly increased damage, but projectile speed is reduced by 60%.";
    }

    protected override void ApplyModifiers()
    {
        if (modifiersApplied) return;

        var spellAttributes = GetBaseSpellAttributes();

        if (spellAttributes != null)
        {
            spellAttributes.speedModifiers.Add(new ValueModifier(ModifierType.Multiplicative, speedMultiplier));
            spellAttributes.damageModifiers.Add(new ValueModifier(ModifierType.Multiplicative, damageMultiplier));
            modifiersApplied = true;
        }
    }

    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);

        if (jObject["speed_multiplier"] != null)
        {
            if (float.TryParse(jObject["speed_multiplier"].ToString(), out float speedMult))
                speedMultiplier = speedMult;
        }

        if (jObject["damage_multiplier"] != null)
        {
            string damageExpr = jObject["damage_multiplier"].ToString();

            // Handle RPN expressions like "2 wave 5 / +"
            var vars = new Dictionary<string, float> {
                { "power", owner.spellPower },
                { "wave", GameManager.Instance.wave }
            };
            damageMultiplier = FloatRPNEvaluator.Evaluate(damageExpr, vars);
        }

        ApplyModifiers();
    }
}