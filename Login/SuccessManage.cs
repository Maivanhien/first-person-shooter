using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccessManage : MonoBehaviour
{
    void Start()
    {
        Invoke("RemoveNotification", 3);
    }

    void RemoveNotification()
    {
        gameObject.SetActive(false);
    }
}
