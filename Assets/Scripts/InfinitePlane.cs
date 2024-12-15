using UnityEngine;

public class InfinitePlane : MonoBehaviour
{
    [SerializeField] private float scale = 1.0f;
    private Material material;

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        UpdateScale();
    }

    void UpdateScale()
    {
        if (material != null)
        {
            material.SetFloat("_Scale", scale);
        }
    }

    void OnValidate()
    {
        if (material != null)
        {
            UpdateScale();
        }
    }
}