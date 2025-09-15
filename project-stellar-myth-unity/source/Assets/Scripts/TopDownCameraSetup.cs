using UnityEngine;

public class TopDownCameraSetup : MonoBehaviour
{
    [Header("Camera Configuration")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool configureOnStart = true;
    
    [Header("Top-Down Settings")]
    [SerializeField] private float cameraHeight = 10f; // Altura da câmera para visão top-down
    [SerializeField] private float orthographicSize = 8f; // Tamanho ortográfico (zoom)
    [SerializeField] private bool useOrthographicProjection = true;
    
    [Header("Camera Angle")]
    [SerializeField] private Vector3 cameraRotation = new Vector3(90f, 0f, 0f); // Rotação para top-down
    [SerializeField] private bool lockRotation = true; // Bloquear rotação da câmera
    
    [Header("Zoom Settings")]
    [SerializeField] private bool enableZoom = false;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private KeyCode zoomInKey = KeyCode.E;
    [SerializeField] private KeyCode zoomOutKey = KeyCode.Q;
    
    private float targetOrthographicSize;
    private CameraFollowController followController;
    
    private void Awake()
    {
        // Encontra a câmera se não foi definida
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }
        
        // Encontra o follow controller
        followController = GetComponent<CameraFollowController>();
        
        if (targetCamera == null)
        {
            Debug.LogError("TopDownCameraSetup: Nenhuma câmera encontrada!");
            enabled = false;
            return;
        }
    }
    
    private void Start()
    {
        if (configureOnStart)
        {
            ConfigureTopDownCamera();
        }
        
        targetOrthographicSize = orthographicSize;
    }
    
    private void Update()
    {
        if (enableZoom)
        {
            HandleZoomInput();
        }
        
        if (lockRotation)
        {
            LockCameraRotation();
        }
    }
    
    private void ConfigureTopDownCamera()
    {
        if (targetCamera == null) return;
        
        // Configura projeção ortográfica se especificado
        if (useOrthographicProjection)
        {
            targetCamera.orthographic = true;
            targetCamera.orthographicSize = orthographicSize;
        }
        
        // Define a rotação da câmera para top-down
        transform.rotation = Quaternion.Euler(cameraRotation);
        
        // Ajusta o offset do follow controller se existir
        if (followController != null)
        {
            Vector3 topDownOffset = new Vector3(0, 0, -cameraHeight);
            followController.SetOffset(topDownOffset);
        }
        
        Debug.Log("TopDownCameraSetup: Câmera configurada para visão top-down");
    }
    
    private void HandleZoomInput()
    {
        if (!useOrthographicProjection || targetCamera == null) return;
        
        float zoomInput = 0f;
        
        // Input via teclado
        if (Input.GetKey(zoomInKey))
        {
            zoomInput = -1f;
        }
        else if (Input.GetKey(zoomOutKey))
        {
            zoomInput = 1f;
        }
        
        // Input via scroll do mouse
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            zoomInput = -scroll * 5f; // Multiplica para tornar o scroll mais sensível
        }
        
        // Aplica o zoom
        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            targetOrthographicSize += zoomInput * zoomSpeed * Time.deltaTime;
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minZoom, maxZoom);
            
            // Suaviza o zoom
            targetCamera.orthographicSize = Mathf.Lerp(targetCamera.orthographicSize, targetOrthographicSize, Time.deltaTime * 3f);
        }
    }
    
    private void LockCameraRotation()
    {
        if (transform.rotation != Quaternion.Euler(cameraRotation))
        {
            transform.rotation = Quaternion.Euler(cameraRotation);
        }
    }
    
    // Método público para configurar a câmera manualmente
    public void SetupTopDownCamera()
    {
        ConfigureTopDownCamera();
    }
    
    // Método público para alterar a altura da câmera
    public void SetCameraHeight(float height)
    {
        cameraHeight = height;
        if (followController != null)
        {
            Vector3 newOffset = new Vector3(0, 0, -height);
            followController.SetOffset(newOffset);
        }
    }
    
    // Método público para alterar o tamanho ortográfico
    public void SetOrthographicSize(float size)
    {
        orthographicSize = size;
        targetOrthographicSize = size;
        if (targetCamera != null && useOrthographicProjection)
        {
            targetCamera.orthographicSize = size;
        }
    }
    
    // Método público para alterar a rotação da câmera
    public void SetCameraRotation(Vector3 rotation)
    {
        cameraRotation = rotation;
        transform.rotation = Quaternion.Euler(rotation);
    }
    
    // Método público para habilitar/desabilitar zoom
    public void SetZoomEnabled(bool enabled)
    {
        enableZoom = enabled;
    }
    
    // Método público para definir limites de zoom
    public void SetZoomLimits(float min, float max)
    {
        minZoom = min;
        maxZoom = max;
    }
}
