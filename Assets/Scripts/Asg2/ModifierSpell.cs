
using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;


[System.Serializable]
public abstract class ModifierSpell : Spell
{
    protected Spell baseSpell;

    public ModifierSpell(Spell baseSpell, SpellCaster owner) : base(owner)
    {
        this.baseSpell = baseSpell;
        InitializeAttributes();
        ApplyModifiers();
    }

    protected override void InitializeAttributes()
    {
        attributes = new SpellAttributes();
    }

    protected SpellAttributes GetBaseSpellAttributes()
    {
        return baseSpell.GetType().GetField("attributes",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic)?.GetValue(baseSpell) as SpellAttributes;
    }


    // Apply modifiers to the base spell's attributes
    protected abstract void ApplyModifiers();

    public override string GetName() => attributes.name + " " + baseSpell.GetName();
    public override int GetIcon() => baseSpell.GetIcon();

    // handled by ValueModifier system
    public override int GetManaCost() => baseSpell.GetManaCost();
    public override int GetDamage() => baseSpell.GetDamage();
    public override float GetCooldown() => baseSpell.GetCooldown();
    public override float GetSpeed() => baseSpell.GetSpeed();
    public override string GetTrajectory() => baseSpell.GetTrajectory();
    public override float GetSize() => baseSpell.GetSize();

    public override bool IsReady() => baseSpell.IsReady();

    // Cast operation to the base spell for specific logic
    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        // Then let the base spell handle the actual casting
        this.team = team;
        last_cast = Time.time;
        yield return baseSpell.Cast(where, target, team);
    }

    // Override SetAttributesFromJson to set both our attributes and apply them to the base spell
    public override void SetAttributesFromJson(JObject jObject)
    {
        base.SetAttributesFromJson(jObject);
        ApplyModifiers();
    }

}