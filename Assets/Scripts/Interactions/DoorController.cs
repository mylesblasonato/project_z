using MDI.Gamekit.Interactables;
using UnityEngine;
using Cinemachine;
using System.Collections;

namespace Game.ProjectZ
{
    public class DoorController : MonoBehaviour
    {
        #region SFIELDS
        [SerializeField] string playerCameraTag = "PlayerCamera", playerCameraOpenTag = "PlayerCameraOpen";
        [SerializeField] Vector3 rotationAxis = new Vector3(0, 1, 0);
        [SerializeField] float degrees = 1f;
        [SerializeField] float amount = 1f;
        [SerializeField] float duration = 1f;
        [SerializeField] string layerMask;
        [SerializeField] float grabDistance = 1f;
        [SerializeField] string grabAxis = "Grab";
        [SerializeField] float distanceToBreakGrab;
        [SerializeField] bool isDrawer = false;
        [SerializeField, Multiline] string tutorialText = "OPEN [Y]";
        #endregion

        new Camera camera;
        bool isOpen = false;

        // Interactables
        IOpenable openInteractable;

        void Awake()
        {
            camera = Camera.main;
            openInteractable = new OpenInteractable("Mouse X", "Mouse Y");
        }

        #region ABSTRACTION
        public void OpenClose()
        {
            if (isDrawer)
            {
                StartCoroutine(OpeningOrClosingDrawer(transform.localPosition.z == amount ? 0 : amount, duration, transform));
            }
            else
            {
                StartCoroutine(OpeningOrClosingDoor(degrees, duration, transform));
            }
        }

        IEnumerator OpeningOrClosingDrawer(float amount, float duration, Transform transform)
        {
            if (transform.localPosition.z != amount)
            {
                openInteractable.OpenDrawer(amount, duration, transform);
                yield return null;
            }
            else
            {
                transform.localPosition = new Vector3(0, 0, amount);
            }
        }

        IEnumerator OpeningOrClosingDoor(float degrees, float duration, Transform transform)
        {
            if (!isOpen)
                openInteractable.Open(degrees, duration, transform, rotationAxis);
            else
                openInteractable.Open(0, duration, transform, rotationAxis);

            isOpen = !isOpen;
            yield return null;
        }
        #endregion
    }
}