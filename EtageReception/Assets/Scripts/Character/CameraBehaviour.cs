using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [Header("Devant/Derrière")]
    [Tooltip("Le point le plus proche du mur de devant (étagère)")]
    [SerializeField] private Transform _maximumZ;

    [Tooltip("Le point le plus éloigné du mur de devant (étagère)")]
    [SerializeField] private Transform _minimumZ;

    [SerializeField] private Transform _wallFront;
    [SerializeField] private Transform _wallBack;


    [Header("Droite/Gauche")]
    [Tooltip("Le point le plus proche du mur de droite")]
    [SerializeField] private Transform _maximumX;

    [Tooltip("Le point le plus proche du mur de gauche")]
    [SerializeField] private Transform _minimumX;

    [SerializeField] private Transform _wallRight;
    [SerializeField] private Transform _wallLeft;

    [Header("Smooth")]
    [SerializeField] float _smoothValue = 0.2f;

    void Start()
    {
        
    }

    void Update()
    {
        //MOVE THE Z POS
       float ZPlayerPos = Mathf.InverseLerp(_wallFront.position.z, _wallBack.position.z, CharacterManager.Instance.CharacterController.transform.position.z);

       float ZPosToUpdate = Mathf.Lerp(_maximumZ.position.z, _minimumZ.position.z, ZPlayerPos);



        //MOVE THE X POS
        float XPlayerPos = Mathf.InverseLerp(_wallRight.position.x, _wallLeft.position.x, CharacterManager.Instance.CharacterController.transform.position.x);

        float XPosToUpdate = Mathf.Lerp(_maximumX.position.x, _minimumX.position.x, XPlayerPos);



        Vector3 objPos = new Vector3(XPosToUpdate, transform.position.y, ZPosToUpdate);
        transform.position = Vector3.Lerp(transform.position, objPos, Time.smoothDeltaTime / _smoothValue);


    }
}
