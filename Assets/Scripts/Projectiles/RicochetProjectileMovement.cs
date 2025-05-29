using UnityEngine;

public class RicochetProjectileMovement : ProjectileMovement
{
    private int bouncesRemaining = 1;

    public RicochetProjectileMovement(float speed) : base(speed) { }

    public override void Movement(Transform transform)
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    public bool TryBounce(Collision2D collision, Transform transform)
    {
        if (bouncesRemaining <= 0) return false;

        ContactPoint2D contact = collision.contacts[0];
        Vector3 newDirection = Vector3.Reflect(transform.right, contact.normal);
        float angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        bouncesRemaining--;
        return true;
    }

}