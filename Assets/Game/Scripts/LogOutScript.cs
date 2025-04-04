using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOutScript : MonoBehaviour
{
    public GameObject LogOutpanel;
    
 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnToLogOut() { 
        LogOutpanel.SetActive(true);
    
    } 
}
