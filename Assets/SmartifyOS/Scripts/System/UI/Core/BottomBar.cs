using UnityEngine;

namespace SmartifyOS.UI
{

    //TODO: Add access to info display here
    public class BottomBar : MonoBehaviour
    {
        private float startYPosition = 0f;
        private bool isVisible = true;

        private void Awake()
        {
            startYPosition = transform.position.y;
        }

        public void Show()
        {
            if (isVisible)
                return;

            isVisible = true;
            Debug.Log("Showing bottom bar");
            LeanTween.moveY(gameObject, startYPosition, 0.2f).setEaseInOutSine();
        }

        public void Hide()
        {
            if (!isVisible)
                return;

            isVisible = false;
            Debug.Log("Hiding bottom bar");
            LeanTween.moveY(gameObject, startYPosition - 200, 0.2f).setEaseInOutSine();
        }
    }
}
