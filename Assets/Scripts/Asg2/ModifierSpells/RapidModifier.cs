using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class RapidModifier : ModifierSpell
{
    private float damage_multiplier = 0.1f;
    private float cooldown_multiplier = 0.1f;
    private float size_multiplier = 0.5f;
    private float mana_multiplier = 0.5f;
    private bool modifiersApplied = false;
    
    public RapidModifier(Spell baseSpell, SpellCaster owner) : base(baseSpell, owner)
    {
    }
    
    protected override void InitializeAttributes()
    {
        base.InitializeAttributes();
        attributes.name = "Rapid";
        attributes.description = "Rapid Fire projectiles";
    }
    
    protected override void ApplyModifiers()
    {
        if (modifiersApplied) return;
        
        var spellAttributes = GetBaseSpellAttributes();
        
        if (spellAttributes != null)
        {
            spellAttributes.damageModifiers.Add(new ValueModifier(ModifierType.Multiplicative, damage_multiplier));
            spellAttributes.cooldownModifiers.Add(new ValueModifier(ModifierType.Multiplicative, cooldown_multiplier));
            spellAttributes.sizeModifiers.Add(new ValueModifier(ModifierType.Multiplicative, size_multiplier));
            spellAttributes.manaCostModifiers.Add(new ValueModifier(ModifierType.Multiplicative, mana_multiplier));
            modifiersApplied = true;
        }
    }
    
    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);
        
        if (jObject["damage_multiplier"] != null)
        {
            if (float.TryParse(jObject["damage_multiplier"].ToString(), out float multiplier))
                damage_multiplier = multiplier;
        }
        if (jObject["cooldown_multiplier"] != null)
        {
            if (float.TryParse(jObject["cooldown_multiplier"].ToString(), out float multiplier))
                cooldown_multiplier = multiplier;
        }
        if (jObject["size_multiplier"] != null)
        {
            if (float.TryParse(jObject["size_multiplier"].ToString(), out float multiplier))
                size_multiplier = multiplier;
        }
        if (jObject["mana_multiplier"] != null)
        {
            if (float.TryParse(jObject["mana_multiplier"].ToString(), out float multiplier))
                mana_multiplier = multiplier;
        }
        
        ApplyModifiers();
    }
}