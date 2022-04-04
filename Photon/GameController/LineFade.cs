using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFade : MonoBehaviour
{
    [SerializeField] Color color;
    [SerializeField] float speed = 15;
    [SerializeField] Vector3 shootPoint = Vector3.zero;
    [SerializeField] Vector3 hitPoint = Vector3.zero;
    private float t;
    LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        t = 0;
    }

    void Update()
    {
        //Move apha of color towards zero
        color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * speed);
        //Update color
        line.startColor = color;
        line.endColor = color;
        if(shootPoint != Vector3.zero && hitPoint != Vector3.zero)
        {
            line.SetPosition(0, shootPoint);
            line.SetPosition(1, new Vector3(Mathf.Lerp(shootPoint.x, hitPoint.x, t), Mathf.Lerp(shootPoint.y, hitPoint.y, t), Mathf.Lerp(shootPoint.z, hitPoint.z, t)));
            t += Time.deltaTime * 10;
        }
    }

    public void SetPoint(Vector3 shoot, Vector3 hit)
    {
        shootPoint = shoot;
        hitPoint = hit;
    }
}
