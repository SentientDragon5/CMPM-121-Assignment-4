using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class DamageMagnifier : ModifierSpell
{
    private float damageMultiplier = 1.5f;
    private float manaCostMultiplier = 1.5f;
    private bool modifiersApplied = false;


    public DamageMagnifier(Spell baseSpell, SpellCaster owner) : base(baseSpell, owner)
    {
    }

    protected override void InitializeAttributes()
    {
        base.InitializeAttributes();
        attributes.name = "Amplified";
        attributes.description = "Increases damage but costs more mana.";
    }

    protected override void ApplyModifiers()
    {
        if (modifiersApplied) return;

        var spellAttributes = GetBaseSpellAttributes();

        if (spellAttributes != null)
        {
            spellAttributes.damageModifiers.Add(new ValueModifier(ModifierType.Multiplicative, damageMultiplier));
            spellAttributes.manaCostModifiers.Add(new ValueModifier(ModifierType.Multiplicative, manaCostMultiplier));

            modifiersApplied = true;
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
                manaCostMultiplier = multiplier;
        }

        ApplyModifiers();
    }
}