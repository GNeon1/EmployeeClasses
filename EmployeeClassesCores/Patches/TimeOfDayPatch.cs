using EmployeeClasses.GUI;
using EmployeeClasses.Roles;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeClasses.Patches
{
    internal class TimeOfDayPatch
    {
        [HarmonyPatch(typeof(TimeOfDay), "SetInsideLightingDimness")]
        [HarmonyPostfix]
        public static void SetInsideLightingDimnessPatch()
        {
            if (RoleManager.Instance.alwaysClock && StartOfRound.Instance.localPlayerController.isCrouching)
                HUDManager.Instance.SetClockVisible(true);
        }
    }
}
