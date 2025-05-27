using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class ArcaneSpray : Spell
{
    public ArcaneSpray(SpellCaster owner) : base(owner)
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
        
        Vector3 direction = (target - where).normalized;
        int numProjectiles = attributes.GetFinalNumProjectiles();
        
        // spray angle (default to 30 degrees)
        float sprayAngle = attributes.spray.HasValue ? attributes.spray.Value * 360f : 30f;
        
        // start angle
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - (sprayAngle / 2);
        float angleIncrement = sprayAngle / (numProjectiles - 1);
        
        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = startAngle + (angleIncrement * i);
            Vector3 projectileDirection = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0f
            );
            
            float lifetime = attributes.lifetime.HasValue ? attributes.lifetime.Value : 0.5f;
            
            GameManager.Instance.projectileManager.CreateProjectile(
                attributes.projectileSprite,
                GetTrajectory(),
                where,
                projectileDirection,
                GetSpeed(),
                OnHit,
                lifetime,//local var
                GetSize()
            );
            
            // Small delay between projectiles for a more natural spray effect
            yield return new WaitForSeconds(0.02f);
        }
    }
}