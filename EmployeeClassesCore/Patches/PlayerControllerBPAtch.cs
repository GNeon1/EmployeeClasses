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
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using Unity.Netcode;

namespace EmployeeClasses.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPAtch
    {
        [HarmonyPatch(typeof(PlayerControllerB), "Awake")]
        [HarmonyPrefix]
        public static void AwakePatch(PlayerControllerB __instance)
        {
            GameObject lightContainer = new GameObject("BeaconLight");
            lightContainer.transform.parent = __instance.transform;
            lightContainer.transform.localPosition = new Vector3(0, 2.6f, 0);
            Light light = lightContainer.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.2f, 0.8f, 0.2f);
            light.intensity = 3;
            light.range = 3;
            lightContainer.SetActive(false);

            __instance.usernameBillboardText.verticalAlignment = TMPro.VerticalAlignmentOptions.Bottom;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        public static void LocalPlayer(ref PlayerControllerB __instance)
        {
            RoleManager.Instance = __instance.gameObject.GetComponent<RoleManager>();
            RoleManager.Instance.CreateHUD();
            RoleManager.Instance.reset();
            RoleManager.Instance.SyncRoles();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SpoofSprintMultiplier(IEnumerable<CodeInstruction> instructions)
        {
            //IL_007c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0086: Expected O, but got Unknown
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            FieldInfo field = typeof(PlayerControllerB).GetField("sprintMultiplier", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo method = typeof(RoleManager).GetMethod("GetSprintMultiplier", BindingFlags.Static | BindingFlags.Public);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].opcode == OpCodes.Ldfld && (FieldInfo)list[i].operand == field)
                {
                    list[i] = new CodeInstruction(OpCodes.Call, (object)method);
                    list.RemoveAt(i - 1);
                }
            }
            return list.AsEnumerable();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SpoofSprintMeter(IEnumerable<CodeInstruction> instructions)
        {
            //IL_007c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0086: Expected O, but got Unknown
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            FieldInfo field = typeof(PlayerControllerB).GetField("sprintMeter", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo method = typeof(RoleManager).GetMethod("GetSprintMeter", BindingFlags.Static | BindingFlags.Public);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].opcode == OpCodes.Ldfld && (FieldInfo)list[i].operand == field)
                {
                    list[i] = new CodeInstruction(OpCodes.Call, (object)method);
                    list.RemoveAt(i - 1);
                }
            }
            return list.AsEnumerable();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SpoofSprintMeterLate(IEnumerable<CodeInstruction> instructions)
        {
            //IL_007c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0086: Expected O, but got Unknown
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            FieldInfo field = typeof(PlayerControllerB).GetField("sprintMeter", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo method = typeof(RoleManager).GetMethod("GetSprintMeter", BindingFlags.Static | BindingFlags.Public);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].opcode == OpCodes.Ldfld && (FieldInfo)list[i].operand == field)
                {
                    list[i] = new CodeInstruction(OpCodes.Call, (object)method);
                    list.RemoveAt(i - 1);
                }
            }
            return list.AsEnumerable();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        [HarmonyPostfix]
        public static void CorrectMeterUI(PlayerControllerB __instance, bool ___isPlayerControlled, bool ___isPlayerDead, Image ___sprintMeterUI, float ___drunkness, ref bool ___isExhausted, bool ___isSprinting, bool ___isWalking, int ___isMovementHindered, float ___sprintTime)
        {
            if (__instance.IsOwner && (!__instance.IsServer || __instance.isHostPlayerObject))
            {
                if (___isPlayerControlled && !___isPlayerDead)
                {
                    float sprintMeter = RoleManager.Instance.sprintMeter;

                    float sprintTime = ___sprintTime;
                    float num3 = 1f;

                    if (___drunkness > 0.02f)
                    {
                        num3 *= Mathf.Abs(StartOfRound.Instance.drunknessSpeedEffect.Evaluate(___drunkness) - 1.25f);
                    }

                    if (___isSprinting)
                    {
                        sprintMeter = Mathf.Clamp(sprintMeter - Time.deltaTime / (sprintTime * RoleManager.Instance.sprintTime) * RoleManager.GetCarryWeight(0) * num3, 0f, 1f);
                    }
                    else if (___isMovementHindered > 0)
                    {
                        if (___isWalking)
                            sprintMeter = Mathf.Clamp(sprintMeter - Time.deltaTime / (sprintTime * RoleManager.Instance.sprintTime) * num3 * 0.5f, 0f, 1f);
                    }
                    else
                    {
                        if (!___isWalking)
                        {
                            sprintMeter = Mathf.Clamp(sprintMeter + Time.deltaTime / ((sprintTime * RoleManager.Instance.sprintTime) + 4f) * num3, 0f, 1f);
                        }
                        else
                        {
                            sprintMeter = Mathf.Clamp(sprintMeter + Time.deltaTime / ((sprintTime * RoleManager.Instance.sprintTime) + 9f) * num3, 0f, 1f);
                        }
                    }

                    RoleManager.Instance.sprintMeter = sprintMeter;
                    ___sprintMeterUI.fillAmount = sprintMeter;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPrefix]
        public static void CalculateCarryWeight(float ___carryWeight)
        {
            if (RoleManager.Instance == null) return;
            if (Mathf.RoundToInt(Mathf.Clamp(___carryWeight - 1f, 0f, 100f) * 105f) > RoleManager.Instance.weightPenaltyThreshhold)
                RoleManager.Instance.carryWeight = (___carryWeight - 1) * RoleManager.Instance.weightPenalty + 1;
            else
                RoleManager.Instance.carryWeight = ___carryWeight;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPatch(typeof(PlayerControllerB), "IVisibleThreat.GetInterestLevel")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SpoofCarryWeight(IEnumerable<CodeInstruction> instructions)
        {
            //IL_007c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0086: Expected O, but got Unknown
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            FieldInfo field = typeof(PlayerControllerB).GetField("carryWeight", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo method = typeof(RoleManager).GetMethod("GetCarryWeight");
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].opcode == OpCodes.Ldfld && (FieldInfo)list[i].operand == field)
                {
                    list.Insert(i + 1, new CodeInstruction(OpCodes.Call, (object)method));
                }
            }
            return list.AsEnumerable();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPrefix]
        public static void Update(PlayerControllerB __instance)
        {
            if (RoleManager.Instance == null) return;
            if (__instance.IsOwner && __instance.isPlayerControlled)
            {
                if (__instance.isSprinting)
                {
                    RoleManager.Instance.sprintMultiplier = Mathf.Lerp(RoleManager.Instance.sprintMultiplier, RoleManager.Instance.maxSprintSpeed, Time.deltaTime * 1f);
                }
                else
                {
                    RoleManager.Instance.sprintMultiplier = Mathf.Lerp(RoleManager.Instance.sprintMultiplier, 1f, 10f * Time.deltaTime);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "SetFaceUnderwaterFilters")]
        [HarmonyPostfix]
        public static void DrowningPostfix(ref bool ___isPlayerDead, ref bool ___isUnderwater, ref Collider ___underwaterCollider, ref Camera ___gameplayCamera)
        {
            if (___isPlayerDead || RoleManager.Instance == null)
                return;
            if (___isUnderwater && ___underwaterCollider != null && ___underwaterCollider.bounds.Contains(___gameplayCamera.transform.position))
            {
                RoleManager.Instance.drowningTimer -= Time.deltaTime / 10f;
            }
            else
                RoleManager.Instance.drowningTimer = Mathf.Clamp(RoleManager.Instance.drowningTimer + Time.deltaTime, 0.1f, RoleManager.Instance.oxygenReserves);
            StartOfRound.Instance.drowningTimer = RoleManager.Instance.drowningTimer + (Time.deltaTime / 10f);
            if (RoleManager.Instance.drowningTimer < 0f)
            {
                RoleManager.Instance.drowningTimer = RoleManager.Instance.oxygenReserves;
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
        [HarmonyPrefix]
        public static void DamagePrefix(ref int __0, bool ___takingFallDamage, CauseOfDeath causeOfDeath)
        {
            if (___takingFallDamage)
            {
                __0 = (int)(__0 * RoleManager.Instance.fallDamage);
            }
            if (causeOfDeath == CauseOfDeath.Gunshots)
            {
                __0 = (int)(__0 * RoleManager.Instance.bulletDamage);
            }
            RoleManager.Instance.stimmed = false;

            if (__0 >= 10)
                RoleManager.Instance.sprintMeter = Mathf.Clamp(RoleManager.Instance.sprintMeter + (float)__0 / 125f, 0f, 1f);
        }

        [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> DamagePlayerTranspiler(IEnumerable<CodeInstruction> instructions)
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

        [HarmonyPatch(typeof(PlayerControllerB), "PlayFootstepSound")]
        [HarmonyPrefix]
        public static bool PlayFootstepSound(ref PlayerControllerB __instance, int ___previousFootstepClip)
        {
            __instance.GetCurrentMaterialStandingOn();
            int num = UnityEngine.Random.Range(0, StartOfRound.Instance.footstepSurfaces[__instance.currentFootstepSurfaceIndex].clips.Length);
            if (num == ___previousFootstepClip)
            {
                num = (num + 1) % StartOfRound.Instance.footstepSurfaces[__instance.currentFootstepSurfaceIndex].clips.Length;
            }
            __instance.movementAudio.pitch = UnityEngine.Random.Range(0.93f, 1.07f);
            bool flag = ((!__instance.IsOwner) ? __instance.playerBodyAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Sprinting") : __instance.isSprinting);
            float num2 = __instance.GetComponent<RoleManager>().GetFootstepVolume(0.9f);
            if (!flag)
            {
                num2 = __instance.GetComponent<RoleManager>().GetFootstepVolume(0.6f);
            }
            __instance.movementAudio.PlayOneShot(StartOfRound.Instance.footstepSurfaces[__instance.currentFootstepSurfaceIndex].clips[num], num2);
            ___previousFootstepClip = num;
            WalkieTalkie.TransmitOneShotAudio(__instance.movementAudio, StartOfRound.Instance.footstepSurfaces[__instance.currentFootstepSurfaceIndex].clips[num], num2);
            return false;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "PlayFootstepServer")]
        [HarmonyPrefix]
        public static bool PlayFootstepServer(ref PlayerControllerB __instance)
        {
            if (!__instance.isClimbingLadder && !__instance.inSpecialInteractAnimation && !__instance.IsOwner && __instance.isPlayerControlled)
            {
                bool noiseIsInsideClosedShip = __instance.isInHangarShipRoom && __instance.playersManager.hangarDoorsClosed;
                if (__instance.isSprinting)
                {
                    RoundManager.Instance.PlayAudibleNoise(__instance.transform.position, 22f, __instance.GetComponent<RoleManager>().GetFootstepVolume(0.6f), 0, noiseIsInsideClosedShip, 7);
                }
                else
                {
                    RoundManager.Instance.PlayAudibleNoise(__instance.transform.position, 17f, __instance.GetComponent<RoleManager>().GetFootstepVolume(0.4f), 0, noiseIsInsideClosedShip, 7);
                }
                __instance.PlayFootstepSound();
            }
            return false;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "PlayFootstepLocal")]
        [HarmonyPrefix]
        public static bool PlayFootstepLocal(ref PlayerControllerB __instance)
        {
            if (!__instance.isClimbingLadder && !__instance.inSpecialInteractAnimation && (__instance.isTestingPlayer || (__instance.IsOwner && __instance.isPlayerControlled)))
            {
                bool noiseIsInsideClosedShip = __instance.isInHangarShipRoom && __instance.playersManager.hangarDoorsClosed;
                if (__instance.isSprinting)
                {
                    RoundManager.Instance.PlayAudibleNoise(__instance.transform.position, 22f, __instance.GetComponent<RoleManager>().GetFootstepVolume(0.6f), 0, noiseIsInsideClosedShip, 7);
                }
                else
                {
                    RoundManager.Instance.PlayAudibleNoise(__instance.transform.position, 17f, __instance.GetComponent<RoleManager>().GetFootstepVolume(0.4f), 0, noiseIsInsideClosedShip, 7);
                }
                __instance.PlayFootstepSound();
            }
            return false;
        }

        [HarmonyPatch(typeof(HUDManager), "AssignNewNodes")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SpoofScanRange(IEnumerable<CodeInstruction> instructions)
        {
            //IL_007c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0086: Expected O, but got Unknown
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            MethodInfo method = typeof(RoleManager).GetMethod("GetScanRange");
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].opcode == OpCodes.Ldc_R4)
                {
                    list.Insert(i + 1, new CodeInstruction(OpCodes.Call, (object)method));
                }
            }
            return list.AsEnumerable();
        }

        [HarmonyPatch(typeof(HUDManager), "MeetsScanNodeRequirements")]
        [HarmonyPrefix]
        public static void MeetsScanNodeRequirements(ref ScanNodeProperties __0)
        {
            __0.maxRange = (int)(__0.maxRange * RoleManager.Instance.scanRange);
            __0.requiresLineOfSight = !RoleManager.Instance.scanWall;

        }

        [HarmonyPatch(typeof(HUDManager), "MeetsScanNodeRequirements")]
        [HarmonyPostfix]
        public static void MeetsScanNodeRequirementsUndo(ref ScanNodeProperties __0)
        {
            __0.maxRange = (int)(__0.maxRange / RoleManager.Instance.scanRange);
        }

        [HarmonyPatch(typeof(PlayerControllerB), "IVisibleThreat.GetThreatLevel")]
        [HarmonyPostfix]
        public static void SpoofThreatLevel(ref int __result)
        {
            __result += RoleManager.GetThreatLevel();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Jump_performed")]
        [HarmonyPrefix]
        public static void JumpPatch(PlayerControllerB __instance, bool ___isJumping, float ___playerSlidingTimer)
        {
            if (!__instance.quickMenuManager.isMenuOpen && ((__instance.IsOwner && __instance.isPlayerControlled && (!__instance.IsServer || __instance.isHostPlayerObject)) || __instance.isTestingPlayer) && !__instance.inSpecialInteractAnimation && !__instance.isTypingChat && (__instance.isMovementHindered <= 0 || __instance.isUnderwater) && !__instance.isExhausted && (__instance.thisController.isGrounded || (!___isJumping && __instance.IsPlayerNearGround())) && !___isJumping && (!__instance.isPlayerSliding || ___playerSlidingTimer > 2.5f) && !__instance.isCrouching)
            {
                RoleManager.Instance.sprintMeter = Mathf.Clamp(RoleManager.Instance.sprintMeter - 0.08f, 0f, 1f);
            }
        }

    }
}
