using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClemCAddons;

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

    [Header("Rotation")]
    [SerializeField] private Quaternion _rotationRight = Quaternion.identity;
    [SerializeField] private Quaternion _rotationLeft = Quaternion.identity;

    [Header("Smooth")]
    [SerializeField] private float _movementSmoothValue = 0.2f;
    [SerializeField] private float _rotationSmoothValue = 0.2f;

    [Header("Zoom")]
    [SerializeField] private float _zoomFront = 5f;
    [SerializeField] private float _zoomBack = 8f;

    void Start()
    {

    }

    void Update()
    {
        //MOVE THE Z POS
       float ZPlayerPos = Mathf.InverseLerp(_wallFront.position.z, _wallBack.position.z, CharacterManager.Instance.CharacterController.transform.FindDeep("Root").position.z);
       float ZPosToUpdate = Mathf.Lerp(_maximumZ.position.z, _minimumZ.position.z, ZPlayerPos);



        //MOVE THE X POS
        float XPlayerPos = Mathf.InverseLerp(_wallRight.position.x, _wallLeft.position.x, CharacterManager.Instance.CharacterController.transform.FindDeep("Root").position.x);

        float XPosToUpdate = Mathf.Lerp(_maximumX.position.x, _minimumX.position.x, XPlayerPos);


        float YPlayerPos = Mathf.Lerp(_zoomFront, _zoomBack, XPlayerPos);

        Debug.Log(YPlayerPos);

        Vector3 objPos = new Vector3(XPosToUpdate, YPlayerPos, ZPosToUpdate);
        transform.position = Vector3.Lerp(transform.position, objPos, Time.smoothDeltaTime / _movementSmoothValue);



        Quaternion rotCam = Quaternion.Lerp(_rotationLeft, _rotationRight, XPlayerPos);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotCam, Time.smoothDeltaTime / _rotationSmoothValue);
        


    }
}
