using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Player : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    private Rigidbody rb;
    private Animator animator;
    

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;


    public float GetHealPct()
    {
        return (float)currentHealth / maxHealth;
    }

    [SyncVar]
    public string username = "loading...";

    public int kills;
    public int deaths;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

   
    

    private bool firstSetup = true;



    public void SetupPlayer ()
    {
        if (isLocalPlayer)
        {
            //Switch camera
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);

        }

        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients ()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }
        SetDefaults();
    }

    
     private void Update()
    {
        if (!isLocalPlayer)
            return;
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(9999999, "Player 1");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            RpcTakeDamage(5, "Player 1");
        }

    }
    


    [ClientRpc]
    public void RpcTakeDamage(int _amount, string _sourceID)
    {
        if (isDead)
            return;
        currentHealth -= _amount;

        GameObject bloodImage = GameObject.Find("BloodSplatter");

        var bloodScript = bloodImage.GetComponent<SplatterFade>();

        bloodScript.TakingDamageBloodSplatter(_amount);

        Debug.Log(transform.name + " now has " + currentHealth + " Health");
        if (currentHealth <= 0)
        {
            
            Die(_sourceID);
        }
    }

    private void Die(string _sourceID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(_sourceID);

        if (sourcePlayer != null)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallBack.Invoke(username, sourcePlayer.username);
        }

       

        deaths++;

        

        

        //Spawn a death effect for ROBOT
        //GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        //Destroy(_gfxIns, 3f);
        //Spawn a death animation for Western

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // Place the blood on the sreen
        GameObject bloodImage = GameObject.Find("BloodSplatter");

        var bloodScript = bloodImage.GetComponent<SplatterFade>();

        bloodScript.Die();

        animator = GetComponent<Animator>();
        animator.SetTrigger("Die");
        StartCoroutine(DyingAnimation());

        


    }

    private IEnumerator DyingAnimation()
    {
        Debug.Log("Before yield waitforseconds" + Time.deltaTime);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        Debug.Log("After yield waitforseconds" + Time.deltaTime);

        //Disable component
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //Disable Game objects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        //disable the collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)

            _col.enabled = false;

        //Switch camera
        if (isLocalPlayer)
        {

            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
            GameManager.instance.SetSceneCameraActive(true);
        }

        Debug.Log(transform.name + " is dead!");

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.mactchSettings.respawnTime);

       
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);


        SetupPlayer();

        Debug.Log(transform.name + " respawned");

    }
    

    public void SetDefaults()
    {
        isDead = false;
        
        currentHealth = maxHealth;

        //set component active
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Enable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
        
            _col.enabled = true;



        //create spawn efffect
        //GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        //Destroy(_gfxIns, 3f);
    }
}

internal class rigidbody
{
}