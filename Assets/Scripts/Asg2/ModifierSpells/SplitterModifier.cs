using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class SplitterModifier : ModifierSpell
{
    private float angle = 10f;
    private float manaCostMultiplier = 1.5f;
    private bool modifiersApplied = false;

    public SplitterModifier(Spell baseSpell, SpellCaster owner) : base(baseSpell, owner)
    {
    }

    protected override void InitializeAttributes()
    {
        base.InitializeAttributes();
        attributes.name = "Split";
        attributes.description = "Spell is cast twice in slightly different directions; increased mana cost.";
    }

    protected override void ApplyModifiers()
    {
        if (modifiersApplied) return;

        var spellAttributes = GetBaseSpellAttributes();

        if (spellAttributes != null)
        {
            spellAttributes.manaCostModifiers.Add(new ValueModifier(ModifierType.Multiplicative, manaCostMultiplier));
            modifiersApplied = true;
        }
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        last_cast = Time.time;

        Vector3 direction = (target - where).normalized;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float leftAngle = baseAngle - angle / 2;
        float rightAngle = baseAngle + angle / 2;

        Vector3 leftDirection = new Vector3(
            Mathf.Cos(leftAngle * Mathf.Deg2Rad),
            Mathf.Sin(leftAngle * Mathf.Deg2Rad),
            0f
        );

        Vector3 rightDirection = new Vector3(
            Mathf.Cos(rightAngle * Mathf.Deg2Rad),
            Mathf.Sin(rightAngle * Mathf.Deg2Rad),
            0f
        );

        Vector3 leftTarget = where + leftDirection * 10f;
        Vector3 rightTarget = where + rightDirection * 10f;

        yield return baseSpell.Cast(where, leftTarget, team);
        yield return baseSpell.Cast(where, rightTarget, team);
    }

    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);

        if (jObject["angle"] != null)
        {
            if (float.TryParse(jObject["angle"].ToString(), out float angleValue))
                angle = angleValue;
        }

        if (jObject["mana_multiplier"] != null)
        {
            if (float.TryParse(jObject["mana_multiplier"].ToString(), out float multiplier))
                manaCostMultiplier = multiplier;
        }

        ApplyModifiers();
    }
}