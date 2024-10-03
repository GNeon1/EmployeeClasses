using EmployeeClasses.Roles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeClasses.Patches
{
    [HarmonyPatch]
    internal class ShovelPatch
    {
        [HarmonyPatch(typeof(Shovel), "HitShovel")]
        [HarmonyPrefix]
        private static void HitShovelPatch(ref int ___shovelHitForce)
        {
            ___shovelHitForce = RoleManager.Instance.attackDamage;
        }
    }
}
