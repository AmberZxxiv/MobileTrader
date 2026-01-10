using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ClassicTouch = UnityEngine.Touch;

public class Scroll_Control : MonoBehaviour
{
    public float scrollForce;
    public float scrollDistance;
    Vector2 _touchStart;
    float _bottomScreen;
    Camera _camera;
    public GameObject tentMenu;
    

    void Start()
    {
    _camera = Camera.main;
    _bottomScreen = Screen.height * 0.25f; ;
    }

    void Update()
    {
        if (Input.touchCount > 0) //pal tactil del movil
        {
            ClassicTouch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            { _touchStart = touch.position; }

            if (touch.phase == TouchPhase.Ended)
            { SwipeDirector(touch.position);  }
        }

        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) // pal raton del PC
        { _touchStart = Input.mousePosition; }

        if (Input.GetMouseButtonUp(0))
        { SwipeDirector(Input.mousePosition); }
        #endif
    }

    void SwipeDirector(Vector2 endPos)
    {
        Vector2 delta = endPos - _touchStart;

        if (delta.magnitude < scrollForce) return;

        bool isHorizontal = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);
        bool isVertical = Mathf.Abs(delta.y) > Mathf.Abs(delta.x);
        float bottomZoneHeight = _bottomScreen;
        bool startedInBottomZone = _touchStart.y < bottomZoneHeight;

        if (isHorizontal) // Camara Lado
        {
            if (delta.x < 0) MoveCamera(1);
            if (delta.x > 0) MoveCamera(-1);
        }

        if (isVertical && startedInBottomZone) // Carro Menu
        {
            RectTransform rt = tentMenu.GetComponent<RectTransform>();

            if (delta.y > 0)
            { rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0f); }
            if (delta.y < 0)
            { rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -1575f); }
        }
    }

    void MoveCamera(int direction)
    {
        Vector2 pos = _camera.transform.position;
        pos.x += scrollDistance * direction;
        _camera.transform.position = pos;
    }
}
