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
        _defaultQuaternion.eulerAngles = new Vector3(20, 0, 0);
    }

    void Update()
    {
   


        //CACLUL DE L'OBJECTIF DE LA CAMERA X
        if(CharacterManager.Instance.CharacterController.transform.GetChild(0).GetChild(0).transform.position.x > _centerOfMap.transform.position.x)
        {
            //CALCUL DE POURCENTAGE POUR LA POSITION X (Droit)
            _xRightPercentage = Mathf.InverseLerp(_centerOfMap.position.x, _rightMaximumXPlayerPos.position.x, CharacterManager.Instance.CharacterController.transform.GetChild(0).GetChild(0).transform.position.x);
            _isOnRightSide = true;

            _rightQuaternion = transform.rotation;
            _rightQuaternion.eulerAngles = new Vector3(_xConstantRotation, _rightRotationMaxLooking, 0);
        }
        else
        {
            //CALCUL DE POURCENTAGE POUR LA POSITION X (Gauche)
            _xLeftPercentage = Mathf.InverseLerp(_centerOfMap.position.x, _leftMaximumXPlayerPos.position.x, CharacterManager.Instance.CharacterController.transform.GetChild(0).GetChild(0).transform.position.x);
            _isOnRightSide = false;
            

            //CALCUL DE LA ROTATION QUAND ON EST SUR LA GAUCHE
            _leftQuaternion = transform.rotation;
            _leftQuaternion.eulerAngles = new Vector3(_xConstantRotation, _leftRotationMaxLooking, 0);
        }



        //DISTANCE Z DE LA CAMERA (ZconstanstDistance)
        float zPosition = CharacterManager.Instance.CharacterController.transform.GetChild(0).GetChild(0).transform.position.z - _zConstantDistance;

        Vector3 leForwardEHEH = new Vector3(this.transform.position.x, CharacterManager.Instance.CharacterController.transform.GetChild(0).GetChild(0).transform.position.x);



        //CALCUL DE LA POSITION DE LA CAMERA
        if(_isOnRightSide == true)
        {
            float XPos = Mathf.Lerp(_centerOfMap.position.x, _leftMaximumXCamPos.position.x, _xRightPercentage);
            transform.position = new Vector3(XPos, _yConstantDistance, zPosition);

            transform.rotation = Quaternion.Lerp(_defaultQuaternion, _rightQuaternion, _xRightPercentage);
        }
        else
        {
            float XPos = Mathf.Lerp(_centerOfMap.position.x, _rightMaximumXCamPos.position.x, _xLeftPercentage);
            transform.position = new Vector3(XPos, _yConstantDistance, zPosition);

            transform.rotation = Quaternion.Lerp(_defaultQuaternion, _leftQuaternion, _xLeftPercentage);
        }

        //REGARDER LE JOUEUR
        //PLUS LA ROTATION EN FONCTION DU X
        //transform.forward = leForwardEHEH;


        //CALCULER L'OBJECTIF (Check le pourcentage de la position du player entre le centerOfMap et le maximumPlayerPos)
        //APPLIQUER CE MÊME POURCENTAGE SUR LA POSITION DE LA CAMERA AVEC UN LERP (MAXIMUMCAMPOSRIGHT, MAXIMUMCAMPOSLEFT, pourcentage trouvé)





        /*
        //transform.LookAt(CharacterManager.Instance.CharacterController.transform.GetChild(0).GetChild(0));
        //MOVE THE Z POS
       float ZPlayerPos = Mathf.InverseLerp(_wallFront.position.z, _wallBack.position.z, CharacterManager.Instance.CharacterController.transform.FindDeep("Root").position.z);
       float ZPosToUpdate = Mathf.Lerp(_maximumZ.position.z, _minimumZ.position.z, ZPlayerPos);



        //MOVE THE X POS
        float XPlayerPos = Mathf.InverseLerp(_wallRight.position.x, _wallLeft.position.x, CharacterManager.Instance.CharacterController.transform.FindDeep("Root").position.x);

        float XPosToUpdate = Mathf.Lerp(_maximumX.position.x, _minimumX.position.x, XPlayerPos);


        float YPlayerPos = Mathf.Lerp(_zoomFront, _zoomBack, XPlayerPos);   


        Vector3 objPos = new Vector3(XPosToUpdate, YPlayerPos, ZPosToUpdate);
        transform.position = Vector3.Lerp(transform.position, objPos, Time.smoothDeltaTime / _movementSmoothValue);


        //ROTATION ON THE CORNER OF THE ROOM
        Quaternion rotCam = Quaternion.Lerp(_rotationLeft, _rotationRight, XPlayerPos); 

        transform.rotation = Quaternion.Lerp(transform.rotation, rotCam, Time.smoothDeltaTime / _rotationSmoothValue);*/



    }
}
