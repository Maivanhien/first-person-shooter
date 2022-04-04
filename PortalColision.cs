using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalColision : MonoBehaviour
{
    [SerializeField] int mapNumber = 1;
    static bool collidered = false;

    void OnTriggerExit(Collider other)
    {
        if (collidered == true)
            return;
        if(other.gameObject.tag == "Player")
        {
            if(mapNumber == 1)
                other.transform.position = other.transform.position + new Vector3(-196.808f, -2.1487f, -0.2204653f);
            if (mapNumber == 2)
                other.transform.position = other.transform.position + new Vector3(193.5f, -2.1587f, 4f);
        }
        collidered = true;
        StartCoroutine(CoroutineCollidered());
    }

    IEnumerator CoroutineCollidered()
    {
        yield return new WaitForSeconds(1f);
        collidered = false;
    }
}
