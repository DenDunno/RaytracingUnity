using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class RoomPreviewButtons : MonoBehaviour
{
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private HorizontalScrollSnap _horizontalScrollSnap;
    
    private void Update()
    {
        int roomsCount = _horizontalScrollSnap.ChildObjects.Length;
        
        _leftButton.gameObject.SetActive(_horizontalScrollSnap.CurrentPage != 0);
        _rightButton.gameObject.SetActive(_horizontalScrollSnap.CurrentPage != roomsCount - 1);
    }
}