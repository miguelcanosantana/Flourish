using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoadScreenScript : MonoBehaviour
{

    //Don't destroy the loading screen object when loading the new scene
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    //Destroy the canvas after a while
    public IEnumerator DestroyCanvas()
    {
        yield return new WaitForSeconds(0f);
        Destroy(gameObject);
    }
}
