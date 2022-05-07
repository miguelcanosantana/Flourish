using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollectableScript : MonoBehaviour
{
    //References
    private Camera playerCamera;


    // Start is called before the first frame update
    void Start()
    {
        //Get the player's main camera
        playerCamera = Camera.main;
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
}
