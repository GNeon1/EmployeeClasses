using EmployeeClasses.Roles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeClasses.Patches
{
    internal class RoundManagerPatch
    {
        [HarmonyPatch(typeof(RoundManager), "SetSteamValveTimes")]
        [HarmonyPostfix]
        public static void SteamValvePatch()
        {
            SteamValveHazard[] array = UnityEngine.Object.FindObjectsOfType<SteamValveHazard>();
            foreach (SteamValveHazard hazard in array)
            {
                hazard.transform.parent.Find("FogZoneContainer/FogZone").GetComponent<UnityEngine.Rendering.HighDefinition.LocalVolumetricFog>().parameters.distanceFadeEnd *= RoleManager.Instance.fogVisibility;
            }
        }
    }
}
