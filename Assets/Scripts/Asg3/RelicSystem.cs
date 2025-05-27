using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicSystem : MonoBehaviour
{
    private static RelicSystem instance;
    public static RelicSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("RelicSystem");
                instance = obj.AddComponent<RelicSystem>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
    
    private PlayerController player;
    private List<Relic> availableRelics = new List<Relic>();
    private List<Relic> activeRelics = new List<Relic>();
    
    // Dictionary of trigger functions
    private Dictionary<string, Action<RelicTrigger, RelicEffect, PlayerController>> triggerSetups = 
        new Dictionary<string, Action<RelicTrigger, RelicEffect, PlayerController>>();
    
    // Dictionary of effect functions
    private Dictionary<string, Action<RelicEffect, PlayerController>> effectApplications = 
        new Dictionary<string, Action<RelicEffect, PlayerController>>();
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        RegisterTriggerHandlers();
        RegisterEffectHandlers();
    }
    
    private void RegisterTriggerHandlers()
    {
        triggerSetups["take-damage"] = SetupTakeDamageTrigger;
        triggerSetups["stand-still"] = SetupStandStillTrigger;
        triggerSetups["on-kill"] = SetupOnKillTrigger;
        triggerSetups["drop-spell"] = SetupOnDropTrigger;
        triggerSetups["take-spell"] = SetupOnTakeTrigger;
    }
    
    private void RegisterEffectHandlers()
    {
        effectApplications["gain-mana"] = ApplyGainManaEffect;
        effectApplications["gain-spellpower"] = ApplyGainSpellPowerEffect;
        effectApplications["gain-speed"] = ApplyGainSpeedEffect;
        // custom relics effects go here
    }
    
    public void Initialize(PlayerController playerController)
    {
        this.player = playerController;
        LoadRelics();
    }
    
    private void LoadRelics()
    {
        availableRelics = new List<Relic>(DataLoader.Instance.relics);
        Debug.Log($"Loaded {availableRelics.Count} relics");
    }
    
    public List<Relic> GetRandomRelics(int count)
    {
        List<Relic> availableForSelection = new List<Relic>(availableRelics);
        
        // Remove relics that are already active
        foreach (var activeRelic in activeRelics)
        {
            availableForSelection.RemoveAll(r => r.name == activeRelic.name);
        }

        // If we don't have enough relics, return all available
        if (availableForSelection.Count <= count)
        {
            return new List<Relic>(availableForSelection);
        }
        // Select random relics
        List<Relic> selectedRelics = new List<Relic>();
        for (int i = 0; i < count; i++)
        {
            if (availableForSelection.Count == 0)
            {
                break;
            } 
            int randomIndex = UnityEngine.Random.Range(0, availableForSelection.Count);
            selectedRelics.Add(availableForSelection[randomIndex]);
            availableForSelection.RemoveAt(randomIndex);
        }
        
        return selectedRelics;
    }
    
    public void ActivateRelic(Relic relic)
    {
        activeRelics.Add(relic);
        
        // Setup the trigger
        if (triggerSetups.TryGetValue(relic.trigger.type, out var triggerSetup))
        {
            triggerSetup(relic.trigger, relic.effect, player);
            Debug.Log($"Activated relic: {relic.name}");
        }
        else
        {
            Debug.LogWarning($"No trigger setup found for type: {relic.trigger.type}");
        }
        
        // Remove from available relics
        availableRelics.RemoveAll(r => r.name == relic.name);
    }

    public int RelicCount
    {
        get
        {
            return activeRelics.Count;
        }
    }
    
    private void SetupTakeDamageTrigger(RelicTrigger trigger, RelicEffect effect, PlayerController player)
    {
        UnityEngine.Events.UnityAction takeDamageAction = () => {
            if (effectApplications.TryGetValue(effect.type, out var applyEffect))
            {
                applyEffect(effect, player);
                Debug.Log($"Triggered '{trigger.description}' effect: {effect.description}");
            }
        };
        
        player.hp.onTakeDamage.AddListener(takeDamageAction);
    }
    
    private void SetupStandStillTrigger(RelicTrigger trigger, RelicEffect effect, PlayerController player)
    {
        int requiredSeconds = 3; // default fallback 
        if (!string.IsNullOrEmpty(trigger.amount) && int.TryParse(trigger.amount, out int seconds))
        {
            requiredSeconds = seconds;
        }
        
        bool effectActive = false;
        
        UnityEngine.Events.UnityAction<int> standStillAction = (seconds) => {
            if (seconds >= requiredSeconds && !effectActive)
            {
                effectActive = true;
                if (effectApplications.TryGetValue(effect.type, out var applyEffect))
                {
                    applyEffect(effect, player);
                    Debug.Log($"Triggered '{trigger.description}' effect: {effect.description}");
                }
            }
        };
        
        player.onStandStill.AddListener(standStillAction);
        
        if (effect.until == "move")
        {
            UnityEngine.Events.UnityAction moveAction = () => {
                if (effectActive)
                {
                    effectActive = false;
                    if (effect.type == "gain-spellpower")
                    {
                        player.SetBonusSpellpower(0);
                        Debug.Log($"Ended '{effect.description}' effect due to movement");
                    }
                }
            };
            
            player.onMove.AddListener(moveAction);
        }
    }
    
    private void SetupOnTakeTrigger(RelicTrigger trigger, RelicEffect effect, PlayerController player)
    {
        UnityEngine.Events.UnityAction takeAction = () => {
            if (effectApplications.TryGetValue(effect.type, out var applyEffect))
            {
                applyEffect(effect, player);
                Debug.Log($"Triggered '{trigger.description}' effect: {effect.description}");
            }
        };
        player.onTakeSpell.AddListener(takeAction);

        if (effect.until == "drop-spell")
        {
            UnityEngine.Events.UnityAction dropAction = () => {
                if (effectApplications.TryGetValue(effect.type, out var applyEffect))
                {
                    player.SetBonusSpellpower(0);
                        Debug.Log($"Ended '{effect.description}' effect due to dropped spell");
                }
            };
            
            player.onDropSpell.AddListener(dropAction);
        }
    }

    private void SetupOnDropTrigger(RelicTrigger trigger, RelicEffect effect, PlayerController player)
    {
        UnityEngine.Events.UnityAction dropAction = () => {
            if (effectApplications.TryGetValue(effect.type, out var applyEffect))
            {
                applyEffect(effect, player);
                Debug.Log($"Triggered '{trigger.description}' effect: {effect.description}");
            }
        };
        
        player.onDropSpell.AddListener(dropAction);
    }
    
    private void SetupOnKillTrigger(RelicTrigger trigger, RelicEffect effect, PlayerController player)
    {
        UnityEngine.Events.UnityAction killAction = () => {
            if (effectApplications.TryGetValue(effect.type, out var applyEffect))
            {
                applyEffect(effect, player);
                Debug.Log($"Triggered '{trigger.description}' effect: {effect.description}");
            }
        };
        
        player.onKill.AddListener(killAction);

        if (effect.until == "take-damage")
        {
            UnityEngine.Events.UnityAction dropAction = () => {
                if (effectApplications.TryGetValue(effect.type, out var applyEffect))
                {
                    player.SetBonusSpellpower(0);
                        Debug.Log($"Ended '{effect.description}' effect due to damage recived");
                }
            };
            
            player.hp.onTakeDamage.AddListener(dropAction);
        }
    }
    
    private void ApplyGainManaEffect(RelicEffect effect, PlayerController player)
    {
        int amount = EvaluateAmount(effect.amount);
        player.GainMana(amount);
        Debug.Log($"Applied effect: Gained {amount} mana");
    }

    private void ApplyGainSpeedEffect(RelicEffect effect, PlayerController player)
    {
        int amount = EvaluateAmount(effect.amount);
        player.SpeedUp(amount);
        Debug.Log($"Applied effect: Gained {amount} speed");
    }
    
    private void ApplyGainSpellPowerEffect(RelicEffect effect, PlayerController player)
    {
        int amount = EvaluateAmount(effect.amount);
        
        if (effect.until == "cast-spell")
        {
            player.SetBonusSpellpower(amount);
            
            UnityEngine.Events.UnityAction resetSpellPower = null;
            resetSpellPower = () => {
                player.SetBonusSpellpower(0);
                player.spellcaster.onCastSpell.RemoveListener(resetSpellPower);
                Debug.Log("Spell cast: Removed bonus spell power");
            };
            
            player.spellcaster.onCastSpell.AddListener(resetSpellPower);
            Debug.Log($"Applied effect: Next spell gains {amount} spell power");
        }
        else
        {
            player.SetBonusSpellpower(amount);
            Debug.Log($"Applied effect: Gained {amount} spell power");
        }
    }
    
    private int EvaluateAmount(string amountStr)
    {
        if (string.IsNullOrEmpty(amountStr))
            return 0;
            
        if (int.TryParse(amountStr, out int result))
            return result;
            
        var variables = new Dictionary<string, int>
        {
            { "wave", GameManager.Instance.wave }
        };
        
        return RPNEvaluator.Evaluate(amountStr, variables);
    }
}