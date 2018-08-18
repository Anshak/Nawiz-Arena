    using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    
    public Transform weaponHolder;

    /// <summary>
    ///     [SerializeField]
    ///private PlayerWeapon primaryWeapon;
    /// </summary>
    
    
    public WeaponShoot primaryWeapon;

    [SerializeField]
    private WeaponShoot m4;
    public bool hasM4 = false;


    //private PlayerWeapon currentWeapon;
    public WeaponShoot currentWeapon;
    

    private WeaponGraphics currentGraphics;

    public bool isReloading = false;
    private AudioSource fireSound;
    [SerializeField]
    private AudioClip reloadSound;
    public GameObject weaponInstanceEquiped = null;
    private GameObject weaponToUse;
    


    // Use this for initialization
    void Start () {
        EquipeWeapon(primaryWeapon);
	}
	
	public WeaponShoot GetCurrentWeapon ()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    public AudioSource GetCurrentSound()
    {
        return fireSound;
    }

    public void EquipeWeapon (WeaponShoot weaponToUse )
    {

        foreach (Transform child in weaponHolder)
        {
            GameObject.Destroy(child.gameObject);
        }

            currentWeapon = weaponToUse;

        GameObject weaponInstanceEquiped = (GameObject) Instantiate(weaponToUse.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponInstanceEquiped.transform.SetParent(weaponHolder);

        Debug.Log("Weapon"+ weaponToUse.name + "eauiped " + weaponHolder.position);

        currentGraphics = weaponInstanceEquiped.GetComponent<WeaponGraphics>();
        fireSound = weaponInstanceEquiped.GetComponent<AudioSource>();

        if (currentGraphics == null)
            Debug.Log("No fucking WeaponGraphics component on the" + weaponInstanceEquiped.name + "weapon");

        if (isLocalPlayer)
            Util.SetLayerRecursively  (weaponInstanceEquiped, LayerMask.NameToLayer(weaponLayerName));
        Reload();   
	}

    public void Reload ()
    {
        if (isReloading)
            return;
        GetComponent<AudioSource>().PlayOneShot(reloadSound);
        StartCoroutine(Reload_Coroutine());
    }

    private IEnumerator Reload_Coroutine ()
    {
        isReloading = true;

        CmdOnReload();

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        currentWeapon.bullets = currentWeapon.maxBullets;

        isReloading = false;
    }

    [Command]
    void CmdOnReload ()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload ()
    {
        Animator anim = currentGraphics.GetComponent<Animator>();
        if (anim != null)
        {
            anim.speed = currentWeapon.weaponReloadSpeedFactor;
          anim.SetTrigger("Reload");
        }
    }

}
