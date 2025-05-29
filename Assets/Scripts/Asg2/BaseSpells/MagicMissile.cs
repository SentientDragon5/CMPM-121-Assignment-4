using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class MagicMissile : Spell
{
    public MagicMissile(SpellCaster owner) : base(owner)
    {
    }

    protected override void InitializeAttributes()
    {
        attributes = new SpellAttributes();
    }
    // No need to override the Cast method
    // The trajectory is set to "homing" in the JSON, which the ProjectileManager handles
}