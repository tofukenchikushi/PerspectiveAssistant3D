using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float verticalSpeed = 3.0f;
    
    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float minVerticalAngle = -89f;
    [SerializeField] private float maxVerticalAngle = 89f;
    
    [Header("Field of View Settings")]
    [SerializeField] private float minFOV = 60f;
    [SerializeField] private float maxFOV = 120f;
    [SerializeField] private float fovSensitivity = 5f;
    
    private float rotationX = 0;
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        mainCamera.fieldOfView = 90f; // デフォルトの視野角
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleFieldOfView();
    }
    
    private void HandleMovement()
    {
        Vector3 move = Vector3.zero;
        
        // カメラの前方ベクトルを水平面に投影
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();
        
        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();
        
        // 前後左右移動（水平面上のみ）
        if (Input.GetKey(KeyCode.W)) move += forward;
        if (Input.GetKey(KeyCode.S)) move -= forward;
        if (Input.GetKey(KeyCode.D)) move += right;
        if (Input.GetKey(KeyCode.A)) move -= right;
        
        // 移動ベクトルの正規化
        if (move != Vector3.zero)
        {
            move = move.normalized * moveSpeed * Time.deltaTime;
        }
        
        // 上下移動
        if (Input.GetKey(KeyCode.Space))
        {
            move += Vector3.up * verticalSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            move += Vector3.down * verticalSpeed * Time.deltaTime;
        }
        
        transform.position += move;
    }
    
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
        
        transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y + mouseX, 0f);
    }
    
    private void HandleFieldOfView()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float newFOV = mainCamera.fieldOfView - scroll * fovSensitivity;
            mainCamera.fieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);
        }
    }
} 