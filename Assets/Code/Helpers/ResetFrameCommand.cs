using UnityEngine;

[RequireComponent(typeof(Raytracing))]
public class ResetFrameCommand : MonoBehaviour
{
    [SerializeField] private Raytracing _raytracing;
    
    private void OnValidate()
    {
        _raytracing = GetComponent<Raytracing>();
    }

    private void Start()
    {
        _raytracing.ResetFrame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _raytracing.ResetFrame();
        }
    }
}