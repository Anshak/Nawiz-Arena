using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    int damageCaused = 25;
    [SerializeField] GameObject player;
    PlayerShoot playerShootScript;

    private const string PLAYER_TAG = "Player";

    public string shooterName;

    public void SetDamage (int damage)
    {
        damageCaused = damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log ("tag du collider est " + collision.collider.tag);
        Debug.Log("name du collider est " + collision.collider.name);

        /*if (collision.collider.tag == PLAYER_TAG)
        {
            playerShootScript = player.GetComponent<PlayerShoot>();
            playerShootScript.CmdPlayerSpecialShot(collision.collider.name, damageCaused, transform.name);
        }
        */

        GameObject hit = collision.gameObject;
        playerShootScript = hit.GetComponent<PlayerShoot>();
        Debug.Log("tag du hit est " + hit.tag);
        Debug.Log("name du hit est " + hit.name);
        if (playerShootScript != null)
            playerShootScript.CmdPlayerSpecialShot(hit.name, damageCaused, playerShootScript.name);

        Destroy(gameObject);
    }

}
