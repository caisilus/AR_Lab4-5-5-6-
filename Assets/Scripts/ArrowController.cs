using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowController : MonoBehaviour
{
    public RectTransform arrow;       // Assign the arrow's RectTransform in the Inspector
    public GameObject closestObject;  // The closest object to track
    public float rotationSpeed = 5f;  // Speed for smooth rotation

    void Update()
    {
        if (arrow == null) return;

        if (closestObject == null) {
            arrow.gameObject.SetActive(false);
        }

        Vector3 objectScreenPos = Camera.main.WorldToScreenPoint(closestObject.transform.position);

        bool isVisible = objectScreenPos.z > 0 && objectScreenPos.x > 0 &&
                         objectScreenPos.x < Screen.width && objectScreenPos.y > 0 &&
                         objectScreenPos.y < Screen.height;

        arrow.gameObject.SetActive(!isVisible);

        if (!isVisible)
        {
            PointArrowAtObjectSmoothly(objectScreenPos);
        }
    }

    void PointArrowAtObjectSmoothly(Vector3 objectScreenPos)
    {
        Vector3 clampedPosition = objectScreenPos;
        if (objectScreenPos.z < 0) // If the object is behind the camera, invert its position
        {
            clampedPosition *= -1;
        }

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, 0, Screen.width);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, 0, Screen.height);

        Vector2 direction = (Vector2)clampedPosition - new Vector2(Screen.width / 2, Screen.height / 2);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        arrow.rotation = Quaternion.Lerp(arrow.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        arrow.position = new Vector3(
            Mathf.Clamp(clampedPosition.x, 50, Screen.width - 50), // Prevent arrow from going off-screen
            Mathf.Clamp(clampedPosition.y, 50, Screen.height - 50),
            0);
    }
}
