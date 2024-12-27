using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatableObject : MonoBehaviour
{
    public void RotateLeft() {
        transform.Rotate(0, 15, 0);
    }

    public void RotateRight() {
        transform.Rotate(0, -15, 0);
    }
}
