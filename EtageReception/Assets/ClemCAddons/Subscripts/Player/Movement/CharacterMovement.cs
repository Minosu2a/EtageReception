using UnityEngine;
using Luminosity.IO;
using ClemCAddons.Utilities;
using System.Diagnostics;
using ClemCAddons.CameraAndNodes;
using Debug = UnityEngine.Debug;

namespace ClemCAddons
{
    namespace Player
    {
        public class CharacterMovement : MonoBehaviour
        {
            [Header("Collision")]
            [SerializeField] private LayerMask _collisionLayer = 8;

            [Header("Camera")]
            [SerializeField] private bool _isUsingTPS = true;

            [Header("Gliding")]
            [SerializeField] private float _glidingGravity = 0.5f;
            [SerializeField] private float _maxGlidingFallSpeed = 0.5f;
            [SerializeField] private float _glidingDeceleration = 2;
            [SerializeField] [LabelOverride("Gliding Speed")] private float _airControl = 0.3f;


            [Header("Walking")]
            [SerializeField] private float _maxSpeed = 10;
            [SerializeField] private float _maxGroundAdaptation = 10;
            [SerializeField] private float _groundAdaptationSpeed = 10;

            [Header("Jumping & Gravity")]
            [SerializeField] private float _jumpStrength = 5;

            [SerializeField] private float _maxFallSpeed = 10;

            [SerializeField] private float _maxBounceHeight = 10;
            [SerializeField] private float _bounceFactor = 0.1f;
            [SerializeField] private float _multiplier = 10f;
            [SerializeField] private float _inputMultiplier = 10f;

            [Header("Jump Buffering")]
            [SerializeField, LabelOverride("Post Jump Buffering (s)")] private float _postJumpBuffering = 0.2f;
            [SerializeField, LabelOverride("Pre Jump Buffering (s)")] private float _preJumpBuffering = 0.2f;

            private Vector3 _impulse = new Vector3();
            private Vector3 _immediateImpulse = new Vector3();
            private Vector3 _direction = new Vector3();
            private Vector3 _localVelocity;
            private float groundDistance;
            private float _fallingHeight = 0f;
            private bool _doNotMove = false;
            private Collider _collider;
            private Rigidbody _rigidbody;
            private bool _firstTimeGround;
            private bool _isOnGround;
            private bool _isFalling = true;
            private NodeBasedCamera _camera;
            private TPSCameraWithNodeSupport _tpsCamera;
            private float _jumpDelay = 0;
            private float _postJumpDelay = 0;

            public LayerMask CollisionLayer { get => _collisionLayer; set => _collisionLayer = value; }
            public Rigidbody Rigidbody
            {
                get
                {
                    if (_rigidbody == null)
                    {
                        _rigidbody = _rigidbody = GetComponent<Rigidbody>();
                    }
                    return _rigidbody;
                }
                set
                {
                    _rigidbody = value;
                }
            }
            public bool IsOnGround { get => _isOnGround; set => _isOnGround = value; }
            public bool IsFalling { get => _isFalling; set => _isFalling = value; }
            public float GroundDistance { get => groundDistance; set => groundDistance = value; }

            public bool IsWalking
            {
                get
                {
                    return (_direction.x.Abs() > 0 || _direction.z.Abs() > 0) && IsOnGround;
                }
            }

            void Start()
            {
                _collider = GetComponent<Collider>();
                _rigidbody = GetComponent<Rigidbody>();
                if (_isUsingTPS)
                    _tpsCamera = FindObjectOfType<TPSCameraWithNodeSupport>();
                else
                    _camera = FindObjectOfType<NodeBasedCamera>();
            }

