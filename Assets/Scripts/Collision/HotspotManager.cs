using Fungus;
using MDI.Gamekit.Core;
using UnityEngine;

namespace Game.ProjectZ
{
    public class HotspotManager : MonoSingleton<HotspotManager>
    {
        [SerializeField] Flowchart[] yearFlows;
        [SerializeField] string yearFlowBlockToExecute = "Speech";
        [SerializeField] Transform[] years;
        [SerializeField] SEvent OnNoMoreRemaining;
        [SerializeField] int currentYear = 0;

        bool hasRemaining = true;
        HotspotController[] controllers;

        void Awake()
        {
            CreateSingleton(this, () => Destroy(this.gameObject));
        }

        public void PoloroidsRemaining()
        {          
            foreach (HotspotController hotspot in controllers)
            {
                if (hotspot.gameObject.activeSelf)
                {
                    hasRemaining = true;
                    break;
                }
                else
                {
                    hasRemaining = false;
                }
            }

            if (!hasRemaining)
            {
                foreach (Transform hotspot in years[currentYear])
                    Destroy(hotspot.gameObject);
                yearFlows[currentYear].ExecuteBlock(yearFlowBlockToExecute);
                currentYear++;
                ActivateYear(currentYear);
            }
        }

        public void ActivateYear(int yearIndex)
        {
            hasRemaining = true;
            foreach (var year in years)
                year.gameObject.SetActive(false);
            years[yearIndex].gameObject.SetActive(true);
            controllers = years[yearIndex].GetComponentsInChildren<HotspotController>();
        }
    }
}
