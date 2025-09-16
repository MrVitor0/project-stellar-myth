using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraFollowController followController;
    [SerializeField] private TopDownCameraSetup topDownSetup;
    
    [Header("Camera Modes")]
    [SerializeField] private CameraMode currentMode = CameraMode.FollowPlayer;
    [SerializeField] private bool allowModeSwitch = false;
    [SerializeField] private KeyCode switchModeKey = KeyCode.C;
    
    [Header("Fixed Camera Settings")]
    [SerializeField] private Transform fixedCameraPosition;
    
    public enum CameraMode
    {
        FollowPlayer,
        Fixed,
        Free
    }
    
    private Vector3 originalCameraPosition;
    private bool isInitialized = false;
    
    private void Awake()
    {
        InitializeReferences();
    }
    
    private void Start()
    {
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
        }
        
        SetCameraMode(currentMode);
        isInitialized = true;
    }
    
    private void Update()
    {
        if (allowModeSwitch && Input.GetKeyDown(switchModeKey))
        {
            SwitchToNextMode();
        }
        
        HandleCurrentMode();
    }
    
    private void InitializeReferences()
    {
        // Encontra câmera principal se não definida
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
        }
        
        // Encontra componentes de câmera se não definidos
        if (followController == null)
        {
            followController = mainCamera.GetComponent<CameraFollowController>();
        }
        
        if (topDownSetup == null)
        {
            topDownSetup = mainCamera.GetComponent<TopDownCameraSetup>();
        }
    }
    
    private void HandleCurrentMode()
    {
        switch (currentMode)
        {
            case CameraMode.FollowPlayer:
                // O CameraFollowController já lida com isso
                break;
                
            case CameraMode.Fixed:
                HandleFixedCamera();
                break;
                
            case CameraMode.Free:
                HandleFreeCamera();
                break;
        }
    }
    
    private void HandleFixedCamera()
    {
        if (fixedCameraPosition != null)
        {
            mainCamera.transform.position = fixedCameraPosition.position;
            mainCamera.transform.rotation = fixedCameraPosition.rotation;
        }
    }
    
    private void HandleFreeCamera()
    {
        // Implementação básica de câmera livre com WASD
        float moveSpeed = 10f;
        Vector3 moveDirection = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W)) moveDirection += Vector3.up;
        if (Input.GetKey(KeyCode.S)) moveDirection += Vector3.down;
        if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;
        
        if (moveDirection != Vector3.zero)
        {
            mainCamera.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
    
    public void SetCameraMode(CameraMode mode)
    {
        currentMode = mode;
        
        if (!isInitialized) return;
        
        // Habilita/desabilita componentes baseado no modo
        switch (mode)
        {
            case CameraMode.FollowPlayer:
                if (followController != null)
                    followController.enabled = true;
                break;
                
            case CameraMode.Fixed:
                if (followController != null)
                    followController.enabled = false;
                break;
                
            case CameraMode.Free:
                if (followController != null)
                    followController.enabled = false;
                break;
        }
    }
    
    private void SwitchToNextMode()
    {
        int currentIndex = (int)currentMode;
        int nextIndex = (currentIndex + 1) % System.Enum.GetValues(typeof(CameraMode)).Length;
        SetCameraMode((CameraMode)nextIndex);
    }
    
    // Métodos públicos para controle externo
    public void SetFollowPlayerMode()
    {
        SetCameraMode(CameraMode.FollowPlayer);
    }
    
    public void SetFixedMode()
    {
        SetCameraMode(CameraMode.Fixed);
    }
    
    public void SetFreeMode()
    {
        SetCameraMode(CameraMode.Free);
    }
    
    public void SetFixedPosition(Transform position)
    {
        fixedCameraPosition = position;
    }
    
    public void EnableModeSwitch(bool enable)
    {
        allowModeSwitch = enable;
    }
    
    public CameraMode GetCurrentMode()
    {
        return currentMode;
    }
    
    // Método para resetar a câmera para posição original
    public void ResetCamera()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPosition;
            SetCameraMode(CameraMode.FollowPlayer);
        }
    }
    
    // Métodos de conveniência para acesso aos componentes
    public void SetCameraTarget(Transform target)
    {
        if (followController != null)
        {
            followController.SetTarget(target);
        }
    }
    
    public void SetCameraOffset(Vector3 offset)
    {
        if (followController != null)
        {
            followController.SetOffset(offset);
        }
    }
    
    public void SetCameraFollowSpeed(float speed)
    {
        if (followController != null)
        {
            followController.SetFollowSpeed(speed);
        }
    }
}
