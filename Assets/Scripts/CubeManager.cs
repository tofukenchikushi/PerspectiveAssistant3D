using UnityEngine;

public class CubeManager : MonoBehaviour
{
    [Header("Cube Settings")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Material previewMaterial;
    [SerializeField] private float placementDistance = 3f;
    [SerializeField] private float groundLevel = 0f;
    [SerializeField] private float maxDeletionDistance = 20f; // 削除可能な最大距離
    [SerializeField] private LayerMask deletionLayerMask; // 削除対象のレイヤー
    
    [Header("Preview Colors")]
    [SerializeField] private Color normalPreviewColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color groundedPreviewColor = new Color(0f, 1f, 0f, 0.5f);
    
    private enum RotationMode
    {
        Fixed,              // モード1: 固定（デフォルト）
        HorizontalOnly,     // モード2: 水平方向のみカメラに依存
        FullCamera         // モード3: 完全にカメラに依存
    }
    
    private RotationMode currentRotationMode = RotationMode.Fixed;
    private GameObject previewCube;
    private Camera mainCamera;
    private bool isEditMode = true;
    private Material previewMaterialInstance;
    
    private void Start()
    {
        mainCamera = Camera.main;
        CreatePreviewCube();
        UpdateCursorState();
        
        // プレハブのレイヤーを設定（レイヤー8を使用）
        if (cubePrefab != null)
        {
            cubePrefab.layer = 8; // 8番目のレイヤーを使用
        }
    }
    
    private void CreatePreviewCube()
    {
        previewCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        previewCube.GetComponent<Collider>().enabled = false;
        
        previewMaterialInstance = new Material(previewMaterial);
        var renderer = previewCube.GetComponent<Renderer>();
        renderer.material = previewMaterialInstance;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        
        previewMaterialInstance.color = normalPreviewColor;
        previewMaterialInstance.renderQueue = 3000;
    }
    
    private void Update()
    {
        // Lキーで編集モードの切り替え
        if (Input.GetKeyDown(KeyCode.L))
        {
            isEditMode = !isEditMode;
            UpdateCursorState();
        }
        
        // Hキーでローテーションモードの切り替え
        if (Input.GetKeyDown(KeyCode.H))
        {
            currentRotationMode = (RotationMode)(((int)currentRotationMode + 1) % 3);
            Debug.Log($"回転モードを変更: {currentRotationMode}");
        }
        
        // Nキーでエッジ延長線の表示切り替え
        if (Input.GetKeyDown(KeyCode.N))
        {
            CubeEdgeExtender.ToggleAllLines();
        }

        if (!isEditMode)
        {
            previewCube.SetActive(false);
            return;
        }

        previewCube.SetActive(true);
        UpdatePreviewPosition();
        UpdatePreviewRotation();
        HandleCubePlacement();
        HandleCubeDeletion();
    }

    private void UpdateCursorState()
    {
        Cursor.lockState = isEditMode ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isEditMode;
    }
    
    private void UpdatePreviewPosition()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 cameraForward = mainCamera.transform.forward;
        
        Vector3 targetPosition = cameraPosition + cameraForward * placementDistance;
        
        bool isGrounded = targetPosition.y - 0.5f < groundLevel;
        
        if (isGrounded)
        {
            targetPosition.y = groundLevel + 0.5f;
            previewMaterialInstance.color = groundedPreviewColor;
        }
        else
        {
            previewMaterialInstance.color = normalPreviewColor;
        }
        
        previewCube.transform.position = targetPosition;
    }
    
    private void UpdatePreviewRotation()
    {
        Quaternion targetRotation = Quaternion.identity;
        
        switch (currentRotationMode)
        {
            case RotationMode.Fixed:
                targetRotation = Quaternion.identity;
                break;
                
            case RotationMode.HorizontalOnly:
                // カメラの前方ベクトルを水平面に投影
                Vector3 forward = mainCamera.transform.forward;
                forward.y = 0;
                forward.Normalize();
                
                if (forward != Vector3.zero)
                {
                    targetRotation = Quaternion.LookRotation(forward);
                }
                break;
                
            case RotationMode.FullCamera:
                targetRotation = mainCamera.transform.rotation;
                break;
        }
        
        previewCube.transform.rotation = targetRotation;
    }
    
    private void HandleCubePlacement()
    {
        if (Input.GetMouseButtonDown(0)) // 左クリック
        {
            GameObject newCube = Instantiate(cubePrefab, previewCube.transform.position, previewCube.transform.rotation);
            // 生成したキューブのレイヤーを設定
            newCube.layer = 8; // 8番目のレイヤーを使用
            // エッジ延長コンポーネントはプレハブにアタッチ済みなので、ここでの追加は不要
        }
    }
    
    private void HandleCubeDeletion()
    {
        if (Input.GetMouseButtonDown(1)) // 右クリック
        {
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
            RaycastHit hit;
            
            // レイキャストで視線上のキューブを検出
            if (Physics.Raycast(ray, out hit, maxDeletionDistance, deletionLayerMask))
            {
                GameObject hitObject = hit.collider.gameObject;
                // プレビューキューブでないことを確認
                if (hitObject != previewCube)
                {
                    Destroy(hitObject);
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        if (previewMaterialInstance != null)
        {
            Destroy(previewMaterialInstance);
        }
    }
} 