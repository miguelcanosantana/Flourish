using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollectableScript : MonoBehaviour
{
    [Header("Stats")]
    public GunItemInfoClass itemToAdd;


    //References
    private Camera playerCamera;
    private GameManagerScript gameManagerScript;


    // Start is called before the first frame update
    void Start()
    {
        //Get the player's main camera
        playerCamera = Camera.main;

        //Get the GameManager
        gameManagerScript = FindObjectOfType<GameManagerScript>();
    }


    // Update is called once per frame
    void Update()
    {
        UpdateUIPosition();
    }


    //Update the position of the UI
    private void UpdateUIPosition()
    {
        transform.LookAt(playerCamera.transform.position);
        transform.Rotate(0, 180, 0);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }


    //When the player enters the collision
    private void OnTriggerEnter(Collider collider)
    {
        //Destroy the card and add it to the player bar
        if (collider.CompareTag("Player"))
        {
            gameManagerScript.playerGunItems.Add(itemToAdd);
            gameManagerScript.RefreshBarUI();
            Destroy(gameObject);
        }
    }
}
