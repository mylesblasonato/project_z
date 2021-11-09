using Fungus;
using Game.ProjectZ;
using MDI.Gamekit.Core;
using MDI.Gamekit.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamineController : MonoBehaviour
{
    #region SFIELDS
    [SerializeField] Flowchart flowchart;
    [SerializeField] string flowExamineBlockName;
    [SerializeField] string floorObjectName = "Floor";
    #endregion

    // Interactable
    IExaminable examineInteractable;

    void Awake()
    {
        examineInteractable = new ExamineInteractable(flowchart);
    }

    #region ABSTRACTION
    public void Examine()
    {
        examineInteractable.Examine(flowExamineBlockName);
    }

    public void HoverOn()
    {

    }

    public void HoverOut()
    {

    }

    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (string.Compare(collision.gameObject.name, floorObjectName) == 0)
        {
            transform.parent = collision.gameObject.transform;
        }
    }
    #endregion
}
