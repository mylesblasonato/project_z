using Fungus;
using MDI.Gamekit.Interactables;
using UnityEngine;
using MDI.Gamekit.Core;

namespace Game.ProjectZ
{
    public class InspectController : MonoBehaviour
    {
        #region SFIELDS
        [SerializeField] string inspectAxis = "Inspect";
        [SerializeField] float inspectDistance = 1f;
        [SerializeField] float inspectRotationSpeed = 1f;
        [SerializeField] Transform inspectOffset;
        #endregion

        PlayerController playerController;

        // Interactables
        IInspectable inspectInteractable;

        void Awake()
        {
            inspectInteractable = new InspectInteractable();
        }

        #region ABSTRACTION
        public void Inspect()
        {
            playerController.TurnCameraFollowOff(playerController.GetCameraBehaviour());
            inspectInteractable.Inspect();
        }

        public void Rotate()
        {
            CursorManager.Instance.SetInteractableName("");
            CursorManager.Instance.ChangeMouseRadius(20f, 1f, name.ToUpper(), "");
            playerController.TurnCameraFollowOff(playerController.GetCameraBehaviour());
            var pos = new Vector2(-InputManager.Instance.GetAxis("Mouse Y"), -InputManager.Instance.GetAxis("Mouse X"));
            inspectInteractable.Rotate(inspectRotationSpeed, transform, pos);
        }

        public void Release()
        {
            CursorManager.Instance.ChangeMouseRadius(20f, 1f, name.ToUpper(), "INTERACT [A]\nINSPECT [R1/R3]");
            playerController.TurnCameraFollowOn(playerController.GetCameraBehaviour());
            inspectInteractable.Release();
        }
        #endregion
    }
}