using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

public class SpellBuilder
{
    private Dictionary<string, JObject> spellDefinitions;
    private List<string> baseSpellKeys;
    private List<string> modifierSpellKeys;
    private System.Random random;

    public SpellBuilder()
    {
        random = new System.Random();
        LoadSpellDefinitions();
    }

    private void LoadSpellDefinitions()
    {
        spellDefinitions = new Dictionary<string, JObject>();
        baseSpellKeys = new List<string>();
        modifierSpellKeys = new List<string>();

        JObject spellsJson = DataLoader.Instance.spells;
        if (spellsJson == null)
        {
            Debug.LogError("Failed to load spells.json");
            return;
        }

        foreach (var property in spellsJson.Properties())
        {
            string key = property.Name;
            JObject spellObj = property.Value as JObject;

            if (spellObj != null)
            {
                spellDefinitions[key] = spellObj;

                // Determine if it's a base spell or a modifier based on name pattern
                if (key == "damage_amp" || key == "speed_amp" || key == "doubler" ||
                    key == "splitter" || key == "chaos" || key == "homing" || key == "rapid" || key == "huge" || key == "slug" || key == "wavy" || 
                    key == "ricochet" || key == "freeze")
                {
                    modifierSpellKeys.Add(key);
                }
                else
                {
                    baseSpellKeys.Add(key);
                }
            }
        }

        Debug.Log($"Loaded {baseSpellKeys.Count} base spells and {modifierSpellKeys.Count} modifier spells");
    }

    // public Spell Build(SpellCaster owner)
    // {
    //     return BuildSpell("arcane_bolt", owner);
    // }

    public Spell BuildSpell(string key, SpellCaster owner)
    {
        if (!spellDefinitions.ContainsKey(key))
        {
            Debug.LogError($"Spell definition not found for key: {key}");
            return new ArcaneBolt(owner); // Fallback
        }

        JObject spellObj = spellDefinitions[key];

        if (modifierSpellKeys.Contains(key))
        {
            Debug.LogError($"Cannot build a modifier spell directly: {key}");
            return new ArcaneBolt(owner);
        }

        Spell spell = CreateSpellInstance(key, owner);

        if (spell != null)
        {
            spell.SetAttributesFromJson(spellObj);
            Debug.Log($"Created spell: {spell.GetName()} with damage: {spell.GetDamage()}");
        }

        return spell ?? new ArcaneBolt(owner);
    }

    //  random spell (possibly with modifiers)
    public Spell BuildRandomSpell(SpellCaster owner)
    {
        // Choose a random base spell
        string baseSpellKey = baseSpellKeys[random.Next(baseSpellKeys.Count)];
        Spell baseSpell = BuildSpell(baseSpellKey, owner);

        // Decide how many modifiers to apply 
        int maxModifiers = GameManager.Instance.wave;
        int numModifiers = random.Next(maxModifiers);

        // Apply modifiers
        Spell result = baseSpell;
        for (int i = 0; i < numModifiers; i++)
        {
            if (modifierSpellKeys.Count > 0)
            {
                string modifierKey = modifierSpellKeys[random.Next(modifierSpellKeys.Count)];
                result = ApplyModifier(modifierKey, result, owner);
            }
        }

        return result;
    }

    private Spell ApplyModifier(string modifierKey, Spell baseSpell, SpellCaster owner)
    {
        if (!spellDefinitions.ContainsKey(modifierKey))
        {
            Debug.LogError($"Modifier definition not found for key: {modifierKey}");
            return baseSpell;
        }

        JObject modifierObj = spellDefinitions[modifierKey];

        Spell modifierSpell = CreateModifierInstance(modifierKey, baseSpell, owner);

        if (modifierSpell != null)
        {
            modifierSpell.SetAttributesFromJson(modifierObj);
        }

        return modifierSpell ?? baseSpell;
    }

    private Spell CreateSpellInstance(string key, SpellCaster owner)
    {
        // add new spells here
        switch (key.ToLower())
        {
            case "arcane_bolt":
                return new ArcaneBolt(owner);
            case "arcane_spray":
                return new ArcaneSpray(owner);
            case "magic_missile":
                return new MagicMissile(owner);
            case "arcane_blast":
                return new ArcaneBlast(owner);
            case "chain_lightning":
                return new ChainLightning(owner);
            case "frost_shard":
                return new FrostShard(owner);
            default:
                Debug.LogError($"Unknown base spell key: {key}");
                return new ArcaneBolt(owner); // currently working spell
        }
    }

    // Create a modifier instance based on the key
    private Spell CreateModifierInstance(string key, Spell baseSpell, SpellCaster owner)
    {
        switch (key.ToLower())
        {
            case "damage_amp":
                return new DamageMagnifier(baseSpell, owner);
            case "doubler":
                return new DoublerModifier(baseSpell, owner);
            case "splitter":
                return new SplitterModifier(baseSpell, owner);
            case "chaos":
                return new ChaosModifier(baseSpell, owner);
            case "speed_amp":
                return new SpeedModifier(baseSpell, owner);
            case "homing":
                return new HomingModifier(baseSpell, owner);
            case "rapid":
                return new RapidModifier(baseSpell, owner);
            case "huge":
                return new HugeModifier(baseSpell, owner);
            case "ricochet":
                return new RicochetModifier(baseSpell, owner);
            case "slug":
                return new SlugModifier(baseSpell, owner);
            case "freeze":
                return new FreezeModifier(baseSpell, owner);
            case "wavy":
                return new WavyModifier(baseSpell, owner);
            default:
                Debug.LogError($"Unknown modifier key: {key}");
                return baseSpell;
        }
    }
}