using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyControllerRanged : MonoBehaviour
{
    public float lookRadius = 40f;
    public float attackRadius = 15f;
    public float attackCD;

    public float health = 50;
    public bool Dying = false;

    public AudioSource EnemyRangedAudio;
    public AudioClip Shoot, NoticePlayer, Death,Damage;

    public Animator EnemyRangedAnim;

    bool seenenemy = false;

    Transform target;
    NavMeshAgent agent;

    CapsuleCollider CapCollider;

    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        CapCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        attackCD -= Time.deltaTime;
        EnemyRangedAnim.SetFloat("Attacking", attackCD);
        if (!Dying)
        {
            //Get distance to player
            float distance = Vector3.Distance(target.position, transform.position);
            //Is player in look radius
            if (distance <= lookRadius)
            {
                //Is player in line of sight
                RaycastHit PlayerRaycast;
                Debug.DrawRay(transform.position, target.position - transform.position);
                if (Physics.Raycast(transform.position, target.position - transform.position, out PlayerRaycast))
                {
                    PlayerMovement player = PlayerRaycast.transform.GetComponent<PlayerMovement>();
                    if (PlayerRaycast.transform == target)
                    {
                        //Attack player if in range
                        if (distance <= attackRadius)
                        {
                            //Play sound on wakeup
                            if (!seenenemy)
                            {
                                EnemyRangedAudio.PlayOneShot(NoticePlayer, 0.5f);
                                seenenemy = true;
                            }
                            //Attacking cooldown
                            if (attackCD <= 0)
                            {
                                agent.velocity = Vector3.zero;
                                player.TakePlayerDamage(10);
                                attackCD = 2f;
                                EnemyRangedAudio.PlayOneShot(Shoot, 0.5f);
                                EnemyRangedAnim.SetFloat("Attacking", attackCD);
                            }
                        } // if not in attack range, go to player
                        else
                        {
                            if (attackCD <= 1f)
                            {
                                agent.SetDestination(target.position);
                            }
                        }

                    }

                }
            }
        }
        else
        {
            agent.velocity = Vector3.zero;
            agent.speed = 0;
        }
    }

    public void TakeEnemyDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            EnemyRangedAudio.PlayOneShot(Death, 0.5f);
            Dying = true;
            agent.velocity = Vector3.zero;
            Invoke("Die", 1.75f);
            Destroy(CapCollider);
            EnemyRangedAnim.SetBool("IsDying", true);
        }
        else
        {
            EnemyRangedAudio.PlayOneShot(Damage, 0.5f);
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
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
