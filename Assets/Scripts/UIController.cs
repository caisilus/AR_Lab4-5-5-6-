using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject buttonCanvas;
    [SerializeField] Button RotateButtonLeft;
    [SerializeField] Button RotateButtonRight;
    [SerializeField] AddWPSObjects addWPSObjects;
    [SerializeField] XROrigin origin;

    private RotatableObject _closestRotatable = null;

    public void RotateLeft() {
        _closestRotatable.RotateLeft();
    }

    public void RotateRight() {
        _closestRotatable.RotateRight();
    }

    private void Start() {
        buttonCanvas.SetActive(false);
        addWPSObjects.OnWPSInitialized += ActivateCanvas;
    }

    private void ActivateCanvas() {
        buttonCanvas.SetActive(true);
    }

    private void Update() {
        if (addWPSObjects.InstantiatedObjects.Count == 0) {
            DeactivateRotateButtons();
        }

        float minDistance = float.MaxValue;
        GameObject closestObject = null;

        foreach (var obj in addWPSObjects.InstantiatedObjects) {
            float distanceToCamera = Vector3.Distance(origin.transform.position, obj.transform.position);
            if (distanceToCamera < minDistance) {
                minDistance = distanceToCamera;
                closestObject = obj;
            }
        }

        _closestRotatable = closestObject.GetComponentInChildren<RotatableObject>();

        if (_closestRotatable != null) {
            ActivateRotateButtons();
        }
    }

    private void ActivateRotateButtons() {
        RotateButtonLeft.gameObject.SetActive(true);
        RotateButtonRight.gameObject.SetActive(true);
    }

    private void DeactivateRotateButtons() {
        RotateButtonLeft.gameObject.SetActive(false);
        RotateButtonRight.gameObject.SetActive(false);
        _closestRotatable = null;
    }
}
