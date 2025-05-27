using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public abstract class Spell 
{
    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;
    protected SpellAttributes attributes;

    // default lifetime. overriden in other spells.
    public float lifetime = 20f;

    public Spell(SpellCaster owner)
    {
        this.owner = owner;
        attributes = new SpellAttributes();
        InitializeAttributes();
    }

    protected abstract void InitializeAttributes();

    public virtual string GetName()
    {
        return attributes.name;
    }
    public virtual string GetDescription()
    {
        return attributes.description;
    }

    public virtual int GetManaCost()
    {
        return attributes.GetFinalManaCost();
    }

    public virtual int GetDamage()
    {
        return attributes.GetFinalDamage(owner.spellPower);
    }

    public virtual float GetCooldown()
    {
        return attributes.GetFinalCooldown();
    }

    public virtual int GetIcon()
    {
        return attributes.icon;
    }

    public virtual float GetSpeed() 
    {
        return attributes.GetFinalSpeed();
    }
    
    public virtual string GetTrajectory() 
    {
        return attributes.trajectory;
    }
    public virtual float GetSize()
    {
        return attributes.GetFinalSize();
    }

    public virtual bool IsReady()
    {
        return (last_cast + GetCooldown() < Time.time);
    }
    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        last_cast = Time.time;
        GameManager.Instance.projectileManager.CreateProjectile(
            attributes.projectileSprite, 
            GetTrajectory(), 
            where, 
            target - where, 
            GetSpeed(), 
            OnHit,
            lifetime,
            GetSize());
        yield return new WaitForEndOfFrame();
    }

    protected virtual void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            int damage = GetDamage();
            Damage.Type damageType = Damage.TypeFromString(attributes.damageType);
            other.Damage(new Damage(damage, damageType));

            GameManager.Instance.totalDamageDealt += damage;
            
            GameManager.Instance.player.GetComponent<PlayerController>().onKill.Invoke();
        }
    }

    // Set attributes from JSON
    public virtual void SetAttributesFromJson(JObject jObject)
    {
        if (jObject == null) return;
        
        attributes.name = jObject["name"]?.ToString();
        attributes.description = jObject["description"]?.ToString();
        
        if (jObject["icon"] != null)
            attributes.icon = jObject["icon"].Value<int>();
        
        // Parse damage
        if (jObject["damage"] != null)
        {
            var damageObj = jObject["damage"] as JObject;
            if (damageObj != null)
            {
                string damageExpr = damageObj["amount"]?.ToString();
                attributes.damageType = damageObj["type"]?.ToString() ?? "arcane";
                
                if (!string.IsNullOrEmpty(damageExpr))
                {
                    var vars = new Dictionary<string, int> { { "power", owner.spellPower }, { "wave", 1 } };
                    attributes.damage = RPNEvaluator.Evaluate(damageExpr, vars);
                }
            }
            else
            {
                string damageExpr = jObject["damage"].ToString();
                
                if (!string.IsNullOrEmpty(damageExpr))
                {
                    var vars = new Dictionary<string, int> { { "power", owner.spellPower }, { "wave", 1 } };
                    attributes.damage = RPNEvaluator.Evaluate(damageExpr, vars);
                }
            }
        }
        
        // Parse mana cost
        if (jObject["mana_cost"] != null)
        {
            string manaCostExpr = jObject["mana_cost"].ToString();
            if (!string.IsNullOrEmpty(manaCostExpr))
            {
                var vars = new Dictionary<string, int> { { "power", owner.spellPower }, { "wave", 1 } };
                attributes.manaCost = RPNEvaluator.Evaluate(manaCostExpr, vars);
            }
        }
        
        // Parse cooldown
        if (jObject["cooldown"] != null)
        {
            string cooldownStr = jObject["cooldown"].ToString();
            if (!string.IsNullOrEmpty(cooldownStr))
            {
                if (float.TryParse(cooldownStr, out float cooldownValue))
                    attributes.cooldown = cooldownValue;
            }
        }
        
        // Parse projectile
        if (jObject["projectile"] != null)
        {
            var projectileObj = jObject["projectile"] as JObject;
            if (projectileObj != null)
            {
                attributes.trajectory = projectileObj["trajectory"]?.ToString() ?? "straight";
                
                if (projectileObj["speed"] != null)
                {
                    string speedStr = projectileObj["speed"].ToString();
                    var vars = new Dictionary<string, float> { { "power", owner.spellPower }, { "wave", 1 } };
                    attributes.speed = FloatRPNEvaluator.Evaluate(speedStr, vars);
                }
                
                if (projectileObj["sprite"] != null)
                    attributes.projectileSprite = projectileObj["sprite"].Value<int>();
                
                if (projectileObj["lifetime"] != null)
                {
                    string lifetimeStr = projectileObj["lifetime"].ToString();
                    var vars = new Dictionary<string, float> { { "power", owner.spellPower }, { "wave", 1 } };
                    attributes.lifetime = FloatRPNEvaluator.Evaluate(lifetimeStr, vars);
                }
            }
        }
        
        // Secondary damage (used by some spells)
        if (jObject["secondary_damage"] != null)
        {
            string secondaryDamageExpr = jObject["secondary_damage"].ToString();
            if (!string.IsNullOrEmpty(secondaryDamageExpr))
            {
                var vars = new Dictionary<string, int> { { "power", owner.spellPower }, { "wave", 1 } };
                attributes.secondaryDamage = RPNEvaluator.Evaluate(secondaryDamageExpr, vars);
            }
        }
        
        // Number of projectiles (used by some spells)
        if (jObject["N"] != null)
        {
            string numProjectilesExpr = jObject["N"].ToString();
            if (!string.IsNullOrEmpty(numProjectilesExpr))
            {
                var vars = new Dictionary<string, int> { { "power", owner.spellPower }, { "wave", 1 } };
                attributes.numProjectiles = RPNEvaluator.Evaluate(numProjectilesExpr, vars);
            }
        }
        
        // Spray angle (used by arcane spray)
        if (jObject["spray"] != null)
        {
            if (float.TryParse(jObject["spray"].ToString(), out float spray))
                attributes.spray = spray;
        }
    }

}
