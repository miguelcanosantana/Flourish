using DG.Tweening;
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
public enum GunItemType
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
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float mouseMultiplier;
    [SerializeField] private float shootRate;

    //References
    [Header("References")]
    [SerializeField] private SaveDataScriptable saveData;
    [SerializeField] private Transform gunSpawnPoint;
    [SerializeField] private GameObject cameraContainer;
    [SerializeField] private GameObject seedPrefab;
    private GameObject seedsFolder;
    private CharacterController playerController;
    private GameManagerScript gameManagerScript;
    private GunItemSaveClass currentItem;

    //Variables
    private Vector2 movementInput = Vector2.zero;
    private Vector2 mouseInput = Vector2.zero;
    private Vector2 scrollActionInput = Vector2.zero;
    private float cameraYRotation;
    private GunAction gunAction;
    private bool canShoot = true;
    private List<GameObject> seedsPool = new List<GameObject>();
    private int currentItemBarPosition = 0;


    //Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;

        //Get objects
        seedsFolder = GameObject.FindGameObjectWithTag("SeedsFolder");

        //Get components
        playerController = GetComponent<CharacterController>();
        gameManagerScript = FindObjectOfType<GameManagerScript>();
    }


    //Update is called once per frame
    void Update()
    {
        Move();
        Rotate();

        //Shoot if allowed
        if (canShoot && gunAction == GunAction.Fire) StartCoroutine(Shoot());
    }


    //Coroutine => Shoot if the player can do it
    private IEnumerator Shoot()
    {
        canShoot = false;

        //Debug.Log("PEW");

        //Shoot a seed (from the pool or a new one)
        //if (seedsPool.Count > 0)
        //{
        //    seedsPool[0].SetActive(true);
        //}
        //else
        //{
        //    GameObject tempSeed = Instantiate(seedPrefab);
        //}

        //If the current item is not null, has amount and is > 0, shoot it
        if (currentItem != null && currentItem.hasAmount && currentItem.itemAmount > 0)
        {
            //Remove item and update ui
            currentItem.itemAmount --;
            gameManagerScript.RefreshBarUI();

            //If the current type is a plant, launch a seed
            if (currentItem.itemType == GunItemType.SunFlower || currentItem.itemType == GunItemType.Tulip)
            {
                //Instantiate a new seed
                GameObject tempSeed = Instantiate(seedPrefab, gunSpawnPoint.transform);
                tempSeed.transform.parent = seedsFolder.transform;

                //Set the seed type
                SeedScript seedScript = tempSeed.GetComponent<SeedScript>();

                //Convert gunType to flowerType
                switch (currentItem.itemType)
                {
                    case GunItemType.SunFlower:
                        seedScript.seedFlowerType = FlowerType.Sunflower;
                        break;

                    case GunItemType.Tulip:
                        seedScript.seedFlowerType = FlowerType.None;
                        break;
                }

                //Give force to the seed
                tempSeed.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 25, ForceMode.Impulse);
            }
        }






        yield return new WaitForSeconds(shootRate);

        canShoot = true;
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
        cameraYRotation += mouseInput.y * mouseMultiplier;
        cameraYRotation = Mathf.Clamp(cameraYRotation, -60, 60);
        cameraContainer.transform.localEulerAngles = new Vector3(-cameraYRotation, 0, 0);
    }


    //Save the player transform in the Persistent scriptable object
    public void SavePlayerTransform()
    {
        saveData.SavePlayerTransform(transform.position, transform.rotation);
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

        //Debug.Log(gunAction);
    }


    //Event => On right click, suck
    public void OnRightSuckInput(InputAction.CallbackContext context)
    {
        bool tempBool = context.ReadValueAsButton();

        if (tempBool) gunAction = GunAction.Suck;
        else gunAction = GunAction.None;

        //Debug.Log(gunAction);
    }


    //Event => When moving the scroll wheel or the d-pad, change between player actions
    public void OnChangeActionInput(InputAction.CallbackContext context)
    {
        scrollActionInput = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Started)
        {
            //Increase position
            if (scrollActionInput.x > 0 || scrollActionInput.y > 0) currentItemBarPosition ++;

            //Decrease position
            if (scrollActionInput.x < 0 || scrollActionInput.y < 0) currentItemBarPosition --;

            //Travel backwards / forwards depending on the items count
            int itemsCount = gameManagerScript.playerGunItems.Count;

            if (currentItemBarPosition >= itemsCount) currentItemBarPosition = 0;
            if (currentItemBarPosition < 0) currentItemBarPosition = itemsCount - 1;

            //Set the current item
            currentItem = gameManagerScript.playerGunItems[currentItemBarPosition];
            Debug.Log(currentItem.itemType.ToString());

            //Move the selected item frame to the current item
            GameObject currentItemObject = gameManagerScript.itemsBarContent.transform.GetChild(currentItemBarPosition).gameObject;
            gameManagerScript.selectedItemFrame.transform.DOMove(currentItemObject.transform.position, 0.25f).SetEase(Ease.OutExpo);
        }
            
    }

}
