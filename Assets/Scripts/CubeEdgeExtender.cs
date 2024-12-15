using UnityEngine;

public class CubeEdgeExtender : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] private float lineLength = 100f; // 延長線の長さ（片側）
    [SerializeField] private float lineWidth = 0.02f; // 線の太さ
    
    private LineRenderer[] xAxisLines; // X軸方向の線 (赤)
    private LineRenderer[] yAxisLines; // Y軸方向の線 (青)
    private LineRenderer[] zAxisLines; // Z軸方向の線 (緑)
    
    // 静的変数でグローバルな状態を管理
    private static bool globalLinesEnabled = false;
    public static bool GlobalLinesEnabled 
    { 
        get { return globalLinesEnabled; }
        private set { globalLinesEnabled = value; }
    }
    
    private void Awake()
    {
        InitializeLines();
        SetLinesActive(GlobalLinesEnabled);
    }
    
    private void InitializeLines()
    {
        xAxisLines = CreateAxisLines(Color.red, 4);
        yAxisLines = CreateAxisLines(Color.blue, 4);
        zAxisLines = CreateAxisLines(Color.green, 4);
    }
    
    private LineRenderer[] CreateAxisLines(Color color, int count)
    {
        LineRenderer[] lines = new LineRenderer[count];
        for (int i = 0; i < count; i++)
        {
            GameObject lineObj = new GameObject($"Line_{color.ToString()}_{i}");
            lineObj.transform.SetParent(transform);
            
            LineRenderer line = lineObj.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Unlit/Color"));
            line.material.color = color;
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.positionCount = 2;
            
            lines[i] = line;
        }
        return lines;
    }
    
    public static void ToggleAllLines()
    {
        GlobalLinesEnabled = !GlobalLinesEnabled;
        var extenders = FindObjectsOfType<CubeEdgeExtender>();
        foreach (var extender in extenders)
        {
            extender.SetLinesActive(GlobalLinesEnabled);
            if (GlobalLinesEnabled)
            {
                extender.UpdateLines();
            }
        }
    }
    
    private void SetLinesActive(bool active)
    {
        if (xAxisLines == null) return;
        foreach (var line in xAxisLines) if (line != null) line.gameObject.SetActive(active);
        foreach (var line in yAxisLines) if (line != null) line.gameObject.SetActive(active);
        foreach (var line in zAxisLines) if (line != null) line.gameObject.SetActive(active);
    }
    
    private void UpdateLines()
    {
        if (!GlobalLinesEnabled) return;
        
        Vector3 center = transform.position;
        Vector3 right = transform.right * 0.5f;
        Vector3 up = transform.up * 0.5f;
        Vector3 forward = transform.forward * 0.5f;
        
        // X軸方向の線（赤）
        UpdateAxisLines(xAxisLines, new Vector3[]
        {
            center + up + forward,
            center + up - forward,
            center - up + forward,
            center - up - forward
        }, transform.right);
        
        // Y軸方向の線（青）
        UpdateAxisLines(yAxisLines, new Vector3[]
        {
            center + right + forward,
            center + right - forward,
            center - right + forward,
            center - right - forward
        }, transform.up);
        
        // Z軸方向の線（緑）
        UpdateAxisLines(zAxisLines, new Vector3[]
        {
            center + right + up,
            center + right - up,
            center - right + up,
            center - right - up
        }, transform.forward);
    }
    
    private void UpdateAxisLines(LineRenderer[] lines, Vector3[] startPoints, Vector3 direction)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            Vector3 startPoint = startPoints[i];
            lines[i].SetPosition(0, startPoint - direction * lineLength);
            lines[i].SetPosition(1, startPoint + direction * lineLength);
        }
    }
    
    private void Update()
    {
        if (GlobalLinesEnabled)
        {
            UpdateLines();
        }
    }
    
    private void OnDestroy()
    {
        if (xAxisLines == null) return;
        foreach (var line in xAxisLines) if (line != null && line.material != null) Destroy(line.material);
        foreach (var line in yAxisLines) if (line != null && line.material != null) Destroy(line.material);
        foreach (var line in zAxisLines) if (line != null && line.material != null) Destroy(line.material);
    }
} 