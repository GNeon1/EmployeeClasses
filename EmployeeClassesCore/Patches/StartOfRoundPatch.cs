using EmployeeClasses.Roles;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using EmployeeClasses.Util;
using UnityEngine.SceneManagement;
using EmployeeClasses.GUI;

namespace EmployeeClasses.Patches
{
    internal class StartOfRoundPatch
    {
        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        [HarmonyPostfix]
        public static void AwakePatch(ref StartOfRound __instance)
        {
            RolesGUI.instance = null;
            foreach (PlayerControllerB player in __instance.allPlayerScripts)
            {
                player.gameObject.AddComponent<RoleManager>();
            }
            //__instance.NetworkObject.OnSpawn(CreateNetworkManager);
        }

        private static void CreateNetworkManager()
        {
            if (StartOfRound.Instance.IsServer || StartOfRound.Instance.IsHost)
            {
                if (RoleManager.Instance == null)
                {
                    GameObject rmInstance = GameObject.Instantiate<GameObject>(ModBase.assets.LoadAsset<GameObject>("RoleManager"));
                    rmInstance.AddComponent<RoleManager>();
                    SceneManager.MoveGameObjectToScene(rmInstance, StartOfRound.Instance.gameObject.scene);
                    rmInstance.GetComponent<NetworkObject>().Spawn();
                    //GameNetworkManager.Instance.GetComponent<NetworkManager>().AddNetworkPrefab(rmInstance);
                    ModBase.als.LogInfo($"Created RoleManager. Scene is: '{rmInstance.scene.name}'");
                }
                else
                {
                    ModBase.als.LogWarning("RoleManager already exists?");
                }
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "ReviveDeadPlayers")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ReviveDeadPlayersTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            //IL_007c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0086: Expected O, but got Unknown
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            MethodInfo method = typeof(RoleManager).GetMethod("GetMaxHealth");
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].opcode == OpCodes.Ldc_I4_S && list[i].operand.ToString() == "100")
                {
                    list.Insert(i + 1, new CodeInstruction(OpCodes.Call, (object)method));
                }
            }
            return list.AsEnumerable();
        }

        [HarmonyPatch(typeof(StartOfRound), "ReviveDeadPlayers")]
        [HarmonyPostfix]
        public static void ReviveDeadPlayersPatch()
        {
            RoleManager.Instance.Revive();
        }

        [HarmonyPatch(typeof(StartOfRound), "Update")]
        [HarmonyPostfix]
        public static void UpdatePatch()
        {
            if (RoleManager.Instance == null) return;
            RoleManager.Instance.UpdateIt();
        }
    }
}
