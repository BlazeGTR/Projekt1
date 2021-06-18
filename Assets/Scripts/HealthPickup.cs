using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public GameObject SpotLightW;
    public int Heal = 0;
    public PlayerMovement PlayerS;

    private void OnTriggerEnter(Collider other)
    {
        PlayerS = other.GetComponent<PlayerMovement>();
        if (PlayerS != null)
        {
            PlayerS.TakePlayerDamage(-Heal);
            Destroy(gameObject);
            Destroy(SpotLightW);
        }
    }
}
