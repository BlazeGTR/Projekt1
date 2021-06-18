using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    //Pistol
    [Header("Pistol Settings")]
    public int PistolDmg = 25;
    public float PistolSpread = 0.1f;
    public float PistolCD = 0.4f;
    public float PistolAltCD = 1.2f;
    public int Pistolammo = 6;
    public float PistolReloadTime = 1f;
    int PistolMaxammo = 6;
    int TotalPistolammo = 48;

    //Shotgun
    [Header("Shotgun Settings")]
    public int ShotgunDmg = 5;
    public int ShotgunPellets = 12;
    public float ShotgunSpread = 0.4f;
    public float ShotgunCD = 1.2f;
    public float ShotgunReloadTime = 2f;
    int Shotgunammo = 5;
    public int ShotgunMaxammo = 5;
    int TotalShotgunammo = 30;
    public float ShotgunPunch = 10;

    //SMG
    [Header("SMG Settings")]
    public float SMGCD = 0.07f;
    public int SMGDmg = 5;
    public float SMGSpread = 0.2f;
    public float SMGReloadTime = 1f;
    int SMGAmmo = 30;
    int SMGMaxAmmo = 30;
    int TotalSMGAmmo = 150;

    //Sniper
    [Header("Sniper Settings")]
    public float SniperCD = 1.5f;
    public float SniperDmg = 50;
    public float SniperReloadTime = 2f;
    int SniperAmmo = 5;
    int SniperMaxAmmo = 5;
    int TotalSniperAmmo = 30;

    //Rocket Launcher
    [Header("RocketLauncher Settings")]
    public float RLauncherCD = 0.5f;
    public float RLaunchForce = 900f;
    public float RLauncherReloadTime = 2f;
    public GameObject rocket;
    int RLauncherAmmo = 2;
    int RLauncherMaxAmmo = 2;
    int TotalRLauncherAmmo = 10;

    //Audios
    [Header("Audio Settings")]
    public float ShotVolume = 0.5f;
    public AudioSource audioSource;
    public AudioClip pistolAudio,shotgunAudio,SMGAudio,SniperAudio, RLauncherAudio;
    public AudioClip pistolReloadAudio, shotgunReloadAudio, SMGReloadAudio,SniperReloadAudio, RLauncherReloadAudio;


    public Camera fpsCam;

    //Effects
    [Header("Effects")]
    public GameObject bloodsplatter;
    public GameObject BulletHole;
    public GameObject BulletTracer;

    public Font ActiveFont;

    //Weapon Sprites
    [Header("Sprites")]
    public Animator weaponsanim;
    public SpriteRenderer spriteRenderer;
    public Sprite ShotgunS;
    public Sprite PistolS;
    public Sprite SniperS;
    public Sprite RLauncherS;
    public Sprite LMGS;
    public Sprite SMGS;

    //Gun Handling
    float ShootCD;
    public float ReloadTimer;
    int HeldWeapon = 1;
    int HeldWeapon2 = 1;
    public bool isReloading = false;

    GameObject[] npcs;
    float GameTimer = 0f;
    bool LevelEnded = false;
    public bool Multiplayer;

    PlayerMovement PlayerM;
    int BurstAmount;

    private void Start()
    {
        PlayerM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }
    //do hit effects
    void Vhit(float damage, RaycastHit Rhit)
    {
        Debug.Log(Rhit.transform.gameObject.layer);
        Debug.Log(Rhit.transform.name);
        switch(Rhit.transform.gameObject.layer)
        {
            case 11:
                {
                    GameObject impactEF = Instantiate(bloodsplatter, Rhit.point, Quaternion.LookRotation(Rhit.normal));
                    Destroy(impactEF, 2f);
                    Target target = Rhit.transform.GetComponent<Target>();
                    if (target != null)
                    {
                        target.TakeDamage(damage);
                    }
                    break;
                }
            case 8:
                {
                    GameObject impactHole = Instantiate(BulletHole, Rhit.point, Quaternion.LookRotation(Rhit.normal) * Quaternion.Euler(0, 0, Random.Range(-180, 180)), Rhit.transform);
                    Destroy(impactHole, 120f);
                    break;
                }
        }
    }

    //Finish level
    public void EndLevel()
    {
        LevelEnded = true;
    }

    private void Update()
    {
        //Game Timer
        if (!LevelEnded)
        {
            GameTimer += Time.deltaTime;
        }

        //Shooting Cooldown
            ShootCD -= Time.deltaTime;
        weaponsanim.SetFloat("WeaponCooldown", ShootCD);
            ReloadTimer -= Time.deltaTime;
        weaponsanim.SetFloat("ReloadTimer", ReloadTimer);

        //Is level over?
        npcs = GameObject.FindGameObjectsWithTag("Enemy");
        if(npcs.Length == 0)
        {
            Invoke("QuitGame", 3f);
            LevelEnded = true;
        }
    }

    void QuitGame()
    {
        Application.Quit();
    }
    //randomize aim
    Vector3 RandomSpread(Vector3 direction, float Accuracy)
    {
        // Vector3 direction = fpsCam.transform.forward; // your initial aim.
        Vector3 spread = direction;
        spread += fpsCam.transform.up * Random.Range(-Accuracy, Accuracy); // add random up or down (because random can get negative too)
        spread += fpsCam.transform.right * Random.Range(-Accuracy, Accuracy); // add random left or right
        direction += spread.normalized * Random.Range(0f, 0.2f);
        return direction;
    }

    //select weapon and do raycasts
    public void fire(int Weapon)
    {
        if (ShootCD <= 0 && !isReloading && ReloadTimer <= 0)
        {
            RaycastHit hit;
            int layerMask = 1 << 10;
            layerMask = ~layerMask;
            switch (Weapon)
            {
                //Pistol
                case 1:
                    {
                        if (Pistolammo > 0)
                        {
                            ShootCD = PistolCD;
                            audioSource.PlayOneShot(pistolAudio, ShotVolume);
                            Vector3 directionPistol = RandomSpread(fpsCam.transform.forward, PistolSpread);
                            GameObject TracerEF = Instantiate(BulletTracer, fpsCam.transform.position - Vector3.up, Quaternion.LookRotation(directionPistol));
                            Destroy(TracerEF, 4f);
                            if (Physics.Raycast(fpsCam.transform.position, directionPistol, out hit, Mathf.Infinity, layerMask))
                            {
                                Vhit(PistolDmg, hit);
                            }
                            Pistolammo--;
                            TotalPistolammo--;
                        }
                        break;
                    }

                //Shotgun
                case 2:
                    {
                        if (Shotgunammo > 0)
                        {
                            PlayerM.BackwardsMomentum(ShotgunPunch);
                            ShootCD = ShotgunCD;
                            audioSource.PlayOneShot(shotgunAudio, ShotVolume);
                            for (int i = 0; i <= ShotgunPellets-1; i++)
                            {
                                Vector3 directionShotgun = RandomSpread(fpsCam.transform.forward, ShotgunSpread);
                                GameObject TracerEF = Instantiate(BulletTracer, fpsCam.transform.position - Vector3.up, Quaternion.LookRotation(directionShotgun));
                                Destroy(TracerEF, 4f);
                                if (Physics.Raycast(fpsCam.transform.position, directionShotgun, out hit, Mathf.Infinity, layerMask))
                                {
                                    Vhit(ShotgunDmg, hit);
                                }
                            }
                            Shotgunammo--;
                            TotalShotgunammo--;
                        }
                        break;
                    }

                //SMG
                case 3:
                    {
                        if (SMGAmmo > 0)
                        {
                            ShootCD = SMGCD;
                            audioSource.PlayOneShot(SMGAudio, ShotVolume + 0.4f);
                            Vector3 directionSMG = RandomSpread(fpsCam.transform.forward, SMGSpread);
                            GameObject TracerEF = Instantiate(BulletTracer, fpsCam.transform.position - Vector3.up, Quaternion.LookRotation(directionSMG));
                            Destroy(TracerEF, 4f);
                            if (Physics.Raycast(fpsCam.transform.position, directionSMG, out hit, Mathf.Infinity, layerMask))
                            {
                                Vhit(SMGDmg, hit);
                            }
                            SMGAmmo--;
                            TotalSMGAmmo--;

                        }
                        break;
                    }

                //Sniper
                case 4:
                    {
                        if (SniperAmmo > 0)
                        {
                            ShootCD = SniperCD;
                            audioSource.PlayOneShot(SniperAudio, ShotVolume + 0.4f);
                            Vector3 directionSniper = RandomSpread(fpsCam.transform.forward, 0);
                            GameObject TracerEF = Instantiate(BulletTracer, fpsCam.transform.position - Vector3.up, Quaternion.LookRotation(directionSniper));
                            Destroy(TracerEF, 4f);
                            if (Physics.Raycast(fpsCam.transform.position, directionSniper, out hit, Mathf.Infinity, layerMask))
                            {
                                Vhit(SniperDmg, hit);
                            }
                            SniperAmmo--;
                            TotalSniperAmmo--;

                        }
                        break;
                    }

                //Rocket
                case 5:
                    {
                        if(RLauncherAmmo > 0)
                        {
                            ShootCD = RLauncherCD;
                            audioSource.PlayOneShot(RLauncherAudio, ShotVolume + 0.2f);
                            Vector3 directionRLauncher = RandomSpread(fpsCam.transform.forward, 0);
                            if (RLauncherAmmo % 2 == 0)
                            {
                                LaunchPojectile(rocket,fpsCam.transform.position - Vector3.right * 0.15f, Quaternion.LookRotation(directionRLauncher), RLaunchForce);
                            }else
                            {
                                LaunchPojectile(rocket,fpsCam.transform.position + Vector3.right * 0.15f, Quaternion.LookRotation(directionRLauncher), RLaunchForce);
                            }

                            RLauncherAmmo--;
                            TotalRLauncherAmmo--;
                        }

                        break;
                    }

                case 6:

                    break;

                case 7:

                    break;

                case 8:

                    break;

                case 9:

                    break;

                case 0:

                    break;
            }
        }
    }

    //Same as above but with alt fire
    public void fireSecondary(int Weapon)
    {
        if (ShootCD <= 0 && !isReloading && ReloadTimer <= 0)
        {
            RaycastHit hit;
            int layerMask = 1 << 10;
            layerMask = ~layerMask;
            switch (Weapon)
            {
                //Pistol
                case 1:
                    {
                        if (Pistolammo > 0)
                        {
                            weaponsanim.SetBool("IsFanning", true);;
                            float Delay = 0.1f;
                            BurstAmount = Pistolammo -1;
                            Invoke("stopFanning", Delay * Pistolammo);
                            for (float i = 0; i<Pistolammo;)
                            {
                                Invoke("RevolverFan", Delay*(Pistolammo-1));
                                Pistolammo--;
                                TotalPistolammo--;
                            }
                        }
                        break;
                    }
            }
        }
    }

    //Why the fuck do i have to do this
    //Revoler fanning
    public void RevolverFan()
    {
        RaycastHit hit;
        int layerMask = 1 << 10;
        layerMask = ~layerMask;
        audioSource.PlayOneShot(pistolAudio, ShotVolume);
        Vector3 directionPistol = RandomSpread(fpsCam.transform.forward, PistolSpread);
        GameObject TracerEF = Instantiate(BulletTracer, fpsCam.transform.position - Vector3.up, Quaternion.LookRotation(directionPistol));
        Destroy(TracerEF, 4f);
        if (Physics.Raycast(fpsCam.transform.position, directionPistol, out hit, Mathf.Infinity, layerMask))
        {
            Vhit(PistolDmg, hit);
        }
    }

    public void stopFanning()
    {
        weaponsanim.SetBool("IsFanning", false);
        ReloadGun(1);
    }

    //Launch projectile weapons
    void LaunchPojectile(GameObject projectilePrefab,Vector3 LaunchLocation,Quaternion rotation, float force)
    { 
        GameObject projectileLaunched = Instantiate(projectilePrefab, LaunchLocation - Vector3.up, rotation);
        Rigidbody rb = projectileLaunched.GetComponent<Rigidbody>();
        rb.AddForce(fpsCam.transform.forward * force);
    }

    //change sprites for current weapons
    public void ChangeSprite(int weapon)
    {
        HeldWeapon = weapon;
        weaponsanim.SetInteger("CurrentWeapon", weapon);
    }

    //do reloading shit
    public float ReloadGun(int CurrentWeapon)
    {
        if (!isReloading && ShootCD <= 0)
        {
            HeldWeapon2 = CurrentWeapon;
            switch (CurrentWeapon)
            {
                case 1:
                    {
                        if (Pistolammo != PistolMaxammo)
                        {
                            isReloading = true;
                            ReloadTimer = PistolReloadTime;
                            audioSource.PlayOneShot(pistolReloadAudio, 1.2f);
                            weaponsanim.SetBool("IsReloading", isReloading);
                            Invoke("Reload", PistolReloadTime);//reload after x seconds                            
                        }
                        break;
                    }

                case 2:
                    {
                        if (Shotgunammo != ShotgunMaxammo)
                        {
                            isReloading = true;
                            ReloadTimer = ShotgunReloadTime;
                            weaponsanim.SetBool("IsReloading", isReloading);
                            audioSource.PlayOneShot(shotgunReloadAudio, 0.5f);
                            Invoke("Reload", ShotgunReloadTime);
                        }
                        break;
                    }

                case 3:
                    {
                        if (SMGAmmo != SMGMaxAmmo)
                        {
                            isReloading = true;
                            weaponsanim.SetBool("IsReloading", isReloading);
                            ReloadTimer = SMGReloadTime;
                            audioSource.PlayOneShot(pistolReloadAudio, 0.5f);
                            Invoke("Reload", SMGReloadTime);
                        }
                        break;
                    }

                case 4:
                    {
                        if (SniperAmmo != SniperMaxAmmo)
                        {
                            isReloading = true;
                            weaponsanim.SetBool("IsReloading", isReloading);
                            ReloadTimer = SniperReloadTime;
                            audioSource.PlayOneShot(SniperReloadAudio, 0.5f);
                            Invoke("Reload", SniperReloadTime);
                        }
                        break;
                    }


                case 5:
                    {
                        if (RLauncherAmmo != RLauncherMaxAmmo)
                        {
                            isReloading = true;
                            weaponsanim.SetBool("IsReloading", isReloading);
                            ReloadTimer = RLauncherReloadTime;
                            audioSource.PlayOneShot(RLauncherReloadAudio, 0.5f);
                            Invoke("Reload", RLauncherReloadTime);
                        }
                        break;
                    }
            }
            return ReloadTimer;
        }
        else
        {
            return 0;
        }
    }

    public void Reload()
    {
        switch (HeldWeapon2)
        {
            case 1:
                {
                    Pistolammo = PistolMaxammo;
                    break;
                }
            case 2:
                {
                    Shotgunammo = ShotgunMaxammo;
                    break;
                }
            case 3:
                {
                    SMGAmmo = SMGMaxAmmo;
                    break;
                }
            case 4:
                {
                    SniperAmmo = SniperMaxAmmo;
                    break;
                }
            case 5:
                {
                    RLauncherAmmo = RLauncherMaxAmmo;
                    break;
                }
        }
        isReloading = false;
        weaponsanim.SetBool("IsReloading", isReloading);
    }
    //draw ammo counter
    private void OnGUI()
    {
        GUIStyle DefaultStyle = new GUIStyle();
        DefaultStyle.font = ActiveFont;
        DefaultStyle.fontSize = 70;

        //Enemies
            GUI.Label(new Rect(40, 40, 300, 60), "Enemies: " + npcs.Length.ToString(), DefaultStyle);

        //Timer
        float Timer = Mathf.Round(GameTimer * 100) / 100;
        GUI.Label(new Rect(Screen.width / 2, 30, 300, 60), Timer.ToString(), DefaultStyle);

        //Ammo
        switch (HeldWeapon)
        {
            case 1:
                {
                    GUI.Label(new Rect(Screen.width - 230, Screen.height - 120, 300, 60), "Ammo " + Pistolammo.ToString(), DefaultStyle);
                    break;
                }
            case 2:
                {
                    GUI.Label(new Rect(Screen.width - 230, Screen.height - 120, 300, 60), "Ammo " + Shotgunammo.ToString(), DefaultStyle);
                    break;
                }
            case 3:
                {
                    GUI.Label(new Rect(Screen.width - 230, Screen.height - 120, 300, 60), "Ammo " + SMGAmmo.ToString(), DefaultStyle);
                    break;
                }
            case 4:
                {
                    GUI.Label(new Rect(Screen.width - 230, Screen.height - 120, 300, 60), "Ammo " + SniperAmmo.ToString(), DefaultStyle);
                    break;
               }
            case 5:
                {
                    GUI.Label(new Rect(Screen.width - 230, Screen.height - 120, 300, 60), "Ammo " + RLauncherAmmo.ToString(), DefaultStyle);
                    break;
                }
        }
    }
}