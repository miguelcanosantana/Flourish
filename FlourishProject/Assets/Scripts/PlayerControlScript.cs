using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControlScript : MonoBehaviour
{

    //Variables
    [Header("Stats")]
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float mouseMultiplier;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 mouseInput = Vector2.zero;
    private float cameraYRotation;


    //References
    [Header("References")]
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

}
