using System.Collections.Generic;
using UnityEngine;

public enum ModifierType
{
    Additive,
    Multiplicative
}

public class ValueModifier
{
    public ModifierType type;
    public float value;

    public ValueModifier(ModifierType type, float value)
    {
        this.type = type;
        this.value = value;
    }

    public float Apply(float baseValue)
    {
        switch (type)
        {
            case ModifierType.Additive:
                return baseValue + value;
            case ModifierType.Multiplicative:
                return baseValue * value;
            default:
                return baseValue;
        }
    }

    public static float ApplyModifiers(float baseValue, List<ValueModifier> modifiers)
    {
        if (modifiers == null || modifiers.Count == 0)
        {
            return baseValue;
        }
        float result = baseValue;
        foreach (var modifier in modifiers)
        {
            result = modifier.Apply(result);
        }
        return result;
    }
    
    public static int ApplyModifiers(int baseValue, List<ValueModifier> modifiers)
    {
        float result = ApplyModifiers((float)baseValue, modifiers);
        return Mathf.RoundToInt(result);
    }
}