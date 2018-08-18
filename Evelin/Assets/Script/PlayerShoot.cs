using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


[RequireComponent(typeof (WeaponManager))]

public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";
    
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    [SerializeField]
    private GameObject projectile;

    private WeaponManager weaponManager;
    private WeaponShoot currentWeapon;
    private Rigidbody firedProjectileRigidbody;
    private Projectile firedProjectileScript;

    public GameObject bfire;
    public GameObject sbfire;
    private bool canFire;
    private float actualROF;
    private float fireTimer = 0f;
    public bool recoil = true;
    private Transform raycastStartSpot;
    public Transform weaponHolderInitial;

    public GameObject bulletHole;

    private float currentCrosshairSize;

    private float currentAccuracy;


    private void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot : No Camera referenced!!!");
            this.enabled = false;
        }

        raycastStartSpot = cam.transform;
            
        weaponManager = GetComponent<WeaponManager>();


        //TODO = instead of a getter consider using delegates (also to change the icon highlight)
        //currentWeapon = weaponManager.GetCurrentWeapon();
        //Debug.Log("PlayerShoot Start method :" + weaponManager.GetCurrentWeapon() + " ---- " + currentWeapon);
        //currentCrosshairSize = currentWeapon.startingCrosshairSize;


    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();


        weaponManager.weaponHolder.transform.position = Vector3.Lerp(weaponManager.weaponHolder.transform.position, weaponHolderInitial.position , currentWeapon.recoilRecoveryRate * Time.deltaTime);
        weaponManager.weaponHolder.transform.rotation = Quaternion.Lerp(weaponManager.weaponHolder.transform.rotation, weaponHolderInitial.rotation, currentWeapon.recoilRecoveryRate * Time.deltaTime);

        // Calculate the current accuracy for this weapon
        currentAccuracy = Mathf.Lerp(currentAccuracy, currentWeapon.accuracy, currentWeapon.accuracyRecoverRate * Time.deltaTime);


        // Calculate the current crosshair size.  This is what causes the crosshairs to grow and shrink dynamically while shooting
        currentCrosshairSize = currentWeapon.startingCrosshairSize + (currentWeapon.accuracy - currentAccuracy) * 0.8f;

        

        if (currentWeapon.firerate != 0)
            actualROF = 1.0f / currentWeapon.firerate;
        else
            actualROF = 0.01f;
        // Update the fireTimer
        fireTimer += Time.deltaTime;

        /*if (PauseMenu.IsOn)
        {
            return;
        }*/

        if (currentWeapon.bullets < currentWeapon.maxBullets)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                weaponManager.Reload();
            }
        }


                bfire = GameObject.Find("FixedButton 2");
        FixedButton FireButton = bfire.GetComponent<FixedButton>();

        sbfire = GameObject.Find("FixedButton 2 s");
        FixedButton FireButtonS = sbfire.GetComponent<FixedButton>();

        //JumpAxis = FireButton.Pressed;
     
        if (canFire)
        {
            if (fireTimer >= actualROF)
            { 
            if (FireButton.Pressed || FireButtonS.Pressed) // replaced Input.GetButtonDown("Fire1") by FireButton.Pressed  -- 
                {
                    Shoot();
           
                }
            }
        }
        

        if (FireButton.Pressed == false && FireButtonS.Pressed == false)
        {
            canFire = true;
        }

    
    }

    //Drawing the crosshair
    void OnGUI()
    {

        // Crosshairs
        // Debug.Log("Should we show crosshair = " + currentWeapon.showCrosshair);

        if (currentWeapon)
        { 
        if (currentWeapon.showCrosshair)
        {
            // Hold the location of the center of the screen in a variable
            Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
            //Debug.Log("Center of the screen is = " + center);
            // Draw the crosshairs based on the weapon's inaccuracy
            // Left
            Rect leftRect = new Rect(center.x - currentWeapon.crosshairLength - currentCrosshairSize, center.y - (currentWeapon.crosshairWidth / 2), currentWeapon.crosshairLength, currentWeapon.crosshairWidth);
            GUI.DrawTexture(leftRect, currentWeapon.crosshairTexture, ScaleMode.StretchToFill);
            // Right
            Rect rightRect = new Rect(center.x + currentCrosshairSize, center.y - (currentWeapon.crosshairWidth / 2), currentWeapon.crosshairLength, currentWeapon.crosshairWidth);
            GUI.DrawTexture(rightRect, currentWeapon.crosshairTexture, ScaleMode.StretchToFill);
            // Top
            Rect topRect = new Rect(center.x - (currentWeapon.crosshairWidth / 2), center.y - currentWeapon.crosshairLength - currentCrosshairSize, currentWeapon.crosshairWidth, currentWeapon.crosshairLength);
            GUI.DrawTexture(topRect, currentWeapon.crosshairTexture, ScaleMode.StretchToFill);
            // Bottom
            Rect bottomRect = new Rect(center.x - (currentWeapon.crosshairWidth / 2), center.y + currentCrosshairSize, currentWeapon.crosshairWidth, currentWeapon.crosshairLength);
            GUI.DrawTexture(bottomRect, currentWeapon.crosshairTexture, ScaleMode.StretchToFill);

            GUI.DrawTexture(new Rect(center.x, center.y, 1, 1), currentWeapon.crosshairTexture, ScaleMode.StretchToFill);

        }

        }
    }

    //Is called on the server wehn a player shoots
    [Command]
    void CmdOnShoot ()
    {
        RpcDoShootEffect();
    }

    //Is called on all clients when we need to do a shoot effect
    [ClientRpc]
    void RpcDoShootEffect ()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
        Debug.Log("RPC do shoot effect triggered");
    }

    //Is called on the server when we hit something
    // takes the hit point and normal of the surfce
    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoHitEffect(_pos, _normal);
    }

    //is called on all client can spawn in cool FX
    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
        
    }

    [Client]
    void ShootSpecial ()
    {
        if (!isLocalPlayer || weaponManager.isReloading) //TODO add the logic of the cooldown for the special
        {

            return;
        }

        CmdSpawnSpecialShoot();
        
    }

    [Command]
    void CmdSpawnSpecialShoot ()
    {
       
    }

    [Client]
    void Shoot()
    {
        if (!isLocalPlayer || weaponManager.isReloading)
        {
            
            return;
        }


        // Reset the fireTimer to 0 (for ROF)
        fireTimer = 0.0f;

        Debug.Log("Shoot");

        if (currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            return;
        }

        currentWeapon.bullets--;

        Debug.Log("remaining bullets" + currentWeapon.bullets);

        // We are shooting, calll the OnShoot method on the server
        CmdOnShoot();

        // Fire once for each shotPerRound value
        for (int i = 0; i < currentWeapon.shotPerRound; i++)
        {


            // taking into account the accuracy
            float accuracyVary = (100 - currentAccuracy) / 2000; // TODO : check if value is correct, initially it was set to 1000
            Vector3 direction = raycastStartSpot.forward;
            direction.x += Random.Range(-accuracyVary, accuracyVary);
            direction.y += Random.Range(-accuracyVary, accuracyVary);
            direction.z += Random.Range(-accuracyVary, accuracyVary);
            currentAccuracy -= currentWeapon.accuracyDropPerShot;
            if (currentAccuracy <= 0.0f)
                currentAccuracy = 0.0f;

            Ray ray = new Ray(raycastStartSpot.position, direction);

            RaycastHit _hit;

            if (Physics.Raycast(ray, out _hit, currentWeapon.range, mask))
            //if (Physics.Raycast(raycastStartSpot.position, raycastStartSpot.forward, out _hit, currentWeapon.range, mask))
            //if (Physics.Raycast(cam.transform.position,cam.transform.forward,out _hit, currentWeapon.range, mask))
            {
                // We hit something

                if (_hit.collider.tag == PLAYER_TAG)
                {
                    CmdPlayerShot(_hit.collider.name, currentWeapon.damage, transform.name);
                }

                //place bullet holes

                GameObject bh = Instantiate(bulletHole, _hit.point, Quaternion.FromToRotation(Vector3.up, _hit.normal));
                //bulletHole.transform.position = _hit.point;
                //bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.up, _hit.normal);
                

                // We hit something call the onhit method on the fucking server

                CmdOnHit(_hit.point, _hit.normal);
            }

        }


        if (currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            
        }
        if (currentWeapon.auto == Auto.Semi)
            canFire = false;

        weaponManager.GetCurrentSound().Play();

        // Calculate random values for the recoil position and rotation
        float kickBack = Random.Range(currentWeapon.recoilKickBackMin, currentWeapon.recoilKickBackMax);
        float kickRot = Random.Range(currentWeapon.recoilRotationMin, currentWeapon.recoilRotationMax);

        //Debug.Log("weaponInstanceEquiped " + weaponManager.weaponInstanceEquiped);
        // Apply the random values to the weapon's postion and rotation
        weaponManager.weaponHolder.transform.Translate(new Vector3(0, 0, -kickBack), Space.Self);
        weaponManager.weaponHolder.transform.Rotate(new Vector3(-kickRot, 0, 0), Space.Self);



    }

    [Command]
    void CmdPlayerShot (string _playerID,int _damage, string _sourceID)
    {
        Debug.Log(_playerID + " has been shot");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage, _sourceID);
    }


    [Command]
    public void CmdPlayerSpecialShot(string _playerID, int _damage, string _sourceID)
    {
        Debug.Log(_playerID + " has been shot");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage, _sourceID);
    }

}
