using UnityEngine;

[System.Serializable]
public class CameraBoundarySetup : MonoBehaviour
{
    [Header("Boundary Configuration")]
    [SerializeField] private CameraFollowController cameraController;
    [SerializeField] private BoundaryType boundaryType = BoundaryType.Manual;
    
    [Header("Manual Boundaries")]
    [SerializeField] private Vector2 manualMinBounds = new Vector2(-20, -20);
    [SerializeField] private Vector2 manualMaxBounds = new Vector2(20, 20);
    
    [Header("Collider Boundaries")]
    [SerializeField] private BoxCollider2D boundaryCollider;
    
    [Header("Automatic Boundaries")]
    [SerializeField] private string[] scenarioLayers = { "Ground", "Walls", "Scenario" };
    [SerializeField] private float expansionMargin = 2f; // Margem extra para os boundaries automáticos
    
    [Header("Visual Settings")]
    [SerializeField] private bool showBoundariesInGame = false;
    [SerializeField] private Color boundaryColor = Color.red;
    [SerializeField] private LineRenderer boundaryRenderer;
    
    public enum BoundaryType
    {
        Manual,         // Definir manualmente no inspector
        Collider,       // Usar um BoxCollider2D
        Automatic,      // Calcular automaticamente baseado nos objetos da cena
        Disabled        // Sem boundaries
    }
    
    private void Awake()
    {
        // Encontra o CameraFollowController se não foi definido
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<CameraFollowController>();
        }
    }
    
    private void Start()
    {
        SetupBoundaries();
        
        if (showBoundariesInGame)
        {
            CreateBoundaryVisual();
        }
    }
    
    public void SetupBoundaries()
    {
        if (cameraController == null)
        {
            Debug.LogWarning("CameraBoundarySetup: CameraFollowController não encontrado!");
            return;
        }
        
        switch (boundaryType)
        {
            case BoundaryType.Manual:
                SetupManualBoundaries();
                break;
                
            case BoundaryType.Collider:
                SetupColliderBoundaries();
                break;
                
            case BoundaryType.Automatic:
                SetupAutomaticBoundaries();
                break;
                
            case BoundaryType.Disabled:
                cameraController.DisableBoundaries();
                Debug.Log("CameraBoundarySetup: Boundaries desabilitados");
                break;
        }
    }
    
    private void SetupManualBoundaries()
    {
        cameraController.SetBoundaries(manualMinBounds, manualMaxBounds);
        Debug.Log($"CameraBoundarySetup: Boundaries manuais definidos - Min: {manualMinBounds}, Max: {manualMaxBounds}");
    }
    
    private void SetupColliderBoundaries()
    {
        if (boundaryCollider != null)
        {
            cameraController.SetBoundariesFromCollider(boundaryCollider);
            Debug.Log("CameraBoundarySetup: Boundaries definidos a partir do collider");
        }
        else
        {
            Debug.LogWarning("CameraBoundarySetup: BoxCollider2D não definido para boundaries por collider");
            SetupManualBoundaries(); // Fallback para manual
        }
    }
    
    private void SetupAutomaticBoundaries()
    {
        cameraController.SetBoundariesFromScenario(scenarioLayers);
        
        // Adiciona margem extra se especificada
        if (expansionMargin > 0)
        {
            // Pega os boundaries atuais e expande
            Vector2 currentMin = cameraController.MinBounds;
            Vector2 currentMax = cameraController.MaxBounds;
            
            currentMin -= Vector2.one * expansionMargin;
            currentMax += Vector2.one * expansionMargin;
            
            cameraController.SetBoundaries(currentMin, currentMax);
        }
    }
    
    private void CreateBoundaryVisual()
    {
        if (boundaryRenderer == null)
        {
            // Cria um GameObject para o LineRenderer
            GameObject boundaryObject = new GameObject("Camera Boundaries Visualizer");
            boundaryObject.transform.SetParent(transform);
            boundaryRenderer = boundaryObject.AddComponent<LineRenderer>();
        }
        
        // Configura o LineRenderer
        boundaryRenderer.material = new Material(Shader.Find("Sprites/Default"));
        boundaryRenderer.startColor = boundaryColor;
        boundaryRenderer.endColor = boundaryColor;
        boundaryRenderer.startWidth = 0.1f;
        boundaryRenderer.endWidth = 0.1f;
        boundaryRenderer.positionCount = 5; // 4 cantos + volta para o primeiro
        boundaryRenderer.useWorldSpace = true;
        boundaryRenderer.sortingOrder = 10;
        
        UpdateBoundaryVisual();
    }
    
    private void UpdateBoundaryVisual()
    {
        if (boundaryRenderer == null || cameraController == null) return;
        
        Vector2 min = cameraController.MinBounds;
        Vector2 max = cameraController.MaxBounds;
        
        // Define os pontos do retângulo
        Vector3[] points = new Vector3[5]
        {
            new Vector3(min.x, min.y, 0), // Bottom-left
            new Vector3(max.x, min.y, 0), // Bottom-right  
            new Vector3(max.x, max.y, 0), // Top-right
            new Vector3(min.x, max.y, 0), // Top-left
            new Vector3(min.x, min.y, 0)  // Back to start
        };
        
        boundaryRenderer.SetPositions(points);
    }
    
    // Métodos públicos para uso em runtime
    public void SetBoundaryType(BoundaryType newType)
    {
        boundaryType = newType;
        SetupBoundaries();
        
        if (showBoundariesInGame && boundaryRenderer != null)
        {
            UpdateBoundaryVisual();
        }
    }
    
    public void SetManualBounds(Vector2 min, Vector2 max)
    {
        manualMinBounds = min;
        manualMaxBounds = max;
        
        if (boundaryType == BoundaryType.Manual)
        {
            SetupBoundaries();
        }
    }
    
    public void SetExpansionMargin(float margin)
    {
        expansionMargin = margin;
        
        if (boundaryType == BoundaryType.Automatic)
        {
            SetupBoundaries();
        }
    }
    
    public void ToggleBoundaryVisual(bool show)
    {
        showBoundariesInGame = show;
        
        if (show && boundaryRenderer == null)
        {
            CreateBoundaryVisual();
        }
        else if (boundaryRenderer != null)
        {
            boundaryRenderer.gameObject.SetActive(show);
        }
    }
    
    // Método para recalcular boundaries (útil quando o cenário muda)
    public void RecalculateBoundaries()
    {
        SetupBoundaries();
        
        if (showBoundariesInGame && boundaryRenderer != null)
        {
            UpdateBoundaryVisual();
        }
    }
    
    private void OnValidate()
    {
        // Atualiza boundaries quando valores mudarem no inspector
        if (Application.isPlaying && cameraController != null)
        {
            SetupBoundaries();
            
            if (showBoundariesInGame && boundaryRenderer != null)
            {
                UpdateBoundaryVisual();
            }
        }
    }
}
