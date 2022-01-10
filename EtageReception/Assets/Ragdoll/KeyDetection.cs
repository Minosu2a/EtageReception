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
            AudioManager.Instance.Start2DSound("S_Door");

            StartCoroutine(Victory());

        }
    }

    private IEnumerator Victory()
    { 

                  AudioManager.Instance.Start2DSound("S_Trump");

        yield return new WaitForSeconds(1f);
    AudioManager.Instance.Start2DSound("S_Trump");
        AudioManager.Instance.Start2DSound("S_Door2");
        yield return new WaitForSeconds(1f);
        

        GameStateManager.Instance.LaunchTransition(EGameState.LEVEL1);

    }

}
