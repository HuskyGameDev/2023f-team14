using System;
using System.Collections;
using System.Collections.Generic;
using PRN;

//using System.Numerics;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : Unity.Netcode.NetworkBehaviour
{

    [SerializeField]
    private PlayerPredictionProcessor processor;
    [SerializeField]
    private PlayerPredictionInputProvider inputProvider;
    [SerializeField]
    private PlayerPredictionStateConsistencyChecker consistencyChecker;


    private CharacterController controller;
    private Ticker ticker;
    private NetworkHandler<PlayerMovementInput, PlayerMovementState> networkHandler;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ticker = new(TimeSpan.FromSeconds(1 / (float)NetworkManager.Singleton.NetworkTickSystem.TickRate));
        NetworkRole role;

        if (IsServer)
            role = IsOwner ? NetworkRole.HOST : NetworkRole.SERVER;
        else
            role = IsOwner ? NetworkRole.OWNER : NetworkRole.GUEST;

        networkHandler = new(
            role: role,
            ticker: ticker,
            processor: processor,
            inputProvider: inputProvider,
            consistencyChecker: consistencyChecker
        );

        networkHandler.onSendStateToClient += SendStateClientRpc;
        networkHandler.onSendInputToServer += SendInputServerRpc;
    }

    private void FixedUpdate()
    {
        ticker.OnTimePassed(TimeSpan.FromSeconds(Time.fixedDeltaTime));
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        networkHandler.Dispose();
    }

    [ServerRpc]
    private void SendInputServerRpc(PlayerMovementInput input)
    {
        networkHandler.OnOwnerInputReceived(input);
    }

    [ClientRpc]
    public void SendStateClientRpc(PlayerMovementState state)
    {
        networkHandler.OnServerStateReceived(state);
    }

    //SERVER ONLY
    public void SetPosition(Vector3 pos)
    {
        if (!IsServer) return;
        controller.enabled = false;
        controller.transform.position = pos;
        controller.enabled = true;
    }

    /**
        private const uint predictionLabelLimit = 100;

        [Header("Movement Settings")]
        public float movementSpeed;
        public float jumpForce;
        public float jumpCooldown;
        public float airMultiplier; // Increases move speed in air
        public float groundDrag; // Slows character when grounded

        [Header("Ground Check Settings")]
        public Transform groundSphere;
        public LayerMask groundLayer;
        public float groundCheckRadius;

        [Header("Miscellaneous")]
        public Camera playerCamera;
        public MouseLook mouseLook;

        private bool isGrounded;
        private bool readyToJump;
        private float speedCoefficient = 10; // Makes character movement more snappy
        private Transform playerOrientation;
        private Vector2 lateralMovementInput;
        private MovementStateRequest? pendingReq;
        private float drag;
        private Vector3 velocity = Vector3.zero;
        private uint labelNumber;
        private PlayerControls playerControls;
        private readonly Queue<(MovementStateRequest, MovementStatePayload)> predictions = new();
        private void Awake()
        {
            InitializeComponents();
            InitializeMouseLook();
            labelNumber = 0;
        }

        public override void OnNetworkSpawn()
        {
            SubscribeToInputEvents();
            if (IsServer)
            {
                transform.position = new Vector3(0, 15, 0);
                NetworkManager.Singleton.NetworkTickSystem.Tick += ServerTickUpdate;
            }

            Time.fixedDeltaTime = 1f / NetworkManager.Singleton.NetworkTickSystem.TickRate;
            base.OnNetworkSpawn();
        }

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        private void Update()
        {
            DetectGround();
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            Gravity();

            if (lateralMovementInput != Vector2.zero)
                OwnerProcessMove();
        }

        private void InitializeComponents()
        {
            //rb = GetComponent<Rigidbody>();
            //rb.freezeRotation = true;
            playerControls = new PlayerControls();
            playerOrientation = transform.Find("PlayerModel");
            playerCamera = transform.GetComponentInChildren<Camera>();
        }

        private void InitializeMouseLook()
        {
            if (mouseLook)
            {
                mouseLook.Initialize(playerControls);
                return;
            }
            Debug.LogError("No camera bound to player!");
        }

        private void SubscribeToInputEvents()
        {
            playerControls.std.Move.performed += ctx => lateralMovementInput = ctx.ReadValue<Vector2>();
            playerControls.std.Move.canceled += ctx => lateralMovementInput = Vector2.zero;
            playerControls.std.Jump.performed += ctx => Jump();
        }

        private void DetectGround()
        {
            isGrounded = Physics.CheckSphere(groundSphere.position, groundCheckRadius, groundLayer);
        }

        private void ApplyDrag()
        {
            if (isGrounded)
            {
                drag = groundDrag;
            }
            else
            {
                drag = 0;
            }
        }
        private void CalculateVelocity(Vector3 prev, Vector3 curr)
        {
            velocity = curr - prev;
        }
        private void SpeedControl()
        {
            // Retrieves current velocity

            Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);

            // Produces coefficient for movement speed if airborne
            float speedMultiplier = 1;
            if (!isGrounded)
            {
                speedMultiplier = airMultiplier;
            }

            // Limit velocity
            if (flatVelocity.magnitude > movementSpeed)
            {
                Vector3 limitedVelocity = movementSpeed * speedMultiplier * flatVelocity.normalized;
                velocity = new Vector3(limitedVelocity.x, velocity.y, limitedVelocity.z);
            }
        }

        // OWNER ONLY
        private void OwnerProcessMove()
        {
            if (!IsOwner) return;

            labelNumber++;
            if (labelNumber > predictionLabelLimit)
            {
                labelNumber = 0;
            }
            var req = new MovementStateRequest(labelNumber, new Vector3(lateralMovementInput.x, 0f, lateralMovementInput.y).normalized);

            //Request movement from server
            HandleMovementServerRpc(req);

            //Predict our movement
            Move(req.input);

            //Save our predictions
            predictions.Enqueue((req, new MovementStatePayload(labelNumber, transform.position)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The end position of the player.</returns>
        private void Move(Vector3? input)
        {
            ApplyDrag();

            //Move the player
            Vector3 temp = transform.position;
            if (input != null)
            {
                Vector3 calculatedMoveVector = movementSpeed * speedCoefficient * Time.fixedDeltaTime * playerOrientation.TransformDirection((Vector3)input);
                transform.position += calculatedMoveVector;
            }
            CalculateVelocity(temp, transform.position);
            SpeedControl();
        }

        [ServerRpc]
        private void HandleMovementServerRpc(MovementStateRequest req, ServerRpcParams serverRpcParams = default)
        {
            if (pendingReq != null) return;
            pendingReq = req;
        }

        //SERVER ONLY -- Called every Tick
        private void ServerTickUpdate()
        {
            ServerProcessMove();
        }

        //SERVER ONLY
        private void ServerProcessMove()
        {
            if (!IsServer) return;
            Gravity();

            if (pendingReq == null) return;

            Move((Vector3)pendingReq?.input);
            RespondClientRpc(new MovementStatePayload((uint)pendingReq?.label, transform.position));
            pendingReq = null;
        }

        private void Gravity()
        {
            //TODO: Implement
        }

        [ClientRpc]
        private void RespondClientRpc(MovementStatePayload payload)
        {
            if (!IsOwner)
            {
                transform.position = payload.position;
                return;
            }

            if (predictions.Count == 0)
            {
                Debug.LogError("Prediction queue empty upon response from server!");
                return;
            }

            //Check our predictions, revert if necessary
            (MovementStateRequest, MovementStatePayload) prediction;
            do
            {
                prediction = predictions.Dequeue();
            } while (payload.label != prediction.Item2.label && predictions.Count > 0);

            float res = FindError(prediction.Item2.position, payload.position);
            Debug.Log(prediction.Item1.label + " " + payload.label);
            Debug.Log(prediction.Item2.position + " " + payload.position);
            var pos1 = prediction.Item2.position;
            var pos2 = payload.position;
            if (res < 0.01f)
            {
                //Our prediction was correct! Keep everything as is for now.
                return;
            }

            Reconcile(payload.position);
        }

        private void Reconcile(Vector3 position)
        {
            if (!IsOwner) return;
            Debug.Log("Reconciling prediction!");

            this.transform.position = position;
            int count = predictions.Count;
            (MovementStateRequest, MovementStatePayload) prediction;
            for (int i = 0; i < count; i++)
            {
                prediction = predictions.Dequeue();
                Move(prediction.Item1.input);
                prediction.Item2.position = transform.position;
                predictions.Enqueue(prediction);
            }
        }

        private float FindError(Vector3 a, Vector3 b)
        {
            return (float)(Math.Pow((a.x - b.x), 2) + Math.Pow((a.y - b.y), 2) + Math.Pow((a.z - b.z), 2));
        }

        private void Jump()
        {
            if (isGrounded && readyToJump)
            {
                // Zero out y velocity for consistent jumps
                //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                // Add force on y-axis
                // rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

                readyToJump = false;
            }

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        private void ResetJump()
        {
            readyToJump = true;
        }*/
}


