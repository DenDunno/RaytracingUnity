using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class RoomRenderingCommand : MonoBehaviour
{
    [SerializeField] private Raytracing _raytracing;
    [SerializeField] private Room[] _rooms;
    [SerializeField] private Button _startButton;
    [SerializeField] private HorizontalScrollSnap _horizontalScrollSnap;
    private int _currentIndex;

    private void Start()
    {
        _rooms[0].gameObject.SetActive(true);

        for (int i = 1; i < _rooms.Length; ++i)
        {
            _rooms[i].gameObject.SetActive(false);
        }
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
