using UnityEngine;

public class ShadowController : MonoBehaviour
{
    private bool shadowsEnabled = false;
    private Light mainLight;
    
    void Start()
    {
        // メインのディレクショナルライトを取得
        mainLight = FindObjectOfType<Light>();
        if (mainLight == null)
        {
            Debug.LogWarning("シーンにディレクショナルライトが見つかりません。");
        }
        
        // 初期状態では影を無効化
        UpdateShadowSettings();
    }
    
    void Update()
    {
        // Kキーで影の表示/非表示を切り替え
        if (Input.GetKeyDown(KeyCode.K))
        {
            shadowsEnabled = !shadowsEnabled;
            UpdateShadowSettings();
        }
    }
    
    void UpdateShadowSettings()
    {
        // ライトの影の設定を更新
        if (mainLight != null)
        {
            mainLight.shadows = shadowsEnabled ? LightShadows.Hard : LightShadows.None;
        }
        
        // クオリティ設定の更新
        QualitySettings.shadows = shadowsEnabled ? ShadowQuality.All : ShadowQuality.Disable;
        
        // すべてのレンダラーの影の設定を更新（プレビューキューブは除外）
        var renderers = FindObjectsOfType<Renderer>();
        foreach (var renderer in renderers)
        {
            // プレビューキューブのマテリアルかどうかをチェック
            if (renderer.material.renderQueue != 3000) // プレビューキューブは3000に設定されている
            {
                renderer.shadowCastingMode = shadowsEnabled ? 
                    UnityEngine.Rendering.ShadowCastingMode.On : 
                    UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
    }
} 