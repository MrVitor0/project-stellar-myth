using UnityEngine;
using CombatSystem; // Para acessar o sistema de stamina

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 5f;
    // runSpeed removido - apenas velocidade normal agora
    [SerializeField] private float currentSpeed;
    [SerializeField] private float movementSmoothing = 0.05f;
    private Vector2 movementDirection;
    private Vector2 currentSmoothedVelocity;
    private Rigidbody2D rb;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldownTime = 1f;
    [SerializeField] private float dashStaminaCost = 20f; // Stamina necessária para dar dash
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
        // Running removido - apenas Walking e Dash agora
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
    
    // Reference to combat system for stamina management
    private ICombatController combatController;
    private CombatAttributes combatAttributes;
    
    // Control variables
    private bool isPlayerLocked = false; // Bloqueia todos os movimentos quando true

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentSpeed = normalSpeed;
        
        // Conecta com o sistema de combate para gerenciar stamina
        combatController = GetComponent<ICombatController>();
        if (combatController != null)
        {
            combatAttributes = combatController.Attributes;
        }
        else
        {
            Debug.LogWarning("PlayerController2D: Não encontrou ICombatController! Dash não consumirá stamina.");
        }
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
        // Se o player estiver bloqueado, não processa inputs
        if (isPlayerLocked)
        {
            movementDirection = Vector2.zero;
            return;
        }
        
        // Capture horizontal and vertical inputs (WASD or arrows)
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        
        // Store movement direction
        movementDirection = new Vector2(moveHorizontal, moveVertical).normalized;
        
        // Velocidade sempre normal - sem corrida
        currentSpeed = normalSpeed;
        
        // Execute dash when pressing Space and available
        if (Input.GetKeyDown(KeyCode.Space) && CanUseDash() && movementDirection.magnitude > 0)
        {
            StartDash();
        }
        
        // Execute punch when pressing attack input and available  
        if (GetAttackInput() && canUsePunch && !isExecutingDash)
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

    /// <summary>
    /// Verifica se pode usar o dash (cooldown + stamina)
    /// </summary>
    private bool CanUseDash()
    {
        // Verifica cooldown
        if (!canUseDash) return false;
        
        // Verifica stamina
        if (combatAttributes != null)
        {
            return combatAttributes.CurrentStamina >= dashStaminaCost;
        }
        
        // Se não tem sistema de combate, permite dash
        return true;
    }

    private void StartDash()
    {
        // Consome stamina antes de executar o dash
        if (combatAttributes != null)
        {
            bool staminaConsumed = combatAttributes.ConsumeStamina(dashStaminaCost);
            if (!staminaConsumed)
            {
                Debug.Log("PlayerController2D: Stamina insuficiente para dash!");
                return; // Não executa dash se não conseguiu consumir stamina
            }
            
            Debug.Log($"PlayerController2D: Dash executado! Stamina restante: {combatAttributes.CurrentStamina:F1}");
        }
        
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
            // Set basic parameters for Animator (removido Correndo - apenas Walking/Dash)
            animator.SetBool("Movendo", isMoving);
            // animator.SetBool("Correndo", ...) REMOVIDO - sem corrida
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
            
            // Set movement type - sempre 0f agora (sem corrida)
            animator.SetFloat("TipoMovimento", 0f);
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
            // Sempre Walking agora - sem Running
            currentState = MovementState.Walking;
            targetAnimationVelocity = 0.5f; // Value for walking
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
    
    /// <summary>
    /// Verifica se algum input de ataque foi pressionado
    /// Compatível com o sistema de combate
    /// </summary>
    private bool GetAttackInput()
    {
        // Input principal configurado no Input Manager (Fire1 = mouse esquerdo ou ctrl)
        if (Input.GetButtonDown("Fire1"))
            return true;
        
        // Teclas alternativas (E, Enter)
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
            return true;
        
        return false;
    }
    
    // Métodos públicos para acesso às informações do dash
    
    /// <summary>
    /// Retorna se o player pode usar o dash (cooldown + stamina)
    /// </summary>
    public bool CanPlayerDash()
    {
        return CanUseDash();
    }
    
    /// <summary>
    /// Retorna o custo de stamina do dash
    /// </summary>
    public float GetDashStaminaCost()
    {
        return dashStaminaCost;
    }
    
    /// <summary>
    /// Retorna se está executando dash
    /// </summary>
    public bool IsCurrentlyDashing()
    {
        return isExecutingDash;
    }
    
    /// <summary>
    /// Retorna o tempo restante de cooldown do dash
    /// </summary>
    public float GetDashCooldownRemaining()
    {
        return canUseDash ? 0f : (dashCooldownTime - currentCooldownTime);
    }
    
    /// <summary>
    /// Retorna a stamina atual do player (se disponível)
    /// </summary>
    public float GetCurrentStamina()
    {
        return combatAttributes?.CurrentStamina ?? 0f;
    }
    
    /// <summary>
    /// Bloqueia todos os movimentos, ataques e dash do player
    /// </summary>
    public void LockPlayer()
    {
        isPlayerLocked = true;
        
        // Para o movimento imediatamente
        movementDirection = Vector2.zero;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // Para animações de movimento
        if (animator != null)
        {
            animator.SetFloat("Velocity", 0f);
        }
        
        Debug.Log("PlayerController2D: Player bloqueado - sem movimentos, ataques ou dash!");
    }
    
    /// <summary>
    /// Desbloqueia o player, permitindo movimentos novamente
    /// </summary>
    public void UnlockPlayer()
    {
        isPlayerLocked = false;
        Debug.Log("PlayerController2D: Player desbloqueado - movimentos liberados!");
    }
    
    /// <summary>
    /// Verifica se o player está bloqueado
    /// </summary>
    public bool IsPlayerLocked()
    {
        return isPlayerLocked;
    }
}