            void LateUpdate()
            {
                UpdateInputs();
                _localVelocity = new Vector3();
                groundDistance = GameTools.FindGround(transform.position, _collider.bounds.extents.y, 10, _collisionLayer, out RaycastHit hit);
                _firstTimeGround = !_isOnGround && groundDistance <= 0.1; // if is on ground is false, has a chance to be true if it becomes true.
                if(groundDistance <= 0.1 && _isOnGround && _postJumpDelay <= 0)
                {
                    _postJumpDelay = _postJumpBuffering;
                }
                _isOnGround = groundDistance <= 0.1 || _postJumpDelay > 0;
                if(groundDistance >= 0.1 && _postJumpDelay > 0)
                {
                    _postJumpDelay -= Time.deltaTime;
                }
                _isFalling = _rigidbody.velocity.y < 0;
                _fallingHeight = _firstTimeGround ? 0 : _fallingHeight;
                if (!_isOnGround && groundDistance <= _rigidbody.velocity.y.Abs() * _preJumpBuffering && _isFalling)
                {
                    _isOnGround = true;
                }
                if (!_isOnGround && _isFalling && _direction.y > 0)
                {
                    _rigidbody.useGravity = false;
                    if (Mathf.Abs(_rigidbody.velocity.y) > _maxGlidingFallSpeed)
                    {
                        _localVelocity.y += (_glidingDeceleration - _glidingGravity) * Time.smoothDeltaTime;
                    }
                    else
                    {
                        _impulse.y -= _glidingGravity * Time.smoothDeltaTime;
                    }
                    _fallingHeight = 0;
                    _direction.y = 0; // consume direction.y
                }
                else
                {
                    _rigidbody.useGravity = true;
                    if (!_isOnGround && _isFalling)
                    {
                        _fallingHeight -= _rigidbody.velocity.y * Time.smoothDeltaTime; // y velocity is negative as _isfalling and !_isOnGround
                    }
                }
                float multiplier = _isOnGround ? 1 : _airControl;
                _direction *= multiplier;
                if (_isOnGround)
                {
                    Vector3 r = hit.normal.ToQuaternion(transform.rotation).eulerAngles.SetY(0);
                    if (r.x.MinusAngle(0,true).Abs() > _maxGroundAdaptation)
                    {
                        r.x = transform.Find("Base").eulerAngles.x;
                    }
                    if (r.z.MinusAngle(0, true).Abs() > _maxGroundAdaptation)
                    {
                        r.z = transform.Find("Base").eulerAngles.z;
                    }
                    transform.Find("Base").rotation = Quaternion.Lerp(transform.Find("Base").rotation, r.ToQuaternion(), Time.smoothDeltaTime * _groundAdaptationSpeed);
                    if (_direction.y > 0 && _rigidbody.velocity.y <= 0 && _jumpDelay <= 0 && _doNotMove == false)
                    {
                        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z); // set rigidbody velocity y to 0
                        _impulse += new Vector3(0, _jumpStrength);
                        _jumpDelay += 0.05f;
                    }
                }
                _jumpDelay = Mathf.Max(0, _jumpDelay - Time.smoothDeltaTime);
                _localVelocity.x = _direction.x * _inputMultiplier;
                _localVelocity.z = _direction.z * _inputMultiplier;
                _localVelocity = _localVelocity.ClampXZTotal(_inputMultiplier);
                Vector3 _aim = (_isUsingTPS? _tpsCamera.transform:_camera.transform).forward;
                _aim.y = 0;
                if (_aim == new Vector3())
                {
                    _aim = transform.forward;
                }
                ApplyImpulse();
                _localVelocity = _doNotMove ? new Vector3() : _localVelocity;
                _immediateImpulse = _doNotMove ? new Vector3() : _immediateImpulse;
                if ((_localVelocity + _immediateImpulse) == Vector3.zero)
                {
                    _rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                }
                else
                {
                    _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                }
                Vector3 velocityFinal = _localVelocity.Remap(_aim);
                velocityFinal.y += _rigidbody.velocity.y;
                velocityFinal = velocityFinal.ClampXZKeepRelation(-_maxSpeed, _maxSpeed);
                velocityFinal = velocityFinal.ClampXZTotal(_maxSpeed);
                velocityFinal = velocityFinal.ClampY(-_maxFallSpeed, _maxFallSpeed);
                _rigidbody.velocity = _immediateImpulse + velocityFinal;
                _immediateImpulse = new Vector3();
            }

            public void SetCanMove(bool canMove)
            {
                _doNotMove = !canMove;
            }
            //private bool _tempStoreOfValue = false;
            //private Stopwatch stopwatch = new Stopwatch();
            private void UpdateInputs()
            {
                _direction.x = InputManager.GetAxis("Horizontal");
                _direction.z = InputManager.GetAxis("Vertical");
                _direction.y = InputManager.GetButton("Jump") ? 1 : 0;
                // ###### A UTILISER SI IL Y A BESOIN DE MESURER la durée d'arrêt complet ######
                //if(Mathf.Abs(_direction.x) == 1)
                //{
                //    _tempStoreOfValue = true;
                //} else
                //{
                //    if (_tempStoreOfValue)
                //    {
                //        if (!stopwatch.IsRunning)
                //        {
                //            stopwatch.Start();
                //        }
                //        if(_rigidbody.velocity.magnitude == 0)
                //        {
                //            stopwatch.Stop();
                //            UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
                //            UnityEngine.Debug.Log(stopwatch.ElapsedTicks);
                //            _tempStoreOfValue = false;
                //            stopwatch.Reset();
                //        }
                //    }
                //}
            }

            private void ApplyImpulse()
            {
                _rigidbody.AddForce(_impulse, ForceMode.Impulse);
                _impulse = new Vector3();
            }

            public void Bounce(float value, bool autoHeight = false)
            {
                if (autoHeight)
                {
                    _impulse.y += Mathf.Min(_maxBounceHeight, value + _fallingHeight * _bounceFactor);
                    _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
                }
                else
                {
                    _impulse.y += Mathf.Min(_maxBounceHeight, value);
                    _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
                }
            }

            public void Bounce(float value, float height)
            {
                _impulse.y += Mathf.Min(_maxBounceHeight, value + height * _bounceFactor);
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            }

            
            public void AddImpulse(Vector3 value)
            {
                _immediateImpulse += value;
            }

            public void Push(float value)
            {
                _rigidbody.velocity += new Vector3(0, value);
            }
        }
    }
}
