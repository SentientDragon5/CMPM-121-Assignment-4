using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class SpeedModifier : ModifierSpell
{
    private float speedMultiplier = 1.75f;
    private bool modifiersApplied = false;

    public SpeedModifier(Spell baseSpell, SpellCaster owner) : base(baseSpell, owner)
    {
    }

    protected override void InitializeAttributes()
    {
        base.InitializeAttributes();
        attributes.name = "Fast";
        attributes.description = "Faster projectile speed.";
    }

    protected override void ApplyModifiers()
    {
        if (modifiersApplied) return;

        var spellAttributes = GetBaseSpellAttributes();

        if (spellAttributes != null)
        {
            spellAttributes.speedModifiers.Add(new ValueModifier(ModifierType.Multiplicative, speedMultiplier));
            modifiersApplied = true;
        }
    }

    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);

        if (jObject["speed_multiplier"] != null)
        {
            if (float.TryParse(jObject["speed_multiplier"].ToString(), out float multiplier))
                speedMultiplier = multiplier;
        }

        ApplyModifiers();
    }
}