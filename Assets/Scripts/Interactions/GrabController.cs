using Fungus;
using Game.ProjectZ;
using MDI.Gamekit.Core;
using MDI.Gamekit.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    #region SFIELDS  
    [SerializeField] Vector3 grabViewOffset;
    [SerializeField] Vector3 grabViewRotationOffset;
    [SerializeField] float grabDistance = 1f;
    [SerializeField] string grabAxis = "Grab";
    [SerializeField] Transform grabOffset;
    [SerializeField] bool isDoor = false;
    [SerializeField] string defaultLayer = "Interactable";
    [SerializeField] string grabLayer = "GrabbedObject";
    [SerializeField] GameObject[] useObjects;
    [SerializeField] string floorObjectName = "Floor";
    #endregion

    Transform parent;
    Collider collider;
    Rigidbody rigidBody;
    Flowchart mainFlow;
    PlayerController playerController;

    // Interactables
    IGrabbable grabInteractable;


    void Awake()
    {
        GetReferences();
        InitialiseInteractables();
    }

    #region ABSTRACTION
    void GetReferences()
    {
        parent = transform.parent;
        collider = GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();
        mainFlow = GameObject.FindGameObjectWithTag("MainFlow").GetComponent<Flowchart>();
        playerController = FindObjectOfType<PlayerController>();
    }

    void InitialiseInteractables()
    {
        grabInteractable = new GrabInteractable(rigidBody, transform);
    }

    public void Grab(PlayerInteractor pi, string tut)
    {

        rigidBody.velocity = Vector3.zero;
        grabInteractable.Grab(grabOffset, grabViewOffset);

        if (grabOffset.childCount > 0)
        {
            foreach (var useObject in useObjects)
            {
                useObject.SetActive(true);
            }

            DoNotClip();

            CursorManager.Instance.ChangeMouseRadius(20f, 1f, name.ToUpper(), tut);
            grabOffset.GetChild(0).eulerAngles = grabViewRotationOffset;
        }

        collider.isTrigger = true;
        pi.SetActiveInteractable(transform);
    }

    public void CheckIfDoor()
    {
        if (isDoor)
            mainFlow.ExecuteBlock("DoorLocked");
        else
            mainFlow.ExecuteBlock("ToBigToGrab");
    }

    public void Release(PlayerInteractor pi, string tut)
    {
        foreach (var useObject in useObjects)
        {
            useObject.SetActive(false);
        }

        if (grabOffset.childCount > 0)
        {
            grabOffset.gameObject.SetLayer(9);
            grabOffset.GetChild(0).gameObject.SetLayer(3);
        }

        grabInteractable.Release(parent);
        collider.isTrigger = false;
        CursorManager.Instance.ChangeMouseRadius(4f, 0f, name.ToUpper(), tut);
        pi.SetActiveInteractable(null);
        playerController.TurnCameraFollowOn(playerController.GetCameraBehaviour());
    }

    void DoNotClip()
    {
        grabOffset.gameObject.SetLayer(9);
        grabOffset.GetChild(0).gameObject.SetLayer(9);
    }

    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (string.Compare(collision.gameObject.name, floorObjectName) == 0)
        {
            transform.parent = collision.gameObject.transform;
        }
    }

    public IGrabbable GetGrabInteractable() =>
        grabInteractable;
    #endregion
}
