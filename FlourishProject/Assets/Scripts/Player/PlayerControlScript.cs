using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//All the actions that can be performed form the gun
public enum GunAction
{
    None,
    Fire,
    Suck
}


//All the item types that go along with a gun action
public enum GunItemTypes
{
    None,
    RegularBee,
    SunFlower,
    Tulip
}



public class PlayerControlScript : MonoBehaviour
{

    //Variables
    [Header("Stats")]
    public int availableBees;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float mouseMultiplier;

    private Vector2 movementInput = Vector2.zero;
    private Vector2 mouseInput = Vector2.zero;
    private Vector2 scrollActionInput = Vector2.zero;
    private float cameraYRotation;
    private GunAction gunAction;


    //References
    [Header("References")]
    [SerializeField] private Transform beeSpawnPoint;
    [SerializeField] private GameObject cameraContainer;
    private CharacterController playerController;
    

    //Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;

        //Get components
        playerController = GetComponent<CharacterController>();
    }


    //Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
    }


    //Move the player 
    private void Move()
    {
        //Vectors depending on player's rotation
        Vector3 localForward = transform.TransformDirection(Vector3.forward);
        Vector3 localRight = transform.TransformDirection(Vector3.right);

        //Combine moves
        Vector3 combinedMoves = ((localForward * movementInput.y) + (localRight * movementInput.x)) * speedMultiplier;

        //Move in both axis
        playerController.SimpleMove(combinedMoves);
    }


    //Rotate the player and the camera
    private void Rotate()
    {
        //Rotate the player along Y axis using mouse X position
        transform.Rotate(0, mouseInput.x * mouseMultiplier, 0);

        //Rotate the camera along X axis using mouse Y position
        //cameraYRotation += Mathf.Clamp(mouseInput.y, -60, 60);
        cameraYRotation += mouseInput.y;
        cameraYRotation = Mathf.Clamp(cameraYRotation, -60, 60);
        cameraContainer.transform.localEulerAngles = new Vector3(-cameraYRotation, 0, 0);
    }


    //Event => When pressing WASD, arrows, etc keys, move the camera
    public void OnMoveKeysInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }


    //Event => When moving the mouse, rotate the player's camera
    public void OnMoveMouseInput(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
    }


    //Event => On left click, shoot
    public void OnLeftShootInput(InputAction.CallbackContext context)
    {
        bool tempBool = context.ReadValueAsButton();

        if (tempBool) gunAction = GunAction.Fire;
        else gunAction = GunAction.None;

        Debug.Log(gunAction);
    }


    //Event => On right click, suck
    public void OnRightSuckInput(InputAction.CallbackContext context)
    {
        bool tempBool = context.ReadValueAsButton();

        if (tempBool) gunAction = GunAction.Suck;
        else gunAction = GunAction.None;

        Debug.Log(gunAction);
    }


    //Event => When moving the scroll wheel or the d-pad, change between player actions
    public void OnChangeActionInput(InputAction.CallbackContext context)
    {
        scrollActionInput = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Started) Debug.Log(scrollActionInput);
    }

}
