using UnityEngine;

[ExecuteAlways]
public class CameraControlling : MonoBehaviour
{
    [SerializeField] private float _speed = 2;
    [SerializeField] private float _sensitivity = 2;
    
    private void Update()
    {
        Vector2 input = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 mouseInput = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 direction = EvaluateDirection(input);
        
        transform.eulerAngles += new Vector3(-mouseInput.y * _sensitivity, mouseInput.x * _sensitivity, 0);
        transform.position += direction;
    }
    
    private Vector3 EvaluateDirection(Vector2 input)
    {
        float speed = _speed;

        if (Input.GetKey(KeyCode.LeftShift))
            speed *= 3;
        
        Vector3 direction = transform.forward * (input.y * speed * Time.deltaTime) + 
                            transform.right * (input.x * speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftControl))
            direction -= Vector3.up * (speed * Time.deltaTime);
        
        if (Input.GetKey(KeyCode.Space))
            direction += Vector3.up * (speed * Time.deltaTime);

        return direction;
    }
}