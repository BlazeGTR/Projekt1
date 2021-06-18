using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rocket : MonoBehaviour
{
    public float BlastRadius = 5f;
    public float BlastForce = 200f;
    public float BlastDamage = 100f;

    float Distance;

    PlayerMovement PlayerM;
    GameObject Player;

    bool hasExploded = false;
    public GameObject explosionEffect;
    public GameObject trailEffect,puffEffect;
    public AudioClip RocketExplosion;

    private void Awake()
    {
        Instantiate(trailEffect, transform.position,transform.rotation,this.transform);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player")
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        if (!hasExploded)
        {
            GameObject Effect = Instantiate(explosionEffect, transform.position, transform.rotation);
            AudioSource audioSource = Effect.AddComponent<AudioSource>();
            audioSource.PlayOneShot(RocketExplosion, 0.5f);
            Collider[] colliders = Physics.OverlapSphere(transform.position, BlastRadius);

            foreach (Collider nearbyObject in colliders)
            {
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    
                    if (rb.tag != "Player")
                    {
                        rb.isKinematic = false;
                        if (rb.tag == "Enemy")
                        {
                            rb.AddExplosionForce(BlastForce*3, transform.position, BlastRadius);
                        }
                        else
                        {
                            rb.AddExplosionForce(BlastForce, transform.position, BlastRadius);
                        }
                    }
                }

                EnemyController EnemyC = nearbyObject.GetComponent<EnemyController>();
                if(EnemyC != null)
                {
                    if (rb.tag != "Player")
                    {
                        EnemyC.TurnKinematicOn();
                    }
                }

                Target target = nearbyObject.GetComponent<Target>();
                if (target != null)
                {
                    Debug.Log("BUM");
                    Distance = Vector3.Distance(transform.position, target.transform.position);

                    float effect = 1 - (Distance / BlastRadius);
                    target.TakeDamage(BlastDamage * effect);
                    Debug.Log(Distance);
                }

            }
            Destroy(Effect, 4f);
            Destroy(gameObject);
        }
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, BlastRadius);
    }
}
