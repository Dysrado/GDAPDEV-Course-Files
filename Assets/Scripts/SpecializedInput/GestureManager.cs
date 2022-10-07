using System;
using UnityEngine;

public class GestureManager : MonoBehaviour
{
    private static GestureManager instance;
    public static GestureManager Instance
    {
        get { return instance; }
    }

    public EventHandler<TapEventArgs> OnTap;
    public EventHandler<SwipeEventArgs> OnSwipe;
    public EventHandler<TwoFingerPanEventArgs> OnTwoFingerPan;

    public TapProperty _tapProperty;
    public SwipeProperty _swipeProperty;
    public TwoFingerPanProperty _twoFingerPanProperty;

    private Vector2 startPoint = Vector2.zero;
    private Vector2 endPoint = Vector2.zero;
    // Total time of the gesture
    private float gestureTime = 0f;
    // Touch trackers
    private Touch trackedFinger1;
    private Touch trackedFinger2;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1)
                CheckSingleFingerGestures();
            else if (Input.touchCount > 1)
            {
                trackedFinger1 = Input.GetTouch(0);
                trackedFinger2 = Input.GetTouch(1);
                
                if (trackedFinger1.phase == TouchPhase.Moved && trackedFinger2.phase == TouchPhase.Moved &&
                    Vector2.Distance(trackedFinger1.position, trackedFinger2.position) <= _twoFingerPanProperty.maxDistance * Screen.dpi)
                {
                    FireTwoFingerPanEvent();
                }
            }
        }
    }

    private void CheckSingleFingerGestures()
    {
        trackedFinger1 = Input.GetTouch(0);

        if (trackedFinger1.phase == TouchPhase.Began)
        {
            startPoint = trackedFinger1.position;
            gestureTime = 0;
        }
        else if (trackedFinger1.phase == TouchPhase.Ended)
        {
            endPoint = trackedFinger1.position;


            // Tap Gesture
            if (gestureTime <= _tapProperty.tapTime && Vector2.Distance(startPoint, endPoint) <= _tapProperty.tapMaxDistance * Screen.dpi)
                FireTapEvent(endPoint);
            // Swipe Gesture
            else if (gestureTime <= _swipeProperty.swipeTime &&
                Vector2.Distance(startPoint, endPoint) >= _swipeProperty.minSwipeDistance * Screen.dpi)
            {
                FireSwipeEvent();
            }
        }
        else
        {
            gestureTime += Time.deltaTime;
        }
    }

    private void FireTapEvent(Vector2 pos)
    {
        Debug.Log("Tap!");
        // Check if anything is listening first
        if (OnTap != null)
        {
            GameObject hitObj = null;
            Ray r = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;

            if (Physics.Raycast(r, out hit, Mathf.Infinity)) {
                hitObj = hit.collider.gameObject;
            }

            TapEventArgs tapArgs = new TapEventArgs(pos, hitObj);
            // Notify tap listeners with tap event
            OnTap(this, tapArgs);

            // If the hit object is tappable, call its OnTap method
            if (hitObj != null)
            {
                ITappable tappable = hitObj.GetComponent<ITappable>();
                if (tappable != null)
                    tappable.OnTap();
            }
        }
    }

    private void FireSwipeEvent()
    {
        Debug.Log("Swipe!");
        Vector2 direction = endPoint - startPoint;
        SwipeDirection swipeDir;

        if (Math.Abs(direction.x) > Math.Abs(direction.y))
        {
            // Horizontal
            if (direction.x > 0)
            {
                Debug.Log("Right");
                swipeDir = SwipeDirection.RIGHT;
            }
            else
            {
                Debug.Log("Left");
                swipeDir = SwipeDirection.LEFT;
            }

        }
        else
        {
            // Vertical
            if (direction.y > 0)
            {
                Debug.Log("Up");
                swipeDir = SwipeDirection.UP;
            }
            else
            {
                Debug.Log("Down");
                swipeDir = SwipeDirection.DOWN;
            }
        }


        GameObject hitObj = null;
        Ray r = Camera.main.ScreenPointToRay(startPoint);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, Mathf.Infinity))
        {
            hitObj = hit.collider.gameObject;
        }

        SwipeEventArgs swipeArgs = new SwipeEventArgs(swipeDir, startPoint, direction, hitObj);
        if (OnSwipe != null)
        {
            OnSwipe(this, swipeArgs);
        }

        if (hitObj != null) {
            ISwipeable swipeable = hitObj.GetComponent<ISwipeable>();
            if (swipeable != null)
                swipeable.OnSwipe(swipeArgs);
        }

    }

    private void FireTwoFingerPanEvent()
    {
        TwoFingerPanEventArgs args = new TwoFingerPanEventArgs(trackedFinger1, trackedFinger2);
        if (OnTwoFingerPan != null)
        {
            OnTwoFingerPan(this, args);
        }
    }


    private void OnDrawGizmos()
    {
        int touchCount = Input.touchCount;

        if (touchCount > 0)
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                Touch t = Input.GetTouch(i);

                Ray r = Camera.main.ScreenPointToRay(t.position);

                switch (t.fingerId)
                {
                    case 0: Gizmos.DrawIcon(r.GetPoint(10), "Orbs/Airless"); break;
                    case 1: Gizmos.DrawIcon(r.GetPoint(10), "Orbs/Curseless"); break;
                    case 2: Gizmos.DrawIcon(r.GetPoint(10), "Orbs/Blless"); break;
                    case 3: Gizmos.DrawIcon(r.GetPoint(10), "Orbs/Flameless"); break;
                    case 4: Gizmos.DrawIcon(r.GetPoint(10), "Orbs/Orb of Venom"); break;
                    default: Gizmos.DrawIcon(r.GetPoint(10), "Orbs/Emptyless"); break;
                }
            }
        }
    }
}