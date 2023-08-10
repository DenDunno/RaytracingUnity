using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class RoomRenderingCommand : MonoBehaviour
{
    [SerializeField] private Raytracing _raytracing;
    [SerializeField] private Room[] _rooms;
    [SerializeField] private Button _startButton;
    [SerializeField] private HorizontalScrollSnap _horizontalScrollSnap;
    [SerializeField] private FireIconsSwitcher _switcher;
    private int _currentIndex;

    private void Start()
    {
        _rooms[0].gameObject.SetActive(true);

        for (int i = 1; i < _rooms.Length; ++i)
        {
            _rooms[i].gameObject.SetActive(false);
        }

        A[] array = null;

        int b = 3;
        int aLength = 20;
        int iterations = b / aLength;
        int remainder = b - aLength * iterations;

        int index = 0;
        for (int i = 0; i < iterations; i++, index += aLength)
        {
            int startIndex = index;
            int endIndex = index + aLength;
            
            Pass(array[startIndex..endIndex]);
        }

        if (remainder > 0)
            Pass(array[index..array.Length]);
    }

    private void Pass(A[] a)
    {
        
    }
    class A
    {
    }

    private void OnEnable()
    {
        _startButton.onClick.AddListener(RenderRoom);
    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(RenderRoom);
    }

    private void Update()
    {
        if (_horizontalScrollSnap.CurrentPage != _currentIndex)
        {
            _rooms[_currentIndex].gameObject.SetActive(false);
            _rooms[_horizontalScrollSnap.CurrentPage].gameObject.SetActive(true);
            _switcher.SetIcons(_horizontalScrollSnap.CurrentPage + 1);
            
            _currentIndex = _horizontalScrollSnap.CurrentPage;
        }
    }

    private void RenderRoom()
    {
        ToggleRoom(false);
        _raytracing.Enable(_rooms[_horizontalScrollSnap.CurrentPage]);
    }

    public void ToggleRoom(bool enable)
    {
        _rooms[_horizontalScrollSnap.CurrentPage].gameObject.SetActive(enable);
    }
}
