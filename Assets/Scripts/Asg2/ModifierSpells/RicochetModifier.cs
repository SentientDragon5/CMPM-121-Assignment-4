using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class RicochetModifier : ModifierSpell
{
    private bool modifiersApplied = false;

    public RicochetModifier(Spell baseSpell, SpellCaster owner) : base(baseSpell, owner)
    {
    }

    protected override void InitializeAttributes()
    {
        base.InitializeAttributes();
        attributes.name = "Ricochet";
        attributes.description = "The projectile bounces off one wall before disappearing.";
    }

    protected override void ApplyModifiers()
    {
        if (modifiersApplied) return;

        var spellAttributes = GetBaseSpellAttributes();

        if (spellAttributes != null)
        {
            spellAttributes.trajectory = "ricochet";
            modifiersApplied = true;
        }
    }

    public override string GetTrajectory()
    {
        return "ricochet";
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

        ApplyModifiers();
    }
}