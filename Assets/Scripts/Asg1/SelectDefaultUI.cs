using UnityEngine;
using UnityEngine.EventSystems;

public class SelectDefaultUI : MonoBehaviour{
    public EventSystem eventSystem;
    void Start()
    {
        eventSystem = GameObject.FindAnyObjectByType<EventSystem>();
        eventSystem.SetSelectedGameObject(gameObject);
    }
}