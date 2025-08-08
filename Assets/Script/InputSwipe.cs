using UnityEngine;

public class InputSwipe : MonoBehaviour
{
    public float swipeThreshold = 50f;
    public Vector2 SwipeDirection { get; private set; }

    private Vector2 startTouch;
    private Vector2 endTouch;

    private void Update()
    {
        SwipeDirection = Vector2.zero;

        // --- PC Key test ---
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.UpArrow)) SwipeDirection = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) SwipeDirection = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) SwipeDirection = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) SwipeDirection = Vector2.right;

        // --- Mouse test ---
        if (Input.GetMouseButtonDown(0))
        {
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endTouch = Input.mousePosition;
            Vector2 swipe = endTouch - startTouch;

            if (swipe.magnitude > swipeThreshold)
            {
                if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                    SwipeDirection = swipe.x > 0 ? Vector2.right : Vector2.left;
                else
                    SwipeDirection = swipe.y > 0 ? Vector2.up : Vector2.down;
            }
        }
#endif

        // --- Mobile Touch ---
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                startTouch = touch.position;
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouch = touch.position;
                Vector2 swipe = endTouch - startTouch;

                if (swipe.magnitude > swipeThreshold)
                {
                    if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                        SwipeDirection = swipe.x > 0 ? Vector2.right : Vector2.left;
                    else
                        SwipeDirection = swipe.y > 0 ? Vector2.up : Vector2.down;
                }
            }
        }
    }
}
