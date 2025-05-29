using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class HomingModifier : ModifierSpell
{
    private float damageMultiplier = 0.75f;
    private float manaAdder = 10f;
    private bool modifiersApplied = false;

    public HomingModifier(Spell baseSpell, SpellCaster owner) : base(baseSpell, owner)
    {
    }

    protected override void InitializeAttributes()
    {
        base.InitializeAttributes();
        attributes.name = "Homing";
        attributes.description = "Homing projectile, with decreased damage and increased mana cost.";
    }

    protected override void ApplyModifiers()
    {
        if (modifiersApplied) return;

        var spellAttributes = GetBaseSpellAttributes();

        if (spellAttributes != null)
        {
            spellAttributes.damageModifiers.Add(new ValueModifier(ModifierType.Multiplicative, damageMultiplier));
            spellAttributes.manaCostModifiers.Add(new ValueModifier(ModifierType.Additive, manaAdder));
            spellAttributes.trajectory = "homing";
            modifiersApplied = true;
        }
    }

    public override string GetTrajectory()
    {
        return "homing";
    }

    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);

        if (jObject["damage_multiplier"] != null)
        {
            if (float.TryParse(jObject["damage_multiplier"].ToString(), out float multiplier))
                damageMultiplier = multiplier;
        }

        if (jObject["mana_adder"] != null)
        {
            if (float.TryParse(jObject["mana_adder"].ToString(), out float adder))
                manaAdder = adder;
        }

        ApplyModifiers();
    }
}