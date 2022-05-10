using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


//Enum with all the bee faces expressions
public enum BeeExpression
{
    None,
    Smile,
    Lost,
}


public class BeeAiScript : MonoBehaviour
{
    [Header("Bee ID")]
    public string id = "None";

    [Header("Stats")]
    public FlowerType flowerTypeMatch;
    public int recollectionAmount;
    [HideInInspector] public List<GameObject> listOfFlowers = new List<GameObject>();
    [HideInInspector] public GameObject targetFlower;
    [HideInInspector] public FlowerScript targetFlowerScript;
    [HideInInspector] public GameObject previousFlower;
    [HideInInspector] public bool allowRecollecting;
    [HideInInspector] public bool alreadyTweening;
    [HideInInspector] public bool canUpdateFlowers = true;
    [HideInInspector] public float loadedPollen = 0f;
    [HideInInspector] public Sequence flySequence;
    [HideInInspector] public bool canUpdatePosedTimer = true;
    [HideInInspector] public int timerSinceLastPosed = 0;
    [HideInInspector] public float maxTimeToPose = 0f;
    [HideInInspector] public bool isLost = false;
    [HideInInspector] public float timeLost = 0f;
    [HideInInspector] public bool isBeingSucked = false;
    [HideInInspector] public float angularSpeedBackup = 0f;
    [HideInInspector] public float speedBackup = 0f;
    [HideInInspector] public float accelerationBackup = 0f;
    private BeeExpression currentFace = BeeExpression.None;

    [Header("References")]
    [SerializeField] private SaveDataScriptable saveData;
    public GameObject beeContainerObject;
    public Animator animator;
    public GameObject rangeChecker;
    private RangeFlowerCheckScript rangeCheckScript;
    private NavMeshAgent agent;
    private BeeStateClass currentState; //The current state in the FSM (Finite State Machine)
    private SkinnedMeshRenderer skinMeshRender;
    [HideInInspector] public GameObject beeParentObject;

    [Header("Face Textures")]
    [SerializeField] private Material smileFace;
    [SerializeField] private Material confusedFace;


    //Start
    void Start()
    {
        //Get the components
        agent = GetComponent<NavMeshAgent>();
        rangeCheckScript = rangeChecker.GetComponent<RangeFlowerCheckScript>();
        skinMeshRender = beeContainerObject.GetComponentInChildren<SkinnedMeshRenderer>();
        beeParentObject = beeContainerObject.transform.parent.gameObject;

        //Get these properties
        angularSpeedBackup = agent.angularSpeed;
        speedBackup = agent.speed;
        accelerationBackup = agent.acceleration;

        //Start the FSM in idle state
        currentState = new Idle(gameObject, this);
    }


    //Update
    void Update()
    {
        currentState = currentState.Process();
    }


    //Set the bee face expression
    public void SetExpression(BeeExpression expression)
    {
        //Select a material depending on the expression (materials need to be set all at once, otherwise they won't work)
        switch (expression)
        {
            case BeeExpression.None:
                break;

            case BeeExpression.Smile:
                skinMeshRender.materials = new Material[]{ skinMeshRender.materials[0], smileFace};
                break;

            case BeeExpression.Lost:
                skinMeshRender.materials = new Material[] { skinMeshRender.materials[0], confusedFace };
                break;
        }
    }


    //Save Bee when called from GameManager
    public void SaveBee()
    {
        //If flower id is None, Generate a guid and convert it to string
        if (id == "None") id = Guid.NewGuid().ToString();

        saveData.SaveBee(
            id,
            agent.transform.position,
            agent.transform.rotation,
            flowerTypeMatch,
            recollectionAmount);
    }


    //Coroutine for FSM => Update the list of flowers the bee can travel to (Using a timer to increase performance)
    public IEnumerator UpdateFlowersList()
    {
        canUpdateFlowers = false;

        //Get the flowers from the range checker sphere trigger
        listOfFlowers = rangeCheckScript.GetNearFlowersOfType(flowerTypeMatch);

        yield return new WaitForSeconds(0.5f);
        canUpdateFlowers = true;
    }


    //Coroutine for FSM => Wait a time and recollect some pollen from the flower 
    public IEnumerator RecollectPollen(float time)
    {
        allowRecollecting = true;

        yield return new WaitForSeconds(time);
        FlowerScript flowerScript = targetFlower.GetComponent<FlowerScript>();
        loadedPollen += flowerScript.TryTakePollen(recollectionAmount);

        allowRecollecting = false;
    }


    //Coroutine for FSM => Update the timer since last posed 
    public IEnumerator UpdateLastPosedTimer()
    {
        canUpdatePosedTimer = false;

        yield return new WaitForSeconds(1f);

        timerSinceLastPosed ++;

        //Debug.Log(timerSinceLastPosed);

        canUpdatePosedTimer = true;
    }


    //Destroy the bee
    public IEnumerator DestroyBee()
    {
        beeContainerObject.transform.DOScale(0f, 1f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(1f);

        Destroy(beeParentObject);
    }

}
