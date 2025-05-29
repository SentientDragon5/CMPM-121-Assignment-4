using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class WavyModifier : ModifierSpell
{
    private bool modifiersApplied = false;

    public WavyModifier(Spell baseSpell, SpellCaster owner) : base(baseSpell, owner)
    {
    }

    protected override void InitializeAttributes()
    {
        base.InitializeAttributes();
        attributes.name = "Wavy";
        attributes.description = "Spell is cast in a wavy line, and the projectile occasionally does significantly more damage.";
    }

    protected override void ApplyModifiers()
    {
        if (modifiersApplied) return;

        var spellAttributes = GetBaseSpellAttributes();

        if (spellAttributes != null)
        {
            spellAttributes.trajectory = "wavy";
            modifiersApplied = true;
        }
    }

    public override string GetTrajectory()
    {
        return "wavy";
    }

    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);

        if (jObject["projectile_trajectory"] != null)
        {
            string trajectory = jObject["projectile_trajectory"].ToString();
            var spellAttributes = GetBaseSpellAttributes();
            if (spellAttributes != null)
            {
                spellAttributes.trajectory = trajectory;
            }
        }

        // Handle crit properties for wavy modifier
        if (jObject["crit"] != null)
        {
            JObject critObj = jObject["crit"] as JObject;
            var spellAttributes = GetBaseSpellAttributes();
            
            if (spellAttributes != null && critObj != null)
            {
                if (critObj["chance"] != null)
                {
                    if (float.TryParse(critObj["chance"].ToString(), out float chance))
                        spellAttributes.critChance = chance;
                }

                if (critObj["damage_multiplier"] != null)
                {
                    if (float.TryParse(critObj["damage_multiplier"].ToString(), out float multiplier))
                        spellAttributes.critMultiplier = multiplier;
                }
            }
        }

        ApplyModifiers();
    }
}