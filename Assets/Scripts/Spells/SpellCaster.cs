using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class SpellCaster 
{
    public int mana;
    public int max_mana;
    public int mana_reg;
    int sp;
    public int spellPower { get => sp + bonusSpellPower; set => sp = value; }
    public int bonusSpellPower = 0;
    public Hittable.Team team;
    public List<Spell> equippedSpells;
    public int activeSpellIndex;
    public Spell activeSpell => equippedSpells.Count > activeSpellIndex ? equippedSpells[activeSpellIndex] : null; // returns the current active spell

    public const int MAX_EQUIPPED_SPELLS = 5;


    public IEnumerator ManaRegeneration()
    {
        while (true)
        {
            GainMana(mana_reg);
            yield return new WaitForSeconds(1);
        }
    }

    public void GainMana(int amount)
    {
        mana += amount;
        mana = Mathf.Min(mana, max_mana);
    }

    public SpellCaster(int mana, int mana_reg, Hittable.Team team)
    {
        this.mana = mana;
        this.max_mana = mana;
        this.mana_reg = mana_reg;
        this.team = team;
        this.spellPower = 10;
        equippedSpells = new List<Spell>();
        activeSpellIndex = 0;
    }

    public void SetSpellPower(int power)
    {
        this.spellPower = power;
    }
    public void SetBonusSpellPower(int power)
    {
        this.bonusSpellPower = power;
    }
    
    // same as set max hp from hittable
    public void SetMaxMana(int max_mana)
    {
        float perc = this.mana * 1.0f / this.max_mana;
        this.max_mana = max_mana;
        this.mana = Mathf.RoundToInt(perc * max_mana);
    }
    
    public bool AddSpell(Spell spell)
    {
        if (equippedSpells.Count < MAX_EQUIPPED_SPELLS)
        {
            equippedSpells.Add(spell);
            return true;
        }
        return false;
    }
    
    public void RemoveSpell(int index)
    {
        if (index >= 0 && index < equippedSpells.Count)
        {
            equippedSpells.RemoveAt(index);
            
            if (activeSpellIndex >= equippedSpells.Count)
            {
                activeSpellIndex = Mathf.Max(0, equippedSpells.Count - 1);
            }
        }
    }
    
    public void SelectSpell(int index)
    {
        if (index >= 0 && index < equippedSpells.Count)
        {
            activeSpellIndex = index;
        }
    }

    public IEnumerator Cast(Vector3 where, Vector3 target)
    {
        if (activeSpell != null && mana >= activeSpell.GetManaCost() && activeSpell.IsReady())
        {
            mana -= activeSpell.GetManaCost();
            yield return activeSpell.Cast(where, target, team);
        }
        onCastSpell.Invoke();
        yield break;
    }
    
    public UnityEvent onCastSpell = new();
}
