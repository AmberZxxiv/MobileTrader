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
    public GameObject wagonMenu;


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

        if (isHorizontal) //movimiento horizontal de zona
        {
            if (delta.x < 0) HorizontCamera(1);
            if (delta.x > 0) HorizontCamera(-1);
        }

        if (isVertical) //movimiento vertical de zona
        {
            if (delta.y < 0) VerticalCamera(1);
            if (delta.y > 0) VerticalCamera(-1);
        }

        if (isVertical && startedInBottomZone) // despliegue del carro menu
        {
            if (delta.y > 0) //deslizo hacia arriba y abro el carro
            { 
            tentMenu.SetActive(false);
            wagonMenu.SetActive(true);
            }
            if (delta.y < 0) //deslizo hacia abajo y cierro el carro
            {
             tentMenu.SetActive(true);
             wagonMenu.SetActive(false);
            }
        }
    }

    void HorizontCamera(int direction)
    {
        Vector2 pos = _camera.transform.position;
        pos.x += scrollDistance * direction;
        _camera.transform.position = pos;
    }

    void VerticalCamera(int direction)
    {
        Vector2 pos = _camera.transform.position;
        pos.y += scrollDistance * direction;
        _camera.transform.position = pos;
    }
}
