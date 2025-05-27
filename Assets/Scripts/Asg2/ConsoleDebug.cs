using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ConsoleDebug : MonoBehaviour
{
    [SerializeField] private GameObject inputFieldObject;
    private TMP_InputField inputField;

    void Start()
    {
        if (inputFieldObject != null)
        {
            inputField = inputFieldObject.GetComponent<TMP_InputField>();
            inputFieldObject.SetActive(false);
        }
        else
        {
            Debug.LogError("InputFieldObject is not assigned in the inspector.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            if (inputField != null)
            {
                inputFieldObject.SetActive(true);
                inputField.text = "/";
                inputField.ActivateInputField();
                inputField.caretPosition = inputField.text.Length;
            }
        }

        if (inputField != null && inputFieldObject.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            string command = inputField.text.TrimStart('/');
            if (!string.IsNullOrEmpty(command))
            {
                ExecuteCommand(command);
            }
            inputField.text = "";
            inputFieldObject.SetActive(false);
        }
    }

    private void ExecuteCommand(string command)
    {
        var method = GetType().GetMethod(command, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
        if (method != null)
        {
            method.Invoke(this, null); // Call the method with no parameters
        }
        else
        {
            Debug.LogWarning($"Command '{command}' not found.");
        }
    }

    private void Test()
    {
        Debug.Log("TestCommand executed!");
    }

    private void KillAll()
    {
        EnemyController[] enemies = Object.FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (EnemyController enemy in enemies)
        {
            enemy.Die();
        }
        Debug.Log("All enemies have been killed.");
    }
    private void FullHeal(){
        var hittable = GameManager.Instance.player.GetComponent<PlayerController>().hp;
        hittable.hp = hittable.max_hp;
    }
    public void Invincible(){
        var hittable = GameManager.Instance.player.GetComponent<PlayerController>().hp;
        hittable.SetMaxHP(100000000);
        hittable.hp = hittable.max_hp;
    }
    private void ka() => KillAll();
    private void fh() => FullHeal();
    private void i() => Invincible();
}
