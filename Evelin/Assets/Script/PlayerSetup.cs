﻿
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;

    private GameObject minimapCamera;
    private Minimap minimap;

 

    private void Start()
    {

        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        } else
        {
        
            //Disable player graphics for local
            //SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //create Player UI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            //Configure PlayerUI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
                Debug.LogError("No PlayerUi comp  on the player UI fucking prefab");
            ui.SetPlayer(GetComponent<Player>());

            minimapCamera = GameObject.Find("MinimapCamera");
            minimap = minimapCamera.GetComponent<Minimap>();

            minimap.SetPlayer(GetComponent<Transform>());

            GetComponent<Player>().SetupPlayer();

            string _username = "loading...";
            if (UserAccountManager.isLoggedIn)
                _username = UserAccountManager.playerUsername;
            else
                _username = transform.name;

            CmdSetUsername(transform.name, _username);

        }

        

    }

    [Command]

    void CmdSetUsername (string playerID, string username)
    {
        Player player = GameManager.GetPlayer(playerID);

        if (player != null)
        {
            Debug.Log(username + " has joined");
                player.username = username;
        }
    }

    void SetLayerRecursively (GameObject obj,int newLayer)
    {
        obj.layer = newLayer;

      foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }


    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        GameManager.RegisterPlayer(_netID,_player);
    }


    void AssignRemoteLayer ()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    private void OnDisable()
    {

        Destroy(playerUIInstance);

        if (isLocalPlayer)
        GameManager.instance.SetSceneCameraActive(true);

        GameManager.UnRegisterPlayer(transform.name);
    }
}
