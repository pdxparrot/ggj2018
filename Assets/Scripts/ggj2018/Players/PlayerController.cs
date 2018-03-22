using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.ggj2018.World;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.Players
{
    public sealed class PlayerController : Core.Players.PlayerController
    {
        [Serializable]
        private struct PauseState
        {
            private Vector3 _velocity;

            public Vector3 Velocity => _velocity;

            public void Save(Rigidbody rigidbody)
            {
                _velocity = rigidbody.velocity;
                rigidbody.velocity = Vector3.zero;
            }

            public void Restore(Rigidbody rigidbody)
            {
                rigidbody.velocity = _velocity;
            }
        }

        private WorldBoundary _verticalBoundaryCollision;

        private readonly List<WorldBoundary> _horizontalBoundaryCollisions = new List<WorldBoundary>();

#region Physics
        [SerializeField]
        [ReadOnly]
        private Vector3 _bankForce;

        public Vector3 BankForce => _bankForce;

        public float Speed => Owner.State.IsIncapacitated ? 0.0f : (GameManager.Instance.IsPaused ? _pauseState.Velocity.magnitude : Rigidbody.velocity.magnitude);

        public float Altitude => Owner.transform.position.y;
#endregion

        [SerializeField]
        [ReadOnly]
        private PauseState _pauseState;

        private new Player Owner => (Player)base.Owner;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            InitRigidbody();

            GameManager.Instance.PauseEvent += PauseEventHandler;
        }

        private void OnDestroy()
        {
            if(GameManager.HasInstance) {
                GameManager.Instance.PauseEvent -= PauseEventHandler;
            }
        }

        private void Update()
        {
#if DEBUG
            CheckForDebug();
#endif
        }

        private void FixedUpdate()
        {
            if(!Owner.IsLocalPlayer) {
                return;
            }

            if(GameManager.Instance.IsPaused) {
                return;
            }

            Owner.Viewer.PlayerUI.PlayerUIPage.PlayerHUD.DebugVisualizer.SetState(Owner);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(!Owner.IsLocalPlayer) {
                return;
            }

            if(CheckWorldCollisionEnter(collision)) {
                return;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if(!Owner.IsLocalPlayer) {
                return;
            }

            if(CheckWorldCollisionExit(collision)) {
                return;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!Owner.IsLocalPlayer) {
                return;
            }

            if(CheckPlayerCollision(other)) {
                return;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + Rigidbody.angularVelocity);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + Rigidbody.velocity);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + _bankForce);
        }
#endregion

        private void InitRigidbody()
        {
            Rigidbody.isKinematic = false;
            Rigidbody.useGravity = true;
            Rigidbody.freezeRotation = true;
            Rigidbody.detectCollisions = true;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            // we run the follow cam in FixedUpdate() and interpolation interferes with that
            Rigidbody.interpolation = RigidbodyInterpolation.None;
        }

        public void Initialize(Player owner)
        {
            Rigidbody.mass = Owner.Bird.Type.Physics.Mass;
            Rigidbody.drag = Owner.Bird.Type.Physics.Drag;
            Rigidbody.angularDrag = Owner.Bird.Type.Physics.AngularDrag;
        }

        public void Redirect(Vector3 velocity)
        {
            Debug.Log($"Redirecting player {Owner.Id}: {velocity}");

            // unwind all of the rotations
            Owner.Bird.transform.localRotation = Quaternion.Euler(0.0f, Owner.Bird.transform.localEulerAngles.y, 0.0f);
            Rigidbody.transform.rotation = Quaternion.Euler(0.0f, Rigidbody.transform.eulerAngles.y, 0.0f);

            // stop moving
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;

            // move in an orderly fashion!
            Rigidbody.velocity = velocity;
        }

#region Input Handling
#if UNITY_EDITOR
        private void CheckForDebug()
        {
            if(Input.GetKey(KeyCode.B)) {
                Rigidbody.angularVelocity = Vector3.zero;
                Rigidbody.velocity = Vector3.zero;
            }
        }
#endif
#endregion

        public void RotateModel(Vector3 axes, float dt)
        {
            Quaternion rotation = Owner.Bird.transform.localRotation;
            Vector3 eulerAngles = Owner.Bird.transform.localEulerAngles;

            if(Owner.State.IsDead) {
                rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            } else if(Owner.State.IsStunned) {
                rotation = Quaternion.Euler(0.0f, eulerAngles.y, 0.0f);
            } else {
                Vector3 targetEuler = new Vector3();

                if(_horizontalBoundaryCollisions.Count < 1) {
                    targetEuler.z = axes.x * -GameManager.Instance.BirdData.MaxBankAngle;
                }

                if(null == _verticalBoundaryCollision) {
                    targetEuler.x = axes.y * -GameManager.Instance.BirdData.MaxAttackAngle;
                }

                Quaternion targetRotation = Quaternion.Euler(targetEuler);
                rotation = Quaternion.Lerp(rotation, targetRotation, PlayerManager.Instance.PlayerData.RotationAnimationSpeed * dt);
            }

            Owner.Bird.transform.localRotation = rotation;
        }

