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

    Transform PlayerTransform;
    Vector3 PlayerPosition;
    Transform LastPos;
    NavMeshAgent agent;
    Rigidbody rb;
    PlayerMovement Playermovement;
    GameObject PlayerObj;

    public AudioSource EnemyAudio;
    public AudioClip Shoot, Death, NoticePlayer, Damage;

    public Animator EnemyAnimator;

    Vector3 MinSpeed = new Vector3(0.5f, 0.5f, 0.5f);

    enum State
    {
        Chasing,
        Attacking,
        Dying,
        Idle,
        Cooldown
    };

    State CurrentState = State.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PlayerTransform = Player.Instance.GetTransform();
        agent = GetComponent<NavMeshAgent>();
        CapCollider = GetComponent<CapsuleCollider>();
        agent.updateRotation = false;
        Playermovement = PlayerTransform.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        attackCD -= Time.deltaTime;
        EnemyAnimator.SetFloat("AttackCD", attackCD);
        float distance = Vector3.Distance(PlayerTransform.position, transform.position);

        switch (CurrentState)
        {
            case State.Chasing:
                {
                    RaycastHit PlayerRaycast;
                    Debug.DrawRay(transform.position, PlayerTransform.position - transform.position);
                    if (Physics.Raycast(transform.position, PlayerTransform.position - transform.position, out PlayerRaycast))
                    {
                        if (PlayerRaycast.transform == PlayerTransform)
                        {
                            LastPos = PlayerTransform;
                        }
                    }
                    agent.SetDestination(PlayerTransform.position);

                    if (distance <= attackRadius)
                    {
                        if (attackCD <= 0)
                        {
                            CurrentState = State.Attacking;
                        }
                    }
                    break;
                }

            case State.Attacking:
                {
                    agent.velocity = Vector3.zero;
                    RaycastHit AttackRay;
                    Physics.Raycast(transform.position, PlayerTransform.position - transform.position, out AttackRay);
                    attackCD = 1f;
                    switch (AttackRay.transform.tag)
                    {
                        case "Player":
                            Playermovement.TakePlayerDamage(5);
                            //EnemyAudio.PlayOneShot(Shoot, 0.5f);
                            CurrentState = State.Cooldown;
                            break;

                        case "Enemy":
                            Target t = AttackRay.transform.GetComponent<Target>();
                            t.TakeDamage(5);
                            CurrentState = State.Cooldown;
                            break;

                        default:
                            break;
                    }
                    break;
                }

            case State.Dying:
                {
                        agent.velocity = Vector3.zero;
                        agent.speed = 0;
                        Invoke("Die", 1.2f);
                        EnemyAnimator.SetBool("IsDying", true);
                    EnemyAudio.PlayOneShot(Death, 0.5f);
                    Destroy(CapCollider);
                    break;
                }

            case State.Idle:
                {
                    if (distance <= lookRadius)
                    {
                        RaycastHit PlayerRaycast;
                        Debug.DrawRay(transform.position, PlayerTransform.position - transform.position);
                        if (Physics.Raycast(transform.position, PlayerTransform.position - transform.position, out PlayerRaycast))
                        {;
                            if (PlayerRaycast.transform == PlayerTransform)
                            {
                                LastPos = PlayerTransform;
                                CurrentState = State.Chasing;

                                if (!seenenemy)
                                {
                                    EnemyAudio.PlayOneShot(NoticePlayer, 0.5f);
                                    seenenemy = true;
                                }
                            }
                        }
                    }

                        break;
                }

            case State.Cooldown:
                {
                    if (attackCD <= 0)
                    {
                        CurrentState = State.Chasing;
                    }
                    break;
                }

            default:
                break;
        }

        /*
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
        */
    }

    public void TakeEnemyDamage(float amount)
    {
        health -= amount;
        if(health <= 0)
        {
           // EnemyAudio.PlayOneShot(Death, 0.5f);
            CurrentState = State.Dying;
            //agent.velocity = Vector3.zero;
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
        transform.rotation = PlayerTransform.rotation;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
