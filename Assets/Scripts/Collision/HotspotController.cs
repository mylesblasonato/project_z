using Fungus;
using UnityEngine;

namespace Game.ProjectZ
{
    public class HotspotController : MonoBehaviour
    {
        [SerializeField] PlayerInteractor playerInteractor;
        [SerializeField] GameObject interactable;
        [SerializeField] Flowchart flowchart;
        [SerializeField] string blockName = "Detection";
        [SerializeField] bool onlyPlayOnce = true;

        MeshRenderer meshRenderer;
        new Collider collider;
        bool hasTriggered = false;

        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            collider = GetComponent<Collider>();
        }

        void Update()
        {
            if (playerInteractor.ActiveInteractable == null) return;
            if (playerInteractor.gameObject != interactable) return;
            collider.enabled = true;
            hasTriggered = false;
            meshRenderer.enabled = true;
        }

        void OnTriggerStay(Collider other)
        {
            if (other.gameObject == interactable && !hasTriggered)
            {
                hasTriggered = true;
                collider.enabled = false;
                flowchart.transform.parent = transform.parent;
                flowchart.ExecuteBlock(blockName);
                if (onlyPlayOnce)
                {
                    gameObject.SetActive(false);
                }
                else
                    meshRenderer.enabled = false;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject == interactable && !onlyPlayOnce)
            {
                if (playerInteractor.ActiveInteractable.gameObject == interactable)
                {
                    collider.enabled = true;
                    hasTriggered = false;
                    meshRenderer.enabled = true;
                }
            }
        }
    }
}