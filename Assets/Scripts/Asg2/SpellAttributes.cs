using System.Collections.Generic;
using UnityEngine;

public class SpellAttributes
{
    // Basic properties
    public string name;
    public string description;
    public int icon;
    
    // Modifiable properties
    public int damage;
    public string damageType = "arcane";
    public int manaCost;
    public float cooldown;
    public float speed;
    public string trajectory = "straight";
    public float size = 0.7f;
    public int projectileSprite = 0;
    
    // Optional properties
    public int? secondaryDamage; 
    public int? numProjectiles;
    public float? spray;  // For ArcaneSpray
    public float? lifetime; // For projectile lifetime
    public int? chainCount;  // Number of chain jumps
    public float? chainDamageDecay; // Damage reduction per chain
    public float? chainRange; // Maximum range for chaining
    
    // Modifiers for various properties
    public List<ValueModifier> damageModifiers = new List<ValueModifier>();
    public List<ValueModifier> manaCostModifiers = new List<ValueModifier>();
    public List<ValueModifier> cooldownModifiers = new List<ValueModifier>();
    public List<ValueModifier> speedModifiers = new List<ValueModifier>();
    public List<ValueModifier> secondaryDamageModifiers = new List<ValueModifier>();
    public List<ValueModifier> numProjectilesModifiers = new List<ValueModifier>();
    public List<ValueModifier> sizeModifiers = new List<ValueModifier>();

    public int GetFinalDamage(int spellPower)
    {
        int baseDamage = damage * spellPower / 10;
        return ValueModifier.ApplyModifiers(baseDamage, damageModifiers);
    }
    
    public int GetFinalManaCost()
    {
        return ValueModifier.ApplyModifiers(manaCost, manaCostModifiers);
    }
    
    public float GetFinalCooldown()
    {
        return ValueModifier.ApplyModifiers(cooldown, cooldownModifiers);
    }
    
    public float GetFinalSpeed()
    {
        return ValueModifier.ApplyModifiers(speed, speedModifiers);
    }
    public float GetFinalSize()
    {
        return ValueModifier.ApplyModifiers(size, sizeModifiers);
    }
    
    public int GetFinalSecondaryDamage(int spellPower)
    {
        if (!secondaryDamage.HasValue) return 0;
        
        int baseSecondaryDamage = secondaryDamage.Value * spellPower / 10;
        return ValueModifier.ApplyModifiers(baseSecondaryDamage, secondaryDamageModifiers);
    }
    
    public int GetFinalNumProjectiles()
    {
        if (!numProjectiles.HasValue) return 1;
        
        return ValueModifier.ApplyModifiers(numProjectiles.Value, numProjectilesModifiers);
    }
}