using Fungus;
using Game.ProjectZ;
using MDI.Gamekit.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    #region SFIELDS
    [SerializeField] string interactablesLayer = "Interactable";
    [SerializeField] string grabLayer = "GrabbedObject";

    [SerializeField, Multiline] string examineTut = "INTERACT [A]\n";
    [SerializeField, Multiline] string grabTut = "GRAB [X]\n";
    [SerializeField, Multiline] string openTut = "OPEN [Y]\n";
    [SerializeField, Multiline] string inspectTut = "INSPECT [R1/R3]\n";

    [SerializeField] string examineAxis = "Examine";
    [SerializeField] float examineDistance = 1000f;

    [SerializeField] string grabAxis = "Grab";
    [SerializeField] float grabDistance = 3f;

    [SerializeField] string openAxis = "Use";
    [SerializeField] float openDistance = 3f;

    [SerializeField] string inspectAxis = "Inspect";
    [SerializeField] float inspectDistance = 3f;

    [SerializeField] SEvent OnExamine;
    [SerializeField] SEvent OnGrab;
    [SerializeField] SEvent OnInspect;
    #endregion

    public Transform ActiveInteractable { get; private set; } = null;
    public Transform ActiveOpenable { get; private set; } = null;
    public bool IsGrabbing { get; private set; } = false;
    public bool IsInspecting { get; private set; } = false;

    new Camera camera;
    Transform parent;
    new Collider collider;
    Rigidbody rigidBody;
    string tutorial = "";

    void Awake()
    {
        camera = Camera.main;
        parent = transform.parent;
        collider = GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (ActiveOpenable != null) return;
        ExamineOnInput(this.examineAxis);
        GrabOnInput(this.grabAxis);
        InspectOnInput(this.inspectAxis);
        OpenOnInput(this.openAxis);

        MouseOverOut();
    }

    #region ABSTRACTION
    public void SetActiveInteractable(Transform newActiveInteractable)
    {
        ActiveInteractable = newActiveInteractable;
    }

    public void SetActiveOpenable(Transform newActiveOpenable)
    {
        ActiveOpenable = newActiveOpenable;
    }

    GameObject DetectInteractableAtDistance(float distance)
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(InputManager.Instance.GetMousePosition());
        if (Physics.Raycast(ray, out hit, distance, ~(1 >> LayerMask.NameToLayer(interactablesLayer))) && hit.transform == transform)
            return hit.transform.gameObject;

        if (ActiveInteractable == transform)
            return ActiveInteractable.gameObject;
        return null;
    }

    void MouseOverOut()
    {
        var hitObject = DetectInteractableAtDistance(1000f);
        if (hitObject == null) return;
        if (hitObject.CompareTag(interactablesLayer))
        {
            if (hitObject.GetComponent<ExamineController>() != null)
                tutorial += examineTut;
            if (hitObject.GetComponent<GrabController>() != null)
                tutorial += grabTut;
            if (hitObject.GetComponent<DoorController>() != null)
                tutorial += openTut;
            if (hitObject.GetComponent<InspectController>() != null)
                tutorial += inspectTut;

            if(tutorial != "")
                CursorManager.Instance.ChangeMouseRadius(0f, 4f, hitObject.name.ToUpper(), tutorial);
            else
                CursorManager.Instance.ChangeMouseRadius(4f, 0f, hitObject.name.ToUpper(), tutorial);
        }
        else
        {
            CursorManager.Instance.ChangeMouseRadius(4f, 0f, "", "");
        }
    }

    void ExamineOnInput(string examineAxis)
    {
        if (InputManager.Instance.GetButtonDown(examineAxis) && NotInteracting())
        {
            var hitObject = DetectInteractableAtDistance(examineDistance);
            if (hitObject == null) return;
            hitObject.GetComponent<ExamineController>().Examine();
        }
    }

    void GrabOnInput(string grabAxis)
    {
        if (InputManager.Instance.GetButtonDown(grabAxis) && NotInteracting())
        {
            var hitObject = DetectInteractableAtDistance(grabDistance);
            if (hitObject == null) return;
            var grabController = hitObject.GetComponent<GrabController>();
            if (grabController == null) return;
            if (IsGrabbing && ActiveInteractable)
            {
                if (rigidBody != null)
                {
                    grabController.Grab(this, tutorial);
                }
                else
                {
                    grabController.CheckIfDoor();
                }
            }
            else
            {
                grabController.Release(this, tutorial);
                IsInspecting = false;
            }
        }
    }
    void InspectOnInput(string inspectAxis)
    {
        InspectController inspectController = null;

        if (InputManager.Instance.GetButtonDown(inspectAxis) && NotInteracting())
        {
            var hitObject = DetectInteractableAtDistance(inspectDistance);
            if (hitObject == null) return;
            inspectController = hitObject.GetComponent<InspectController>();
            if (inspectController == null) return;
            if (IsGrabbing && ActiveInteractable)
            {
                inspectController.Inspect();
            }
            else
            {
                if (IsGrabbing)
                {
                    inspectController.Release();
                }
            }
        }

        if (IsInspecting && IsGrabbing && inspectController != null)
        {
            inspectController.Rotate();
        }
    }

    void OpenOnInput(string openAxis)
    {
        if (InputManager.Instance.GetButtonDown(openAxis) && NotInteracting())
        {
            
        }

        if (InputManager.Instance.GetButtonDown(inspectAxis) && NotInteracting())
        {
            var hitObject = DetectInteractableAtDistance(inspectDistance);
            if (hitObject == null) return;
            var doorController = hitObject.GetComponent<DoorController>();
            if (doorController == null) return;
            doorController.OpenClose();
        }
    }

    static bool NotInteracting() =>
         FindObjectOfType<SayDialog>() == null;
    #endregion
}
