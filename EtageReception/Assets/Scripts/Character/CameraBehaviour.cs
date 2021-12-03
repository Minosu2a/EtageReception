using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClemCAddons;

public class CameraBehaviour : MonoBehaviour
{


    #region Comments
    /*  [Header("Devant/Derrière")]
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

     */
    #endregion Comments
    //IL FAUT UN OBJECTIF DE LA CAM POUR X ET S'ADAPTER
    //POSITION Z TOUJOURS LA MÊME
    //TRANSFORM FORWARD DE LA CAMERA A MODIFIER (ROTATION A METTRE EN COMPTE)

    [SerializeField] private Transform _centerOfMap = null;

    [Header("X Camera Borders")]
    [SerializeField] private Transform _rightMaximumXCamPos = null;
    [SerializeField] private Transform _leftMaximumXCamPos = null;

    [Header("X Player Borders (Wall)")]
    [SerializeField] private Transform _rightMaximumXPlayerPos = null;
    [SerializeField] private Transform _leftMaximumXPlayerPos = null;

    [SerializeField] private float _leftRotationMaxLooking = 334.5f;
    [SerializeField] private float _rightRotationMaxLooking = 65.5f;


    [Header("Z Distance")]
    [SerializeField] private float _zConstantDistance = 3f;

    [Header("Y Distance")]
    [SerializeField] private float _yConstantDistance = 6f;

    [Header("X Rotation")]
    [SerializeField] private float _xConstantRotation = 20f;

    private bool _isOnRightSide = false;
    private float _xRightPercentage = 0;
    private float _xLeftPercentage = 0;

    private Quaternion _leftQuaternion = Quaternion.identity;
    private Quaternion _rightQuaternion = Quaternion.identity;

    private Quaternion _defaultQuaternion = Quaternion.identity;

    void Start()
    {
        _defaultQuaternion.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    void Update()
    {
        Transform characterTransform = CharacterManager.Instance.CharacterController.transform.FindDeep("Root");

        //----------------- CONFIG 1 ----------------------//

        /*

        //CACLUL DE L'OBJECTIF DE LA CAMERA X
        if (characterTransform.position.x > _centerOfMap.transform.position.x)
        {
            //CALCUL DE POURCENTAGE POUR LA POSITION X (Droit)
            _xRightPercentage = Mathf.InverseLerp(_centerOfMap.position.x, _rightMaximumXPlayerPos.position.x, characterTransform.position.x);
            _isOnRightSide = true;
            Debug.Log("Test Step 1 Right");


            _rightQuaternion = transform.rotation;
            _rightQuaternion.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, _rightRotationMaxLooking, 0);
        }
        else
        {
            //CALCUL DE POURCENTAGE POUR LA POSITION X (Gauche)
            _xLeftPercentage = Mathf.InverseLerp(_centerOfMap.position.x, _leftMaximumXPlayerPos.position.x, characterTransform.position.x);
            _isOnRightSide = false;
            Debug.Log("Test Step 1 Left");


            //CALCUL DE LA ROTATION QUAND ON EST SUR LA GAUCHE
            _leftQuaternion = transform.rotation;
            _leftQuaternion.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, _leftRotationMaxLooking, 0);
        }



        //DISTANCE Z DE LA CAMERA (ZconstanstDistance)
        //float zPosition = characterTransform.position.z - _zConstantDistance;

        //Vector3 leForwardEHEH = new Vector3(this.transform.position.x, characterTransform.position.x);
        Debug.Log("Test Step 2");



        //CALCUL DE LA POSITION DE LA CAMERA
        if (_isOnRightSide == true)
        {
            float XPos = Mathf.Lerp(_centerOfMap.position.x, _leftMaximumXCamPos.position.x, _xRightPercentage);
            transform.position = new Vector3(XPos, _yConstantDistance, transform.position.z);

            transform.rotation = Quaternion.Lerp(_defaultQuaternion, _rightQuaternion, _xRightPercentage);
            Debug.Log("Test Step 3 Right");

        }
        else
        {
            float XPos = Mathf.Lerp(_centerOfMap.position.x, _rightMaximumXCamPos.position.x, _xLeftPercentage);
            transform.position = new Vector3(XPos, _yConstantDistance, transform.position.z);

            transform.rotation = Quaternion.Lerp(_defaultQuaternion, _leftQuaternion, _xLeftPercentage);

            Debug.Log("Test Step 3 Left");
        }

        */

        //----------------- CONFIG 2 ----------------------//


        //CACLUL DE L'OBJECTIF DE LA CAMERA X
        if (characterTransform.position.x > _centerOfMap.transform.position.x)
        {
            //CALCUL DE POURCENTAGE POUR LA POSITION X (Droit)
            _xRightPercentage = Mathf.InverseLerp(_centerOfMap.position.x, _rightMaximumXPlayerPos.position.x, characterTransform.position.x);
            _isOnRightSide = true;
            Debug.Log("Test Step 1 Right");


            _rightQuaternion = transform.rotation;
            _rightQuaternion.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, _rightRotationMaxLooking, 0);
        }
        else
        {
            //CALCUL DE POURCENTAGE POUR LA POSITION X (Gauche)
            _xLeftPercentage = Mathf.InverseLerp(_centerOfMap.position.x, _leftMaximumXPlayerPos.position.x, characterTransform.position.x);
            _isOnRightSide = false;
            Debug.Log("Test Step 1 Left");


            //CALCUL DE LA ROTATION QUAND ON EST SUR LA GAUCHE
            _leftQuaternion = transform.rotation;
            _leftQuaternion.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, _leftRotationMaxLooking, 0);
        }



        //DISTANCE Z DE LA CAMERA (ZconstanstDistance)
        //float zPosition = characterTransform.position.z - _zConstantDistance;

        //Vector3 leForwardEHEH = new Vector3(this.transform.position.x, characterTransform.position.x);
        Debug.Log("Test Step 2");



        //CALCUL DE LA POSITION DE LA CAMERA
        if (_isOnRightSide == true)
        {
            float XPos = Mathf.Lerp(_centerOfMap.position.x, _rightMaximumXCamPos.position.x, _xRightPercentage);
            transform.position = new Vector3(XPos, _yConstantDistance, transform.position.z);

            transform.rotation = Quaternion.Lerp(_defaultQuaternion, _rightQuaternion, _xRightPercentage);
            Debug.Log("Test Step 3 Right");

        }
        else
        {
            float XPos = Mathf.Lerp(_centerOfMap.position.x, _leftMaximumXCamPos.position.x, _xLeftPercentage);
            transform.position = new Vector3(XPos, _yConstantDistance, transform.position.z);

            transform.rotation = Quaternion.Lerp(_defaultQuaternion, _leftQuaternion, _xLeftPercentage);

            Debug.Log("Test Step 3 Left");
        }


    }
}
