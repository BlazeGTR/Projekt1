using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    public float lookRadius = 20f;
    public float attackRadius = 3f;
    public float attackCD;

    bool Dying = false;
    bool seenenemy = false;

    public float health = 100;
    CapsuleCollider CapCollider;

    Transform target;
    Transform LastPos;
    NavMeshAgent agent;
    Rigidbody rb;

    public AudioSource EnemyAudio;
    public AudioClip Shoot, Death, NoticePlayer, Damage;

    public Animator EnemyAnimator;

    Vector3 MinSpeed = new Vector3(0.5f, 0.5f, 0.5f);


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        CapCollider = GetComponent<CapsuleCollider>();
        agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        attackCD -= Time.deltaTime;
        float distance = Vector3.Distance(target.position, transform.position);
            if (!Dying)
            {
                if (distance <= lookRadius)
                {
                    RaycastHit PlayerRaycast;
                    Debug.DrawRay(transform.position, target.position - transform.position);
                    if (Physics.Raycast(transform.position, target.position - transform.position, out PlayerRaycast))
                    {
                        PlayerMovement player = PlayerRaycast.transform.GetComponent<PlayerMovement>();
                        if (PlayerRaycast.transform == target)
                        {
                            LastPos = target;
                            agent.SetDestination(target.position);
                            if (!seenenemy)
                            {
                                EnemyAudio.PlayOneShot(NoticePlayer, 0.5f);
                                seenenemy = true;
                            }

                            if (distance <= attackRadius)
                            {
                                if (attackCD <= 0)
                                {
                                    agent.velocity = Vector3.zero;
                                    player.TakePlayerDamage(5);
                                    EnemyAudio.PlayOneShot(Shoot, 0.5f);
                                    attackCD = 1f;
                                }
                            }
                        }
                    }
                }
                else if (LastPos != null)
                {
                    agent.SetDestination(LastPos.position);
                }
            }
            else
            {
                agent.velocity = Vector3.zero;
                agent.speed = 0;
            } 
    }

    public void TurnKinematicOn()
    {
        Invoke("TurnKinematicOn2", 1f);   
    }

    void TurnKinematicOn2()
    {
        rb.isKinematic = true;
    }
    public void TakeEnemyDamage(float amount)
    {
        health -= amount;
        if(health <= 0)
        {
            EnemyAudio.PlayOneShot(Death, 0.5f);
            Dying = true;
            agent.velocity = Vector3.zero;
            Invoke("Die", 1.2f);
            EnemyAnimator.SetBool("IsDying", true);
            Destroy(CapCollider);
        }
        else
        {
            EnemyAudio.PlayOneShot(Damage, 0.5f);
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
    void LateUpdate()
    {
        if (agent.velocity.normalized != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }
        transform.rotation = target.rotation;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
