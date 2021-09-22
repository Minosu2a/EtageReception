using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDetection : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Key")
        {
            //VICTORY
            Debug.Log("Victory");
        }
    }

}
