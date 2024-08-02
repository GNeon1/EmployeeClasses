using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using EmployeeClasses.Roles;
using EmployeeClasses.Patches;
using UnityEngine;
using System.IO;
using System.Reflection;
using EmployeeClasses.Inputs;
using UnityEngine.InputSystem;
using EmployeeClasses.Util;
using Unity.Netcode;

namespace EmployeeClasses
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    public class ModBase : BaseUnityPlugin
    {
        private const string modGUID = "Jade.EmployeeClasses";
        private const string modName = "EmployeeClasses";
        private const string modVersion = "0.1.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ModBase Instance;

        public static ManualLogSource als;

        public static AssetBundle assets;

        internal static Keybinds keybinds;

        void Awake()
        {
            if (Instance == null)
                Instance = this;

            als = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            assets = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "employeeclasses"));
            if (assets == null)
                als.LogError("Failed to load custom assets.");

            keybinds = new Keybinds();
            SetupKeybindCallbacks();

            Sounds.Awake();

            harmony.PatchAll(typeof(ModBase));
            harmony.PatchAll(typeof(RoleManager));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(QuickMenuManagerPatch));
            harmony.PatchAll(typeof(ShovelPatch));
            harmony.PatchAll(typeof(HUDManagerPatch));
            harmony.PatchAll(typeof(PlayerControllerBPAtch));
            harmony.PatchAll(typeof(GameNetworkManagerPatch));
            harmony.PatchAll(typeof(TimeOfDayPatch));

            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }

            als.LogInfo(":");
            als.LogInfo("EmployeeClasses has loaded!");
            als.LogInfo(":");

            //new RoleManager();
        }

        public void SetupKeybindCallbacks() {
            keybinds.SkillKey.canceled += OnSkillKeyReleased;
            keybinds.SkillKey.performed += OnSkillKeyPressed;
        }

        public void OnSkillKeyPressed(InputAction.CallbackContext context)
        {
            RoleManager.Instance.PressSkill();
        }

        public void OnSkillKeyReleased(InputAction.CallbackContext context) {
            RoleManager.Instance.ReleaseSkill();
        }

        public static void logOutput(String msg) {
            als.LogInfo(msg);
        }
    }
}
