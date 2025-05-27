using UnityEngine;

public class RelicUIManager : MonoBehaviour
{
    public GameObject relicUIPrefab;
    public PlayerController player;
    //private static RelicUIManager theInstance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.relicsUI = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PutsRelic(Relic r)
    {
        // make a new Relic UI representation
        GameObject rui = Instantiate(relicUIPrefab, transform);
        int temp = RelicSystem.Instance.RelicCount - 1;
        rui.transform.localPosition = new Vector3(-450 + 40 * temp, 0, 0);
        RelicUI ruic = rui.GetComponent<RelicUI>();
        ruic.SetRelic(r);
        //ruic.player = player;
        
    }
}
