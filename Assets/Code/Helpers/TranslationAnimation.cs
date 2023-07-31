using UnityEngine;

public class TranslationAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _speed;
    private Vector3 _start;
    private Vector3 _target;

    private void Start()
    {
        _start = transform.position + _offset;
        _target = transform.position - _offset;
    }

    private void Update()
    {
        float uniformValue = Mathf.Sin(Time.time * _speed) * 0.5f + 0.5f;

        transform.position = Vector3.Lerp(_start, _target, uniformValue);
    }
}