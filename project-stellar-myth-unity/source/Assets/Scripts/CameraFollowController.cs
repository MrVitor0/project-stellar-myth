using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target; // O transform do jogador a ser seguido
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); // Offset da câmera (Z negativo para ficar atrás)
    
    [Header("Movement Settings")]
    [SerializeField] private float followSpeed = 5f; // Velocidade de seguimento
    [SerializeField] private bool useSmoothDamp = true; // Usar suavização ou lerp
    [SerializeField] private float smoothTime = 0.3f; // Tempo de suavização para SmoothDamp
    
    [Header("Boundaries (Optional)")]
    [SerializeField] private bool useBoundaries = false;
    [SerializeField] private Vector2 minBounds = new Vector2(-10, -10);
    [SerializeField] private Vector2 maxBounds = new Vector2(10, 10);
    
    [Header("Advanced Boundaries")]
    [SerializeField] private bool useColliderBounds = false; // Usar bounds de um collider
    [SerializeField] private BoxCollider2D boundaryCollider; // Collider que define a área
    [SerializeField] private bool considerCameraSize = true; // Considerar o tamanho da câmera nos limites
    [SerializeField] private float boundaryBuffer = 0f; // Buffer adicional para os limites
    
    [Header("Look Ahead Settings")]
    [SerializeField] private bool useLookAhead = false; // Antecipação baseada na velocidade do jogador
    [SerializeField] private float lookAheadDistance = 2f;
    [SerializeField] private float lookAheadSmoothing = 0.5f;
    
    // Variáveis privadas
    private Vector3 velocity = Vector3.zero; // Para SmoothDamp
    private Vector3 currentLookAhead = Vector3.zero;
    private Rigidbody2D targetRigidbody;
    private Camera cameraComponent;
    private Vector2 calculatedMinBounds;
    private Vector2 calculatedMaxBounds;
    
    private void Awake()
    {
        // Obtém o componente da câmera
        cameraComponent = GetComponent<Camera>();
     
        
        // Se nenhum target foi definido, tenta encontrar o jogador automaticamente
        if (target == null)
        {
            PlayerController2D player = FindObjectOfType<PlayerController2D>();
            if (player != null)
            {
                target = player.transform;
                targetRigidbody = player.GetComponent<Rigidbody2D>();
            }
          
        }
        else
        {
            targetRigidbody = target.GetComponent<Rigidbody2D>();
        }
        
        // Calcula os boundaries considerando o tamanho da câmera
        UpdateBoundaries();
    }
    
    private void Start()
    {
        // Posiciona a câmera na posição inicial do target
        if (target != null)
        {
            Vector3 initialPosition = target.position + offset;
            if (useBoundaries)
            {
                initialPosition = ApplyBoundaries(initialPosition);
            }
            transform.position = initialPosition;
        }
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        FollowTarget();
    }
    
    private void FollowTarget()
    {
        // Calcula a posição desejada
        Vector3 desiredPosition = target.position + offset;
        
        // Adiciona look ahead se habilitado
        if (useLookAhead && targetRigidbody != null)
        {
            Vector3 targetLookAhead = (Vector3)targetRigidbody.linearVelocity.normalized * lookAheadDistance;
            currentLookAhead = Vector3.Lerp(currentLookAhead, targetLookAhead, lookAheadSmoothing * Time.deltaTime);
            desiredPosition += currentLookAhead;
        }
        
        // Aplica boundaries se habilitado
        if (useBoundaries)
        {
            desiredPosition = ApplyBoundaries(desiredPosition);
        }
        
        // Move a câmera para a posição desejada
        if (useSmoothDamp)
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
    }
    
    private void UpdateBoundaries()
    {
        if (!useBoundaries) return;
        
        if (useColliderBounds && boundaryCollider != null)
        {
            // Usa os bounds do collider
            Bounds colliderBounds = boundaryCollider.bounds;
            minBounds = new Vector2(colliderBounds.min.x, colliderBounds.min.y);
            maxBounds = new Vector2(colliderBounds.max.x, colliderBounds.max.y);
        }
        
        // Calcula os limites considerando o tamanho da câmera
        calculatedMinBounds = minBounds;
        calculatedMaxBounds = maxBounds;
        
        if (considerCameraSize && cameraComponent != null)
        {
            float cameraHalfWidth, cameraHalfHeight;
            
            if (cameraComponent.orthographic)
            {
                cameraHalfHeight = cameraComponent.orthographicSize;
                cameraHalfWidth = cameraHalfHeight * cameraComponent.aspect;
            }
            else
            {
                // Para câmera perspectiva, calcula com base na distância
                float distance = Mathf.Abs(offset.z);
                cameraHalfHeight = distance * Mathf.Tan(cameraComponent.fieldOfView * 0.5f * Mathf.Deg2Rad);
                cameraHalfWidth = cameraHalfHeight * cameraComponent.aspect;
            }
            
            // Ajusta os limites para que a câmera não mostre além das boundaries
            calculatedMinBounds.x += cameraHalfWidth + boundaryBuffer;
            calculatedMinBounds.y += cameraHalfHeight + boundaryBuffer;
            calculatedMaxBounds.x -= cameraHalfWidth + boundaryBuffer;
            calculatedMaxBounds.y -= cameraHalfHeight + boundaryBuffer;
            
            // Garante que os limites mínimos não sejam maiores que os máximos
            if (calculatedMinBounds.x >= calculatedMaxBounds.x)
            {
                float center = (minBounds.x + maxBounds.x) * 0.5f;
                calculatedMinBounds.x = center - 0.1f;
                calculatedMaxBounds.x = center + 0.1f;
            }
            
            if (calculatedMinBounds.y >= calculatedMaxBounds.y)
            {
                float center = (minBounds.y + maxBounds.y) * 0.5f;
                calculatedMinBounds.y = center - 0.1f;
                calculatedMaxBounds.y = center + 0.1f;
            }
        }
    }
    
    private Vector3 ApplyBoundaries(Vector3 position)
    {
        // Atualiza os boundaries se necessário
        if (useColliderBounds || considerCameraSize)
        {
            UpdateBoundaries();
        }
        
        position.x = Mathf.Clamp(position.x, calculatedMinBounds.x, calculatedMaxBounds.x);
        position.y = Mathf.Clamp(position.y, calculatedMinBounds.y, calculatedMaxBounds.y);
        return position;
    }
    
    // Método público para definir o target dinamicamente
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetRigidbody = newTarget != null ? newTarget.GetComponent<Rigidbody2D>() : null;
    }
    
    // Método público para definir o offset
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
    
    // Método público para definir boundaries dinamicamente
    public void SetBoundaries(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBoundaries = true;
        useColliderBounds = false;
        UpdateBoundaries();
    }
    
    // Método para definir boundaries usando um collider
    public void SetBoundariesFromCollider(BoxCollider2D collider)
    {
        boundaryCollider = collider;
        useColliderBounds = true;
        useBoundaries = true;
        UpdateBoundaries();
    }
    
    // Método para definir boundaries automaticamente baseado em objetos da cena
    public void SetBoundariesFromScenario(string[] layerNames)
    {
        Bounds sceneBounds = new Bounds();
        bool boundsInitialized = false;
        
        foreach (string layerName in layerNames)
        {
            int layerIndex = LayerMask.NameToLayer(layerName);
            if (layerIndex == -1) continue;
            
            GameObject[] objects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in objects)
            {
                if (obj.layer == layerIndex)
                {
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        if (!boundsInitialized)
                        {
                            sceneBounds = renderer.bounds;
                            boundsInitialized = true;
                        }
                        else
                        {
                            sceneBounds.Encapsulate(renderer.bounds);
                        }
                    }
                }
            }
        }
        
        if (boundsInitialized)
        {
            minBounds = new Vector2(sceneBounds.min.x, sceneBounds.min.y);
            maxBounds = new Vector2(sceneBounds.max.x, sceneBounds.max.y);
            useBoundaries = true;
            useColliderBounds = false;
            UpdateBoundaries();
            
        }
    }
    
    // Método para desabilitar boundaries
    public void DisableBoundaries()
    {
        useBoundaries = false;
    }
    
    // Método para habilitar boundaries
    public void EnableBoundaries()
    {
        useBoundaries = true;
        UpdateBoundaries();
    }
    
    // Método para definir se deve considerar o tamanho da câmera
    public void SetConsiderCameraSize(bool consider)
    {
        considerCameraSize = consider;
        UpdateBoundaries();
    }
    
    // Método para definir buffer dos boundaries
    public void SetBoundaryBuffer(float buffer)
    {
        boundaryBuffer = buffer;
        UpdateBoundaries();
    }
    
    // Método para ajustar a velocidade de seguimento
    public void SetFollowSpeed(float speed)
    {
        followSpeed = speed;
    }
    
    // Método para ajustar o smooth time
    public void SetSmoothTime(float time)
    {
        smoothTime = time;
    }
    
    // Propriedades públicas para acessar os boundaries
    public Vector2 MinBounds => minBounds;
    public Vector2 MaxBounds => maxBounds;
    public Vector2 CalculatedMinBounds => calculatedMinBounds;
    public Vector2 CalculatedMaxBounds => calculatedMaxBounds;
    public bool IsBoundariesEnabled => useBoundaries;
    
    // Debug para visualizar os boundaries no Scene View
    private void OnDrawGizmosSelected()
    {
        if (useBoundaries)
        {
            // Desenha os boundaries originais em amarelo
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, transform.position.z);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0);
            Gizmos.DrawWireCube(center, size);
            
            // Desenha os boundaries calculados (considerando tamanho da câmera) em laranja
            if (considerCameraSize && cameraComponent != null)
            {
                Gizmos.color = Color.red;
                Vector3 calcCenter = new Vector3((calculatedMinBounds.x + calculatedMaxBounds.x) / 2, 
                                                (calculatedMinBounds.y + calculatedMaxBounds.y) / 2, 
                                                transform.position.z);
                Vector3 calcSize = new Vector3(calculatedMaxBounds.x - calculatedMinBounds.x, 
                                             calculatedMaxBounds.y - calculatedMinBounds.y, 0);
                Gizmos.DrawWireCube(calcCenter, calcSize);
                
                // Desenha a área de visão da câmera
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                float cameraHalfWidth, cameraHalfHeight;
                
                if (cameraComponent.orthographic)
                {
                    cameraHalfHeight = cameraComponent.orthographicSize;
                    cameraHalfWidth = cameraHalfHeight * cameraComponent.aspect;
                }
                else
                {
                    float distance = Mathf.Abs(offset.z);
                    cameraHalfHeight = distance * Mathf.Tan(cameraComponent.fieldOfView * 0.5f * Mathf.Deg2Rad);
                    cameraHalfWidth = cameraHalfHeight * cameraComponent.aspect;
                }
                
                Vector3 cameraSize = new Vector3(cameraHalfWidth * 2, cameraHalfHeight * 2, 0);
                Gizmos.DrawCube(transform.position, cameraSize);
            }
        }
        
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
            
            if (useLookAhead && targetRigidbody != null)
            {
                Gizmos.color = Color.blue;
                Vector3 lookAheadPos = target.position + currentLookAhead;
                Gizmos.DrawLine(target.position, lookAheadPos);
                Gizmos.DrawWireSphere(lookAheadPos, 0.5f);
            }
        }
        
        // Desenha o collider de boundary se estiver sendo usado
        if (useColliderBounds && boundaryCollider != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(boundaryCollider.bounds.center, boundaryCollider.bounds.size);
        }
    }
}
