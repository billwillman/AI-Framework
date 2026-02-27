using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EasyCharacterMovement;
using TreeDesigner;

public interface ICharacterDerivative
{
    public PlatformCharacter Character { get; }
}

public class PlatformCharacter : Character, IAbilityRunnerOwner
{
    [Space(20)]
    public float RotationRate;
    public float HorizontalInputLerpSpeed;
    public float ForwardSpeedLerpSpeed;

    public Ability[] Abilities;
    public Action<InputAction.CallbackContext> OnInput;

    public int Direction { get; private set; }
    public int VelocityDirection { get; private set; }
    public Vector2 MovementInput { get; private set; }
    public float ForwardSpeed { get; private set; }
    public float VerticalSpeed { get; private set; }
    public float GroundDistance { get; private set; }
    public bool Rotating { get; private set; }
    public PlatformTimelinePlayer TimelinePlayer { get; private set; }
    public AbilityRunner AbilityRunner { get; set; }

    protected override void Start()
    {
        base.Start();

        Direction = 1;
        TimelinePlayer = GetComponentInChildren<PlatformTimelinePlayer>();
        TimelinePlayer.PlatformCharacter = this;

        AbilityRunner = new AbilityRunner();
        AbilityRunner.Init(this);

        for (int i = 0; i < Abilities.Length; i++)
        {
            Abilities[i] = Abilities[i].Clone();
            AbilityRunner.AddAbility(Abilities[i]);
        }

        EnableGravity(true);
    }
    protected override void Update()
    {
        base.Update();

        MovementInput = GetMovementInput();
        TimelinePlayer.SetFloat("HorizontalInput", MovementInput.x);
        TimelinePlayer.SetFloat("HorizontalInputAbs", Mathf.Lerp(TimelinePlayer.GetFloat("HorizontalInputAbs"), Mathf.Abs(MovementInput.x), HorizontalInputLerpSpeed));

        if (Rotating)
        {
            if (Direction > 0)
            {
                characterMovement.RotateTowards(new Vector3(1, 0, -0.01f), RotationRate * Time.deltaTime, false);
            }
            else
            {
                characterMovement.RotateTowards(new Vector3(-1, 0, 0.01f), RotationRate * Time.deltaTime, false);
            }
            if (Vector3.Angle(transform.forward, Direction * Vector3.right) < 1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(Direction * Vector3.right, transform.up);
                characterMovement.rotation = targetRotation;
                Rotating = false;
            }
        }
        TimelinePlayer.SetBool("Rotating", Rotating);

        ForwardSpeed = characterMovement.forwardSpeed;
        TimelinePlayer.SetFloat("ForwardSpeed", Mathf.Lerp(TimelinePlayer.GetFloat("ForwardSpeed"), ForwardSpeed, ForwardSpeedLerpSpeed));

        VerticalSpeed = characterMovement.velocity.y;
        TimelinePlayer.SetFloat("VerticalSpeed", VerticalSpeed);

        if (characterMovement.velocity.x != 0)
            VelocityDirection = characterMovement.velocity.x > 0 ? 1 : -1;

        TimelinePlayer.SetBool("Grounded", IsGrounded());
        if (Physics.SphereCast(characterMovement.position + (0.25f + 0.1f) * transform.up, 0.25f, -transform.up, out RaycastHit hitResult, 1000, characterMovement.collisionLayers, QueryTriggerInteraction.Ignore))
        {
            GroundDistance = hitResult.distance - 0.1f;
            TimelinePlayer.SetFloat("GroundDistance", GroundDistance);
        }

        AbilityRunner.Update(Time.deltaTime);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateExternalForce();
    }

    public void RotateTo(int direction)
    {
        Direction = direction;
        Rotating = true;
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();
        characterMovement.colliderFilterCallback += ColliderFilterCallback;
        characterMovement.modifyDeltaPositionCallback += ModifyDeltaPositionCallback;
    }
    protected override void Walking(Vector3 desiredVelocity)
    {
        // If using root motion output animation velocity

        // Calculate new velocity
        float actualFriction = useSeparateBrakingFriction ? brakingFriction : groundFriction;
        characterMovement.velocity = CalcVelocity(characterMovement.velocity, desiredVelocity, actualFriction);

        // Apply downwards force

        if (applyStandingDownwardForce)
            ApplyDownwardsForce();
    }
    protected override Vector3 CalcDesiredVelocity()
    {
        // Current movement direction

        Vector3 movementDirection = GetMovementDirection();

        // The desired velocity from animation (if using root motion) or from input movement vector

        Vector3 desiredVelocity = useRootMotion && rootMotionController
            ? Vector3.Lerp(movementDirection * GetMaxSpeed(), rootMotionController.animRootMotionVelocity, RootMotionWeight)
            : movementDirection * GetMaxSpeed();

        // Return desired velocity (constrained to constraint plane if any)

        return characterMovement.ConstrainVectorToPlane(desiredVelocity);
    }

