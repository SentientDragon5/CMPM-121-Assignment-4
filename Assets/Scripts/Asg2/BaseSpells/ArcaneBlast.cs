// Create a new file called ArcaneBlast.cs
using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class ArcaneBlast : Spell
{
    public ArcaneBlast(SpellCaster owner) : base(owner)
    {
    }
    
    protected override void InitializeAttributes()
    {
        attributes = new SpellAttributes();
    }
    
    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        last_cast = Time.time;
        
        Vector3 direction = target - where;
        
        GameManager.Instance.projectileManager.CreateProjectile(
            attributes.projectileSprite,
            GetTrajectory(),
            where,
            direction,
            GetSpeed(),
            OnBlastHit,
            lifetime,
            GetSize()
        );
        
        yield return new WaitForEndOfFrame();
    }
    
    private void OnBlastHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            // Deal damage with the main projectile
            int damage = GetDamage();
            Damage.Type damageType = Damage.TypeFromString(attributes.damageType);
            other.Damage(new Damage(damage, damageType));
            
            GameManager.Instance.totalDamageDealt += damage;
        }
        
        // Spawn secondary projectiles in all directions
        SpawnSecondaryProjectiles(impact);
    }
    
    private void SpawnSecondaryProjectiles(Vector3 position)
    {
        int numProjectiles = attributes.GetFinalNumProjectiles();
        float angleStep = 360f / numProjectiles;
        
        // Get the secondary projectile attributes
        JObject projectileObj = null;
        if (attributes.GetType().GetField("jObject", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(this) is JObject jObj)
        {
            if (jObj["secondary_projectile"] != null)
                projectileObj = jObj["secondary_projectile"] as JObject;
        }
        
        // Default secondary projectile values
        float speed = 20f;
        float lifetime = 0.1f;
        int sprite = attributes.projectileSprite;
        
        if (projectileObj != null)
        {
            if (projectileObj["speed"] != null && float.TryParse(projectileObj["speed"].ToString(), out float s))
                speed = s;
            
            if (projectileObj["lifetime"] != null && float.TryParse(projectileObj["lifetime"].ToString(), out float lt))
                lifetime = lt;
            
            if (projectileObj["sprite"] != null)
                sprite = projectileObj["sprite"].Value<int>();
        }
        
        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = i * angleStep;
            Vector3 direction = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0f
            );
            
            GameManager.Instance.projectileManager.CreateProjectile(
                sprite,
                "straight",
                position,
                direction,
                speed,
                OnSecondaryHit,
                lifetime,
                GetSize()
            );
        }
    }
    private void OnSecondaryHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            int damage = attributes.GetFinalSecondaryDamage(owner.spellPower);
            Damage.Type damageType = Damage.TypeFromString(attributes.damageType);
            other.Damage(new Damage(damage, damageType));
            
            GameManager.Instance.totalDamageDealt += damage;
        }
    }
}