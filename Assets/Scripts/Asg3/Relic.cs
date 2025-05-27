using System.Data.SqlTypes;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Relic
{
    public string name;
    public int sprite;
    public RelicTrigger trigger;
    public RelicEffect effect;
}

[System.Serializable]
public class RelicTrigger
{
    public string description;
    public string type;
    public string amount;
}
[System.Serializable]
public class RelicEffect
{
    public string description;
    public string type;
    public string amount;
    public string until;
}