#region Movement
        public void Turn(Vector3 axes, float dt)
        {
            if(Owner.State.IsDead) {
                return;
            }

#if true
            float turnSpeed = Owner.Bird.Type.Physics.TurnSpeed * axes.x;
            Quaternion rotation = Quaternion.AngleAxis(turnSpeed * dt, Vector3.up);
            Rigidbody.MoveRotation(Rigidbody.rotation * rotation);
#else
            // TODO: this only works if Y rotatoin is unconstrained
            // it also breaks because the model rotates :(
            const float AngularThrust = 0.5f;
            _rigidbody.AddRelativeTorque(Vector3.up * AngularThrust * axes.x);
#endif

            // adding a force opposite our current x velocity should help stop us drifting
            Vector3 relativeVelocity = transform.InverseTransformDirection(Rigidbody.velocity);
            _bankForce = -relativeVelocity.x * Rigidbody.angularDrag * transform.right;
            Rigidbody.AddForce(_bankForce);
        }

        public void Move(Vector3 axes, float dt)
        {
            if(Owner.State.IsDead) {
                // just let gravity drop us (this feels slow tho)
            } else if(Owner.State.IsStunned) {
                // just don't fall
                Rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);
            } else {
                float attackAngle = axes.y * -GameManager.Instance.BirdData.MaxAttackAngle;
                Vector3 attackVector = Quaternion.AngleAxis(attackAngle, Vector3.right) * Vector3.forward;
                Rigidbody.AddRelativeForce(attackVector * Owner.Bird.Type.Physics.LinearThrust);

                if(Owner.State.IsBraking) {
                    Rigidbody.AddRelativeForce(Vector3.forward * -Owner.Bird.Type.Physics.BrakeThrust * Owner.State.BrakeAmount);
                }

                if(Owner.State.IsBoosting) {
                    Rigidbody.AddRelativeForce(Vector3.forward * Owner.Bird.Type.Physics.BoostThrust * Owner.State.BoostAmount);
                }

                // lift if we're not falling
                if(axes.y >= 0.0f) {
                    Rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);
                }
            }

            // cap our fall speed
            if(Rigidbody.velocity.y < -Owner.Bird.Type.Physics.TerminalVelocity) {
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, -Owner.Bird.Type.Physics.TerminalVelocity, Rigidbody.velocity.z);
            }
        }
#endregion

#region Collision Handlers
        private bool CheckWorldCollisionEnter(Collision collision)
        {
            WorldBoundary boundary = collision.gameObject.GetComponent<WorldBoundary>();
            if(null == boundary) {
                return false;
            }

            if(boundary.IsVertical) {
                _verticalBoundaryCollision = boundary;
            } else {
                _horizontalBoundaryCollisions.Add(boundary);
            }

            return true;
        }

        private bool CheckWorldCollisionExit(Collision collision)
        {
            WorldBoundary boundary = collision.gameObject.GetComponent<WorldBoundary>();
            if(null == boundary) {
                return false;
            }

            if(boundary.IsVertical) {
                _verticalBoundaryCollision = null;
            } else {
                _horizontalBoundaryCollisions.Remove(boundary);
            }

            return true;
        }

        private bool CheckPlayerCollision(Collider other)
        {
            Player player = other.GetComponentInParent<Player>();
            if(null == player) {
                return false;
            }

            // TODO: both players are going to get this so we should
            // probably do something here so that we don't double-collide them

            // TODO: hande this logic off to something else
            // maybe the Player or the PlayerState
            if(Owner.Bird.Type.IsPredator && player.Bird.Type.IsPrey) {
                player.State.PlayerKill(Owner, GetComponentInChildren<Collider>());
            } else if(Owner.Bird.Type.IsPrey && player.Bird.Type.IsPredator) {
                Owner.State.PlayerKill(player, other);
            } else {
                Owner.State.PlayerStun(player, other);
                player.State.PlayerStun(Owner, other);
            }

            return true;
        }
#endregion

#region Event Handlers
        private void PauseEventHandler(object sender, EventArgs args)
        {
            if(GameManager.Instance.IsPaused) {
                _pauseState.Save(Rigidbody);
            } else {
                _pauseState.Restore(Rigidbody);
            }
            Rigidbody.isKinematic = GameManager.Instance.IsPaused;
        }
#endregion
    }
}
