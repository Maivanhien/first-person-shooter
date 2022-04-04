using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    public string name = "Glock";
    public int damage = 5;
    public float range = 100f;
    public float fireRate = 0f;
    public int maxBullets = 30;
    //public int maxReloadBullets = 30;
    [HideInInspector] public int bullets;
    public float reloadTime = 3f;
    public bool sniper = false;
    public GameObject graphics;

    public void SetBullets()
    {
        bullets = maxBullets;
    }
}
