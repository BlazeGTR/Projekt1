using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : MonoBehaviour
{
    public GameObject SpotLightW;
    public int Weapon = 0;
    public PlayerMovement PlayerS;

    private void OnTriggerEnter(Collider other)
    {
        PlayerS = other.GetComponent<PlayerMovement>();
        PlayerS.GettingWeapons(Weapon);
        Destroy(gameObject);
        Destroy(SpotLightW);
    }
}
