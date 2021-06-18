using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float PlayerHealth = 100;

    public Font ActiveFont;

    //speeds
    public float speed = 12f;
    public float gravity = -25f;
    public float jumpH = 3f;

    //Groundcheck
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask EnemyMask;

    //Equipment
    public bool ShotgunOwned = false;
    public bool SMGOwned = false;
    public bool SniperOwned = false;
    public bool RLauncherOwned = false;

    //Movement
    public Vector3 velocity;
    bool isGrounded;
    bool OnEmeny;
    public Vector3 move;

    //Other scripts
    public FireScript FireS;
    public int CurrentWeapon = 1;

    //Sounds
    public AudioSource PlayerAudio;
    public AudioClip[] JumpS = new AudioClip[6];
    public AudioClip[] StartS = new AudioClip[6];
    public AudioClip Damage,Death;
    public AudioClip pickup;

    public float ViewAngle;

    //Pushing player back
    float BackwardRecoil = 0f;
    float UpwardsRecoil = 0f;
    float HorizontalMomentum;
    float VerticalMomentum;

    void Start()
    {
        //Play start level voiceline
        Invoke("PlayVoiceline", 0.5f);

        //init settings
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        //Init scripts
        FireS = GameObject.FindGameObjectWithTag("WControllerTag").GetComponent<FireScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement and Jumping
        {
            //Ground check
            {
                isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
                OnEmeny = Physics.CheckSphere(groundCheck.position, groundDistance, EnemyMask);

                if ((isGrounded | OnEmeny) && velocity.y < 0)
                {
                    velocity.y = -2f;
                }
            }
            //Movement
            {
                float x = Input.GetAxis("Horizontal");
                float z = Input.GetAxis("Vertical");


                move = (transform.right * x) + (transform.forward * z);
                if (!isGrounded)
                {
                    if (BackwardRecoil > 0)
                    {
                        HorizontalMomentum = (90 - Mathf.Abs(ViewAngle)) / 90;
                        VerticalMomentum = (ViewAngle) / 90;
                        move += (transform.forward * -1 * BackwardRecoil * HorizontalMomentum);
                        if(UpwardsRecoil >0)
                        {
                            velocity.y = UpwardsRecoil * VerticalMomentum;
                            UpwardsRecoil += gravity;
                        } 
                        BackwardRecoil -= 0.1f;  
                    }
                }
                else
                {
                    BackwardRecoil = 0;
                    UpwardsRecoil = 0;
                    VerticalMomentum = 0;
                    HorizontalMomentum = 0;
                }
                controller.Move(move * speed * Time.deltaTime);
            }
            //Jumping & Gravity
            {
                if (Input.GetButtonDown("Jump") && (isGrounded | OnEmeny))
                {
                    velocity.y = Mathf.Sqrt(jumpH * -2f * gravity);
                    PlayerAudio.PlayOneShot(JumpS[Random.Range(0, 5)],0.2f);
                }

                velocity.y += gravity * Time.deltaTime;

                controller.Move(velocity * Time.deltaTime);
            }
        }
        //Guns and stuff
        {
            //Shooting
            {
                if (Input.GetMouseButton(0))
                {
                        FireS.fire(CurrentWeapon);
                }
            }

            {
                if (Input.GetMouseButton(1))
                {
                    FireS.fireSecondary(CurrentWeapon);
                }
            }

            //Reload
            {
                if (Input.GetButtonDown("Reload"))
                {
                   FireS.ReloadGun(CurrentWeapon);
                }
            }

            //Changing Weapons
            {
                float Lastweapon = CurrentWeapon;
                if (!FireS.isReloading)
                {
                    if (Input.GetButtonDown("Weapon 1")) CurrentWeapon = 1;
                    if (ShotgunOwned)
                    {
                        if (Input.GetButtonDown("Weapon 2")) CurrentWeapon = 2;
                        FireS.ChangeSprite(CurrentWeapon);
                    }
                    if (SMGOwned)
                    {
                        if (Input.GetButtonDown("Weapon 3")) CurrentWeapon = 3;
                        FireS.ChangeSprite(CurrentWeapon);
                    }
                    if (SniperOwned)
                    {
                        if (Input.GetButtonDown("Weapon 4")) CurrentWeapon = 4;
                        FireS.ChangeSprite(CurrentWeapon);
                    }
                    if (RLauncherOwned)
                    {
                        if (Input.GetButtonDown("Weapon 5")) CurrentWeapon = 5;
                        FireS.ChangeSprite(CurrentWeapon);
                    }
                    /*if (Input.GetButtonDown("Weapon 6")) CurrentWeapon = 6;
                    if (Input.GetButtonDown("Weapon 7")) CurrentWeapon = 7;
                    if (Input.GetButtonDown("Weapon 8")) CurrentWeapon = 8;
                    if (Input.GetButtonDown("Weapon 9")) CurrentWeapon = 9;
                    if (Input.GetButtonDown("Weapon 0")) CurrentWeapon = 0; */
                }
            }

        }

    }

    //Start level voiceline
    void PlayVoiceline()
    {
        PlayerAudio.PlayOneShot(StartS[Random.Range(0, 5)],0.7f);
    }

    //Set view angle
    public void SetViewAngle(float angle)
    {
        ViewAngle = angle;
    }

    //Add backward punch
    public void BackwardsMomentum(float strength)
    {
        BackwardRecoil = strength;
        UpwardsRecoil = strength*2;
    }

    //Add weapons from pickups
    public void GettingWeapons(int weapon)
    {
        switch(weapon)
        {
            case 2:
                {
                    PlayerAudio.PlayOneShot(pickup, 0.5f);
                    ShotgunOwned = true;
                    break;
                }
            case 3:
                {
                    PlayerAudio.PlayOneShot(pickup, 0.5f);
                    SMGOwned = true;
                    break;
                }
            case 4:
                {
                    PlayerAudio.PlayOneShot(pickup, 0.5f);
                    SniperOwned = true;
                    break;
                }
            case 5:
                {
                    PlayerAudio.PlayOneShot(pickup, 0.5f);
                    RLauncherOwned = true;
                    break;
                }
        }
    }

    //take amount of damage
    public void TakePlayerDamage(float amount)
    {
        PlayerHealth -= amount;
        if (amount > 0)
        {
            if (PlayerHealth > 0)
            {
                PlayerAudio.PlayOneShot(Damage, 0.8f);
            }
            else
            {
                PlayerAudio.PlayOneShot(Death, 0.5f);
                FireS.EndLevel();
                Invoke("QuitGame", 2f);
            }
        }else
        {
            PlayerAudio.PlayOneShot(pickup, 0.5f);
        }
    }
    void QuitGame()
    {
        Application.Quit();
    }
    //draw health
    void OnGUI()
    {
        GUIStyle DefaultStyle = new GUIStyle();
        DefaultStyle.font = ActiveFont;
        DefaultStyle.fontSize = 60;
        GUI.skin.label.fontSize = 15;

        //Health Counter
        GUI.Label(new Rect(10,Screen.height - 120, 300, 60),"Health " + Mathf.RoundToInt(PlayerHealth).ToString(), DefaultStyle);


    }


}
