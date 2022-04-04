using UnityEngine;
using UnityEngine.UI;

public class NamePlate : MonoBehaviour
{
    [SerializeField] Text nicknameText;
    [SerializeField] GameObject player;

    void Update()
    {
        nicknameText.text = player.name;
        //Camera facing billboard
        Camera cam = Camera.main;
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
