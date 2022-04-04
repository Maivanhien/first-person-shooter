using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeed : MonoBehaviour
{
    [SerializeField] GameObject killFeedItem;

    void Start()
    {
        GameManager.gameManager.onPlayerKilledCallback += OnKill;
    }

    public void OnKill(string player, string source)
    {
        GameObject itemGO = Instantiate(killFeedItem, this.transform);
        itemGO.transform.GetChild(0).GetComponent<Text>().text = "<b>" + source + "</b> killed <i>" + player + "</i>";
        Destroy(itemGO, 4f);
    }
}
