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

namespace EmployeeClasses.Patches
{
    internal class HUDManagerPatch
    {
        [HarmonyPatch(typeof(HUDManager), "UpdateHealthUI")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> UpdateHealthUIPatch(IEnumerable<CodeInstruction> instructions)
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
    }
}
