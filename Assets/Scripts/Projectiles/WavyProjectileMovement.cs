using UnityEngine;

public class WavyProjectileMovement : ProjectileMovement
{
    // public float start;
    public WavyProjectileMovement(float speed) : base(speed)
    {
        // start = Time.time;
    }

    public override void Movement(Transform transform)
    {
        // transform.Translate(new Vector3(speed * Time.deltaTime, Mathf.Sin(Time.time * 15) * 1, 0), Space.Self);
        transform.Translate(new Vector3(speed * Time.deltaTime, Mathf.Sin(Time.time * 20) / 15, 0), Space.Self);
    }
}