using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class HugeModifier : ModifierSpell
{
    private float sizeMultiplier = 2f;
    private bool modifiersApplied = false;

    public HugeModifier(Spell baseSpell, SpellCaster owner) : base(baseSpell, owner)
    {
    }

    protected override void InitializeAttributes()
    {
        base.InitializeAttributes();
        attributes.name = "Huge";
        attributes.description = "Makes the spell huge.";
    }

    protected override void ApplyModifiers()
    {
        if (modifiersApplied) return;

        var spellAttributes = GetBaseSpellAttributes();

        if (spellAttributes != null)
        {
            spellAttributes.sizeModifiers.Add(new ValueModifier(ModifierType.Multiplicative, sizeMultiplier));
            modifiersApplied = true;
        }
    }

    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);

        if (jObject["size_multiplier"] != null)
        {
            if (float.TryParse(jObject["size_multiplier"].ToString(), out float multiplier))
                sizeMultiplier = multiplier;
        }

        ApplyModifiers();
    }
}