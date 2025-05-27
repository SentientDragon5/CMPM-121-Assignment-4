using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class ArcaneBolt : Spell
{
    public ArcaneBolt(SpellCaster owner) : base(owner)
    {
    }
    
    protected override void InitializeAttributes()
    {
        attributes = new SpellAttributes();
    }
}