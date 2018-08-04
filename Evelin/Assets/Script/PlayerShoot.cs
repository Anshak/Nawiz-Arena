using UnityEngine;
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
    private PlayerWeapon currentWeapon;
    private Rigidbody firedProjectileRigidbody;
    private Projectile firedProjectileScript;

    public GameObject bfire;

    private void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot : No Camera referenced!!!");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();

    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();   

        if (PauseMenu.IsOn)
        {
            return;
        }

        if (currentWeapon.bullets < currentWeapon.maxBullets)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                weaponManager.Reload();
            }
        }

        bfire = GameObject.Find("FixedButton 2");
        FixedButton FireButton = bfire.GetComponent<FixedButton>();
                               
        //JumpAxis = FireButton.Pressed;

        if (FireButton.Pressed) // replaced Input.GetButtonDown("Fire1") by FireButton.Pressed
        {
            Shoot();

        }

        /*if (CrossPlatformInputManager.GetButtonDown("Fire2"))
        {
            ShootSpecial();

        }*/
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
        //create a bullet from prefab
        GameObject firedProjectile = Instantiate(projectile) as GameObject;

        //choose the position 
        firedProjectile.transform.position = cam.transform.position + new Vector3(2, 1, 1);

        //Add velocity to the bullet
        firedProjectileRigidbody = firedProjectile.GetComponent<Rigidbody>();
        firedProjectileRigidbody.velocity = cam.transform.forward * 20;

        //assign source to projectile
        firedProjectileScript = firedProjectile.GetComponent<Projectile>();
        Debug.Log("The fucking shooter is " + transform.name);
        firedProjectileScript.shooterName    = transform.name;

        //spawn the bullet on the server
        NetworkServer.Spawn(firedProjectile);
    }

    [Client]
    void Shoot()
    {
        if (!isLocalPlayer || weaponManager.isReloading)
        {
            
            return;
        }

        if (currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            return;
        }

        currentWeapon.bullets--;

        Debug.Log("remaining bullets" + currentWeapon.bullets);

        // We are shooting, calll the OnShoot method on the server
        CmdOnShoot();
            
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position,cam.transform.forward,out _hit, currentWeapon.range, mask))
        {
            // We hit something

            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage, transform.name);
            }

            // We hit something call the onhit method on the fucking server

            CmdOnHit(_hit.point, _hit.normal);
        }
        if (currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            
        }
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
