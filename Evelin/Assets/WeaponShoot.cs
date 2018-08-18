using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Auto
{
    Full,
    Semi
}

public class WeaponShoot : MonoBehaviour {


    // TODO switch to scriptable objects

    //General weapon data
    [Header("General weapon data")]
    public string weaponName = "Pistol";
    public int damage = 10;
    public float range = 100f;
    public float firerate = 10f;
    public float actualROF;
    public int maxBullets = 20;
    public float reloadTime = 1f;
    public Auto auto = Auto.Semi;
    public GameObject graphics;
    public AudioClip fireSound;
    public float weaponReloadSpeedFactor = 0.3f;
    public int shotPerRound = 1;

    //recoil
    [Header("Recoil")]
    public float recoilKickBackMin;
    public float recoilKickBackMax;
    public float recoilRotationMin;
    public float recoilRotationMax;
    public float recoilRecoveryRate = 0.01f;

    // Accuracy
    [Header("Accuracy")]
    public float accuracy = 80.0f;                      // How accurate this weapon is on a scale of 0 to 100
    private float currentAccuracy;                      // Holds the current accuracy.  Used for varying accuracy based on speed, etc.
    public float accuracyDropPerShot = 1.0f;            // How much the accuracy will decrease on each shot
    public float accuracyRecoverRate = 0.1f;            // How quickly the accuracy recovers after each shot (value between 0 and 1)

    //Crosshair
    [Header("Crosshair")]
    public bool showCrosshair = true;                   // Whether or not the crosshair should be displayed
    public Texture2D crosshairTexture;                  // The texture used to draw the crosshair
    public int crosshairLength = 10;                    // The length of each crosshair line
    public int crosshairWidth = 4;                      // The width of each crosshair line
    public float startingCrosshairSize = 10.0f;

    //The two fire buttons
    public GameObject bfire;
    public GameObject sbfire;

    [HideInInspector]
    public int bullets;

    public void PlayerWeapon()
    {
        bullets = maxBullets;
    }


    // Use this for initialization
    void Start () {
        // weaponManager = GetComponent<WeaponManager>();

    
    }
	
	// Update is called once per frame
	void Update () {



    }
}
