using UnityEngine;

public class FireIconsSwitcher : MonoBehaviour
{
    [SerializeField] private FireIcon[] _icons;
    
    public void SetIcons(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            _icons[i].gameObject.SetActive(true);
        }
        
        for (int i = count; i < _icons.Length; ++i)
        {
            _icons[i].gameObject.SetActive(false);
        }
    }
}