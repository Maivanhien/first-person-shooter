using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffectDestroy : MonoBehaviour
{
    private float delay = 4f;
    private float countDown;

    // Start is called before the first frame update
    void Start()
    {
        countDown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countDown -= Time.deltaTime;
        if (countDown <= 0f)
            Destroy(gameObject);
    }
}