    #region Input
    public Dictionary<string, InputAction> InputActionMap { get; private set; } = new Dictionary<string, InputAction>();
    public void BindInput(string name, InputPhase inputPhase, Action<InputAction.CallbackContext> callback)
    {
        if (InputActionMap.TryGetValue(name, out InputAction inputAction))
        {
            switch (inputPhase)
            {
                case InputPhase.Started:
                    inputAction.started += callback;
                    break;
                case InputPhase.Performed:
                    inputAction.performed += callback;
                    break;
                case InputPhase.Canceled:
                    inputAction.canceled += callback;
                    break;
                default:
                    break;
            }
        }
    }
    public void UnbindInput(string name, InputPhase inputPhase, Action<InputAction.CallbackContext> callback)
    {
        if (InputActionMap.TryGetValue(name, out InputAction inputAction))
        {
            switch (inputPhase)
            {
                case InputPhase.Started:
                    inputAction.started -= callback;
                    break;
                case InputPhase.Performed:
                    inputAction.performed -= callback;
                    break;
                case InputPhase.Canceled:
                    inputAction.canceled -= callback;
                    break;
                default:
                    break;
            }
        }
    }
    public bool GetInputPhase(string name, InputPhase inputPhase)
    {
        if(InputActionMap.TryGetValue(name,out InputAction inputAction))
        {
            return inputAction.phase.ToString() == inputPhase.ToString();
        }
        else
        {
            return false;
        }
    }
    protected override void InitPlayerInput()
    {
        base.InitPlayerInput();

        if (inputActions == null)
            return;
        if (movementInputAction != null)
            InputActionMap.Add("Movement", movementInputAction);
        if (jumpInputAction != null)
            InputActionMap.Add("Jump", jumpInputAction);

        InputAction dodgeInputAction = inputActions.FindAction("Dodge");
        if (dodgeInputAction != null)
        {
            dodgeInputAction.Enable();
            InputActionMap.Add("Dodge", dodgeInputAction);
        }

        InputAction attackInputAction = inputActions.FindAction("Attack");
        if (attackInputAction != null)
        {
            attackInputAction.Enable();
            InputActionMap.Add("Attack", attackInputAction);
        }

        InputAction skillInputAction = inputActions.FindAction("Skill");
        if (skillInputAction != null)
        {
            skillInputAction.Enable();
            InputActionMap.Add("Skill", skillInputAction);
        }
    }
    protected override void OnJump(InputAction.CallbackContext context) { }
    #endregion

    #region ExternalForce
    [Range(0, 1)]
    public float ExternalForceDamp;

    Vector3 m_ExternalForce;
    public void UpdateExternalForce()
    {
        characterMovement.AddForce(m_ExternalForce);
        m_ExternalForce = (1 - ExternalForceDamp) * m_ExternalForce;
    }
    public void AddExternalForce(Vector3 deltaForce)
    {
        if (deltaForce.y > 0)
            characterMovement.PauseGroundConstraint();
        m_ExternalForce += deltaForce;
    }
    public void ClearExternalForce()
    {
        m_ExternalForce = Vector3.zero;
    }
    #endregion

    #region StopOnCharacter
    public bool StopOnCharacter;
    public LayerMask UnitLayer;
    private bool ColliderFilterCallback(Collider collider)
    {
        // If collided collider is a character (e.g. using CharacterMovement component)
        // ignore collisions with it (e.g. filter it)

        if (collider.TryGetComponent(out CharacterMovement _))
            return true;

        // Return false to allow collisions (e.g. not filter it)

        return false;
    }
    private void ModifyDeltaPositionCallback(ref Vector3 deltaPosition)
    {
        if (!StopOnCharacter) return;
        

        var result = CapsuleCastSelf(transform.position, deltaPosition.normalized, deltaPosition.magnitude, UnitLayer);
        foreach (var c in result)
        {
            if (c.collider.TryGetComponent(out CharacterMovement characterMovement) && characterMovement != this.characterMovement)
            {
                float distance = Vector3.Distance(characterMovement.position, transform.position) - characterMovement.radius - this.characterMovement.radius;
                if (distance < 0/* && MovementInput.x == 0*/)
                {
                    //deltaPosition = Vector3.zero;
                    deltaPosition = new Vector3(0, deltaPosition.y, deltaPosition.z);
                }
                else
                {
                    float targetDistance = deltaPosition.magnitude - distance;
                    deltaPosition = Vector3.ClampMagnitude(deltaPosition, targetDistance);
                }
                break;
            }
        }
    }
    public RaycastHit[] CapsuleCastSelf(Vector3 from, Vector3 direction, float maxDistance, int layerMask)
    {
        Vector3 point1 = from + transform.up * characterMovement.radius;
        Vector3 point2 = from + transform.up * (characterMovement.height - characterMovement.radius);

        return Physics.CapsuleCastAll(point1, point2, characterMovement.radius, direction, maxDistance, layerMask);
    }
    #endregion

    #region AccelerationControl
    float m_AccelerationConrol = 1;
    public float AccelerationConrol => Mathf.Clamp01(m_AccelerationConrol);
    public void AddAccelerationConrol(float deltaValue)
    {
        m_AccelerationConrol += deltaValue;
    }
    public override float GetMaxAcceleration()
    {
        return base.GetMaxAcceleration() * AccelerationConrol;
    }
    #endregion

    #region RootMotion
    float m_RootMotionWeight;
    public float RootMotionWeight
    {
        get => m_RootMotionWeight;
        set
        {
            m_RootMotionWeight = Mathf.Max(0, value);
        }
    }

    int m_EnableRootmotionCount;
    public void EnableRootmotion(bool enable)
    {
        if (enable)
        {
            m_EnableRootmotionCount++;
        }
        else
        {
            m_EnableRootmotionCount--;
        }
        useRootMotion = m_EnableRootmotionCount > 0;
    }
    #endregion

    #region Gravity
    int m_EnableGravityCount;

    public override void EnableGravity(bool enable)
    {
        if (enable)
        {
            m_EnableGravityCount++;
        }
        else
        {
            m_EnableGravityCount--;
        }
        _applyGravity = m_EnableGravityCount > 0;
    }
    #endregion
}