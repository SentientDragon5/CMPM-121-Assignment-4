using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class DoublerModifier : ModifierSpell
{
    private float castDelay = 0.5f;
    private float manaCostMultiplier = 1.5f;
    private float cooldownMultiplier = 1.5f;
    private bool modifiersApplied = false;

    public DoublerModifier(Spell baseSpell, SpellCaster owner) : base(baseSpell, owner)
    {
    }

    protected override void InitializeAttributes()
    {
        base.InitializeAttributes();
        attributes.name = "Doubled";
        attributes.description = "Casts the spell twice with a short delay.";
    }

    protected override void ApplyModifiers()
    {
        if (modifiersApplied) return;

        var spellAttributes = GetBaseSpellAttributes();

        if (spellAttributes != null)
        {
            spellAttributes.manaCostModifiers.Add(new ValueModifier(ModifierType.Multiplicative, manaCostMultiplier));
            spellAttributes.cooldownModifiers.Add(new ValueModifier(ModifierType.Multiplicative, cooldownMultiplier));
        }
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        last_cast = Time.time;

        yield return baseSpell.Cast(where, target, team);

        yield return new WaitForSeconds(castDelay);

        yield return baseSpell.Cast(where, target, team);
    }

    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);

        if (jObject["delay"] != null)
        {
            if (float.TryParse(jObject["delay"].ToString(), out float delay))
                castDelay = delay;
        }

        if (jObject["mana_multiplier"] != null)
        {
            if (float.TryParse(jObject["mana_multiplier"].ToString(), out float multiplier))
                manaCostMultiplier = multiplier;
        }

        if (jObject["cooldown_multiplier"] != null)
        {
            if (float.TryParse(jObject["cooldown_multiplier"].ToString(), out float multiplier))
                cooldownMultiplier = multiplier;
        }

        ApplyModifiers();
    }
}