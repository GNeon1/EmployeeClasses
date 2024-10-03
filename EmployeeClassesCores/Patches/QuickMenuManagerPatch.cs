using EmployeeClasses.GUI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace EmployeeClasses.Patches
{
    [HarmonyPatch]
    internal class QuickMenuManagerPatch
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(QuickMenuManager), "OpenQuickMenu")]
        public static void GUIAwake()
        {
            if (RolesGUI.instance == null)
                new RolesGUI();
            RolesGUI.instance.OpenMenu();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(QuickMenuManager), "CloseQuickMenu")]
        public static void GUIAsleep()
        {
            RolesGUI.instance.CloseMenu();
        }
    }
}
