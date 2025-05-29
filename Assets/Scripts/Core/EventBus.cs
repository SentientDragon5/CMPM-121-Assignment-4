using UnityEngine;
using UnityEngine.Events;
using System;

public class EventBus
{
    private static EventBus theInstance;
    public static EventBus Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new EventBus();
            return theInstance;
        }
    }

    public event Action<Vector3, Damage, Hittable> OnDamage;
    public event Action OnTakeDamage;
    public event Action<int> OnStandingStill;
    public event Action OnMove;
    public event Action OnKill;
    public event Action OnCastSpell;
    public void DoDamage(Vector3 where, Damage dmg, Hittable target)
    {
        OnDamage?.Invoke(where, dmg, target);
    }

    // new methods to handle relics
    public void TriggerTakeDamage()
    {
        OnTakeDamage?.Invoke();
    }

    public void TriggerStandingStill(int seconds)
    {
        OnStandingStill?.Invoke(seconds);
    }

    public void TriggerMove()
    {
        OnMove?.Invoke();
    }

    public void TriggerKill()
    {
        OnKill?.Invoke();
    }

    public void TriggerCastSpell()
    {
        OnCastSpell?.Invoke();
    }
}
