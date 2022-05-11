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
    PurpleBee,
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
    [SerializeField] private GameObject tornadoObject;
    [SerializeField] private GameObject cameraContainer;
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private GameObject regularBeePrefab;
    [SerializeField] private GameObject purpleBeePrefab;
    [SerializeField] private GameObject menuBackground;
    private GameObject seedsFolder;
    private GameObject beesFolder;
    private CharacterController playerController;
    private GameManagerScript gameManagerScript;
    private GunItemInfoClass currentItem;
    private GameObject tempSeed;
    private GameObject tempBee;
    private List<GameObject> nearBees = new List<GameObject>();
    private List<GameObject> nearFlowers = new List<GameObject>();

    //Variables
    private Vector2 movementInput = Vector2.zero;
    private Vector2 mouseInput = Vector2.zero;
    private Vector2 scrollActionInput = Vector2.zero;
    private float cameraYRotation;
    private GunAction gunAction;
    private bool canShoot = true;


    //Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        //Get folders
        seedsFolder = GameObject.FindGameObjectWithTag("SeedsFolder");
        beesFolder = GameObject.FindGameObjectWithTag("BeesFolder");

        //Get components
        playerController = GetComponent<CharacterController>();
        gameManagerScript = FindObjectOfType<GameManagerScript>();
    }


    //Event => When pressing escape key or gamepad select, rotate the player's camera
    public void OnEscapeMenuPress(InputAction.CallbackContext context)
    {
        //Toggle if the game is paused or not
        gameManagerScript.isGamePaused = !gameManagerScript.isGamePaused;

        //Set active if the game is paused
        menuBackground.SetActive(gameManagerScript.isGamePaused);
    }


    //Event => OnClickSave



    //Calculate the happiness levels and refresh upper bar ui
    public void CalculateHappiness()
    {
        //Calculate the ratio, if there are more bees, divide by bees, if there are more flowers, divide by flowers
        //Ratio assures there's an equal amount of bees and flowers
        float ratio = 1f;
        if (nearBees.Count > nearFlowers.Count) ratio = (float) nearFlowers.Count / (float) nearBees.Count;
        if (nearBees.Count < nearFlowers.Count) ratio = (float) nearBees.Count / (float) nearFlowers.Count;

        //Debug.Log(ratio);

        //Get the total items and multiply them by the ratio
        int totalItems = nearBees.Count + nearFlowers.Count;
        gameManagerScript.playerHappiness = Mathf.Clamp((int)(totalItems * ratio), 0, 100);
        gameManagerScript.RefreshUpperUi();
    }


    //When objects enter the player range add them to the list
    private void OnTriggerEnter(Collider collider)
    {
        CalculateHappiness();

        //If the object is not already on the list and it's a bee, add it
        if (!nearBees.Contains(collider.gameObject) && (collider.gameObject.CompareTag("Bee")))
        {
            nearBees.Add(collider.gameObject);
        }

        //If the object is not already on the list and it's a flower, add it
        if (!nearFlowers.Contains(collider.gameObject) && (collider.gameObject.CompareTag("Flower")))
        {
            nearFlowers.Add(collider.gameObject);
        }
    }


    //When objects exit the player range remove them from the list
    private void OnTriggerExit(Collider collider)
    {
        CalculateHappiness();

        //If on the list and it's a bee, remove it
        if (nearBees.Contains(collider.gameObject))
        {
            nearBees.Remove(collider.gameObject);
        }

        //If on the list and it's a flower, remove it
        if (nearFlowers.Contains(collider.gameObject))
        {
            nearFlowers.Remove(collider.gameObject);
        }
    }


    //Update is called once per frame
    void Update()
    {
        Move();
        Rotate();

        //Do an action depending on the shoot action
        switch (gunAction)
        {
            case GunAction.None:
                tornadoObject.SetActive(false);
                break;

            //Shoot if allowed
            case GunAction.Fire:
                tornadoObject.SetActive(false);
                if (canShoot && gunAction == GunAction.Fire) StartCoroutine(Shoot());
                break;

            //Suck
            case GunAction.Suck:
                tornadoObject.SetActive(true);
                break;
        }
    }


    //Coroutine => Shoot if the player can do it
    private IEnumerator Shoot()
    {
        canShoot = false;

        //If the current item is not null, has amount and is > 0, shoot it
        if (currentItem != null && currentItem.hasAmount && currentItem.itemAmount > 0)
        {
            //Remove item and update ui
            currentItem.itemAmount --;
            gameManagerScript.RefreshBarUI();

            //If the current type is a bee, launch the bee
            if (currentItem.itemType == GunItemType.RegularBee || currentItem.itemType == GunItemType.PurpleBee)
            {

                //Instantiate a new bee depending on it's type
                switch (currentItem.itemType)
                {
                    case GunItemType.RegularBee:
                        tempBee = Instantiate(regularBeePrefab, gunSpawnPoint.transform);
                        break;

                    case GunItemType.PurpleBee:
                        tempBee = Instantiate(purpleBeePrefab, gunSpawnPoint.transform);
                        break;
                }
                
                tempBee.transform.parent = beesFolder.transform;

                //Give force to the seed
                //Rigidbody rigidbody = tempSeed.GetComponent<Rigidbody>();
                //rigidbody.velocity = Vector3.zero;
                //rigidbody.angularVelocity = Vector3.zero;
                //rigidbody.AddRelativeForce(Vector3.forward * 25, ForceMode.Impulse);

                //Reset bee
                tempBee = null;
            }


            //If the current type is a plant, launch a seed
            if (currentItem.itemType == GunItemType.SunFlower || currentItem.itemType == GunItemType.Tulip)
            {
                //Try to get an inactive seed, if no luck, instantiate a new one
                foreach (Transform seed in seedsFolder.transform)
                {
                    if (seed.gameObject.activeSelf == false)
                    {
                        tempSeed = seed.gameObject;
                        break;
                    }
                }

                //If the seed is null, instantiate a new one
                if (tempSeed == null)
                {
                    tempSeed = Instantiate(seedPrefab, gunSpawnPoint.transform);
                    tempSeed.transform.parent = seedsFolder.transform;
                }
                //Recover an existing seed
                else
                {
                    tempSeed.transform.position = gunSpawnPoint.transform.position;
                    tempSeed.transform.rotation = gunSpawnPoint.transform.rotation;
                    tempSeed.SetActive(true);
                }

                //Set the seed type
                SeedScript seedScript = tempSeed.GetComponent<SeedScript>();

                //Convert gunType to flowerType
                switch (currentItem.itemType)
                {
                    case GunItemType.SunFlower:
                        seedScript.seedFlowerType = FlowerType.Sunflower;
                        break;

                    case GunItemType.Tulip:
                        seedScript.seedFlowerType = FlowerType.Tulip;
                        break;
                }

                //Give force to the seed
                Rigidbody rigidbody = tempSeed.GetComponent<Rigidbody>();
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.AddRelativeForce(Vector3.forward * 25, ForceMode.Impulse);

                //Reset seed
                tempSeed = null;
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
            if (scrollActionInput.x > 0 || scrollActionInput.y > 0) gameManagerScript.currentItemBarPosition ++;

            //Decrease position
            if (scrollActionInput.x < 0 || scrollActionInput.y < 0) gameManagerScript.currentItemBarPosition--;

            //Travel backwards / forwards depending on the items count
            int itemsCount = gameManagerScript.playerGunItems.Count;

            if (gameManagerScript.currentItemBarPosition >= itemsCount) gameManagerScript.currentItemBarPosition = 0;
            if (gameManagerScript.currentItemBarPosition < 0) gameManagerScript.currentItemBarPosition = itemsCount - 1;

            //Set the current item
            currentItem = gameManagerScript.playerGunItems[gameManagerScript.currentItemBarPosition];
            //Debug.Log(currentItem.itemType.ToString());

            //Move the selected item frame to the current item
            gameManagerScript.MoveSelectedFrame(0.25f);
        }
            
    }

}
