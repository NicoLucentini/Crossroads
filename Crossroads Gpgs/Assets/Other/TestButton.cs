using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestButton : MonoBehaviour
{
    
    public string printSomething;
	

    public void PrintSomething()
    {
        Debug.Log(printSomething);
    }
	
}
