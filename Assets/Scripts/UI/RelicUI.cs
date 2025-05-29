using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicUI : MonoBehaviour
{
    public PlayerController player;
    public int index;

    public Image icon;
    public GameObject highlight;
    public TextMeshProUGUI label;
    public Relic relic;

    public void SetRelic(Relic relic)
    {
        this.relic = relic;

        GameManager.Instance.relicIconManager.PlaceSprite(relic.sprite, icon);
        this.highlight.gameObject.SetActive(false);
        this.label.gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
