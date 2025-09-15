using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float movementSmoothing = 0.05f;
    private Vector2 movementDirection;
    private Vector2 currentSmoothedVelocity;
    private Rigidbody2D rb;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldownTime = 1f;
    private bool canUseDash = true;
    private bool isExecutingDash = false;
    private float currentDashTime = 0f;
    private float currentCooldownTime = 0f;
    
    [Header("Direction Settings")]
    private CharacterDirection currentDirection = CharacterDirection.Down;
    private CharacterDirection lastDirection = CharacterDirection.Down;
    
    [Header("Animation Settings")]
    [SerializeField] private float stateTransitionTime = 0.1f; // Time between state transitions
    [SerializeField] private float animationVelocityThreshold = 0.01f; // Minimum threshold for animation velocity
    private float animationVelocity = 0f;                       // Current animation velocity (for blend)
    private float targetAnimationVelocity = 0f;                   // Target velocity for smoothing
    private MovementState currentState = MovementState.Idle;  // Current character state
    private bool isWalking = false;                              // Indicator if character is walking
    private float directionBlendID = 0f;                         // Numeric direction ID for Blend Tree
    
    [Header("Attack Settings")]
    [SerializeField] private float punchDuration = 0.4f;           // Punch animation duration
    [SerializeField] private float punchCooldownTime = 0.2f;     // Wait time between punches
    private bool isExecutingPunch = false;                     // Indicator if executing a punch
    private bool canUsePunch = true;                            // Indicator if can use punch again
    private float currentPunchTime = 0f;                           // Current punch elapsed time
    private float currentPunchCooldownTime = 0f;                   // Current punch cooldown elapsed time
    
    // Enum to control movement states
    private enum MovementState
    {
        Idle,
        Walking,
        Running,
        Dash
    }

    // Enum to control character direction
    private enum CharacterDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    // Reference to animation component
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentSpeed = normalSpeed;
    }

    private void Update()
    {
        // Process player inputs
        ProcessInputs();
        
        // Manage dash cooldown
        ManageDash();
        
        // Manage punch duration and cooldown
        ManagePunch();
    }

    private void FixedUpdate()
    {
        // Apply movement
        if (!isExecutingDash)
        {
            Move();
        }
    }

    private void ProcessInputs()
    {
        // Capture horizontal and vertical inputs (WASD or arrows)
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        
        // Store movement direction
        movementDirection = new Vector2(moveHorizontal, moveVertical).normalized;
        
        // Check if Shift is pressed to run
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = normalSpeed;
        }
        
        // Execute dash when pressing Space and available
        if (Input.GetKeyDown(KeyCode.Space) && canUseDash && movementDirection.magnitude > 0)
        {
            StartDash();
        }
        
        // Execute punch when pressing left mouse button and available
        if (Input.GetMouseButtonDown(0) && canUsePunch && !isExecutingDash)
        {
            StartPunch();
        }
    }

    private void Move()
    {
        // Calculate smoothed movement
        Vector2 desiredVelocity = movementDirection * currentSpeed;
        currentSmoothedVelocity = Vector2.Lerp(currentSmoothedVelocity, desiredVelocity, movementSmoothing);
        
        // Apply movement to rigidbody
        rb.linearVelocity = currentSmoothedVelocity;
        
        // Update animations (if has an Animator)
        if (animator != null)
        {
            UpdateAnimations();
        }
    }

    private void StartDash()
    {
        isExecutingDash = true;
        canUseDash = false;
        currentDashTime = 0f;
        
        // Apply an instant force in movement direction
        rb.linearVelocity = movementDirection * dashForce;
        
        // Here you can add visual effects for the dash
        // For example: trail renderer, particles, etc.
    }

    private void ManageDash()
    {
        // Manage dash duration
        if (isExecutingDash)
        {
            currentDashTime += Time.deltaTime;
            
            if (currentDashTime >= dashDuration)
            {
                isExecutingDash = false;
                rb.linearVelocity = movementDirection * currentSpeed;
            }
        }
        
        // Manage dash cooldown
        if (!canUseDash)
        {
            currentCooldownTime += Time.deltaTime;
            
            if (currentCooldownTime >= dashCooldownTime)
            {
                canUseDash = true;
                currentCooldownTime = 0f;
            }
        }
    }

    private void UpdateAnimations()
    {
        // Check if character is moving
        bool isMoving = movementDirection.magnitude > 0;
        
        // Update character direction if moving
        if (isMoving)
        {
            UpdateCharacterDirection();
        }
        
        // Determine current movement state
        UpdateMovementState(isMoving);
        
        // Update animation velocity with smoothing
        UpdateAnimationVelocity();

        // Update numeric direction ID for Blend Tree
        UpdateDirectionBlendID();
        
        // Control animations through Animator and Blend Trees
        if (animator != null)
        {
            // Set basic parameters for Animator
            animator.SetBool("Movendo", isMoving);
            animator.SetBool("Correndo", currentSpeed == runSpeed && isMoving);
            animator.SetBool("Dash", isExecutingDash);
            animator.SetBool("isWalking", isWalking);
            
            // Set parameter for punch system
            animator.SetBool("isPunching", isExecutingPunch);
            
            // Set movement state for Animator
            animator.SetInteger("EstadoMovimento", (int)currentState);
            
            // Use current direction when moving, or last direction when stopped
            CharacterDirection directionToUse = isMoving ? currentDirection : lastDirection;
            animator.SetInteger("Direcao", (int)directionToUse);
            
            // Parameters for directional Blend Trees
            Vector2 directionVector = GetDirectionVector(isMoving);
            
            // Set direction parameters for Blend Trees
            animator.SetFloat("DirecaoX", directionVector.x);
            animator.SetFloat("DirecaoY", directionVector.y);
            
            // Set smoothed velocity for transitions between states
            animator.SetFloat("VelocidadeAnimacao", animationVelocity);
            
            // Set direction ID for Blend Tree (discrete values for each direction)
            animator.SetFloat("DirecaoID", directionBlendID);
            
            // Set real velocity for other animation parameters
            animator.SetFloat("Velocidade", movementDirection.magnitude * (currentSpeed / normalSpeed));
            
            // Set movement type (walking or running) - useful for multiple Blend Trees
            animator.SetFloat("TipoMovimento", currentSpeed == runSpeed ? 1f : 0f);
        }
    }
    
    private Vector2 GetDirectionVector(bool isMoving)
    {
        // If moving, use current movement direction
        if (isMoving)
        {
            return movementDirection;
        }
        // If not moving, convert last known direction to a vector
        else
        {
            Vector2 lastDirectionVector = Vector2.zero;
            switch (lastDirection)
            {
                case CharacterDirection.Up:
                    lastDirectionVector = new Vector2(0, 1);
                    break;
                case CharacterDirection.Down:
                    lastDirectionVector = new Vector2(0, -1);
                    break;
                case CharacterDirection.Right:
                    lastDirectionVector = new Vector2(1, 0);
                    break;
                case CharacterDirection.Left:
                    lastDirectionVector = new Vector2(-1, 0);
                    break;
            }
            return lastDirectionVector;
        }
    }
    
    private void UpdateMovementState(bool isMoving)
    {
        // Determine current state based on character conditions
        if (isExecutingDash)
        {
            currentState = MovementState.Dash;
            targetAnimationVelocity = 1.0f; // Maximum value for dash
        }
        else if (isMoving)
        {
            if (currentSpeed == runSpeed)
            {
                currentState = MovementState.Running;
                targetAnimationVelocity = 0.75f; // Value for running
            }
            else
            {
                currentState = MovementState.Walking;
                targetAnimationVelocity = 0.5f; // Value for walking
            }
        }
        else
        {
            currentState = MovementState.Idle;
            targetAnimationVelocity = 0.0f; // Value for idle
        }
    }
    
    private void UpdateAnimationVelocity()
    {
        // Smooth animation velocity transition
        animationVelocity = Mathf.Lerp(animationVelocity, targetAnimationVelocity, stateTransitionTime);
        
        // If value is very close to zero, set to exactly zero to avoid absurd values
        if (targetAnimationVelocity == 0f && animationVelocity < animationVelocityThreshold)
        {
            animationVelocity = 0f;
        }
        
        // Update isWalking state based on animation velocity
        isWalking = animationVelocity > animationVelocityThreshold && currentState == MovementState.Walking;
    }
    
    private void UpdateCharacterDirection()
    {
        // Determine predominant direction (up, down, left, right)
        float absX = Mathf.Abs(movementDirection.x);
        float absY = Mathf.Abs(movementDirection.y);
        
        if (movementDirection.magnitude > 0.1f)
        {
            // Save current direction as last direction before updating it
            lastDirection = currentDirection;
            
            // Check which component (x or y) is dominant to determine direction
            if (absX > absY)
            {
                // Horizontal movement is dominant
                if (movementDirection.x > 0)
                    currentDirection = CharacterDirection.Right;
                else
                    currentDirection = CharacterDirection.Left;
            }
            else
            {
                // Vertical movement is dominant
                if (movementDirection.y > 0)
                    currentDirection = CharacterDirection.Up;
                else
                    currentDirection = CharacterDirection.Down;
            }
        }
    }
    
    /// <summary>
    /// Convert character direction to numeric value for Blend Tree
    /// </summary>
    private void UpdateDirectionBlendID()
    {
        // Define distinct values for each main direction
        // This method uses specific integer values that will be easier to use in Blend Tree
        // North (Up): 0, East (Right): 1, South (Down): 2, West (Left): 3
        
        // Use current direction when moving, or last known direction when stopped
        CharacterDirection directionToUse = movementDirection.magnitude > 0.1f ? currentDirection : lastDirection;
        
        // Convert direction enum to specific numeric value
        switch (directionToUse)
        {
            case CharacterDirection.Up:
                directionBlendID = 0f;
                break;
            case CharacterDirection.Right:
                directionBlendID = 1f;
                break;
            case CharacterDirection.Down:
                directionBlendID = 2f;
                break;
            case CharacterDirection.Left:
                directionBlendID = 3f;
                break;
        }
    }
    
    /// <summary>
    /// Start punch animation and state
    /// </summary>
    private void StartPunch()
    {
        // Activate punch state
        isExecutingPunch = true;
        canUsePunch = false;
        currentPunchTime = 0f;
        
        // During punch, we can reduce character speed or stop it completely
        // depending on game type
        Vector2 reducedVelocity = movementDirection * (currentSpeed * 0.5f);
        rb.linearVelocity = reducedVelocity;
        
        // If you have sound or visual effects for punch, you can add them here
        // For example: Instantiate particle effect, play sound, etc.
    }
    
    /// <summary>
    /// Manage punch duration and cooldown
    /// </summary>
    private void ManagePunch()
    {
        // Manage punch duration
        if (isExecutingPunch)
        {
            currentPunchTime += Time.deltaTime;
            
            if (currentPunchTime >= punchDuration)
            {
                // End punch
                isExecutingPunch = false;
                
                // Restore normal speed if necessary
                if (!isExecutingDash)
                {
                    rb.linearVelocity = movementDirection * currentSpeed;
                }
            }
        }
        
        // Manage punch cooldown
        if (!canUsePunch)
        {
            currentPunchCooldownTime += Time.deltaTime;
            
            if (currentPunchCooldownTime >= punchCooldownTime)
            {
                canUsePunch = true;
                currentPunchCooldownTime = 0f;
            }
        }
    }
}