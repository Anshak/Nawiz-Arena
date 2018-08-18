using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    RectTransform thrusterFuelFill;

    [SerializeField]
    RectTransform healthBar;

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject scoreBoard;

    [SerializeField]
    Text ammoText;

    [SerializeField]
    Text deathText;

    [SerializeField]
    Text killText;

    private Player player;
    private PlayerController controller;
    private WeaponManager weaponManager;

    public void SetPlayer(Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();
        weaponManager = player.GetComponent<WeaponManager>();
    }

    private void Start()
    {
        PauseMenu.IsOn = false;
    }

    private void Update()
    {
        
        SetFuelAmount(controller.GetThrusterFuelAmount());
        SetHealthAmount(player.GetHealPct());
        SetAmmoAmount(weaponManager.GetCurrentWeapon().bullets);
        SetDeathAmount(player.deaths);
        SetKillAmount(player.kills);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoard.SetActive(true);

        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.SetActive(false);
        }
    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn = pauseMenu.activeSelf;
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);

    }

    void SetHealthAmount(float _amount)
    {
        healthBar.localScale = new Vector3(1f, _amount, 1f);
    }

    void SetAmmoAmount (int _amount)
    {
        ammoText.text = _amount.ToString();
    }

    void SetDeathAmount (int _amount)
    {
        deathText.text = _amount.ToString();
    }

    void SetKillAmount(int _amount)
    {
        killText.text = _amount.ToString();
    }
}
