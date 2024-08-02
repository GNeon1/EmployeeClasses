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
using UnityEngine.PlayerLoop;
using System.Runtime.CompilerServices;
using BepInEx;
using EmployeeClasses.Util;
using EmployeeClasses.GUI;
using UnityEngine.Experimental.GlobalIllumination;
using Unity.Netcode;
using System.Collections;
using EmployeeClasses.Inputs;
using DunGen;
using System.Net.NetworkInformation;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using JetBrains.Annotations;

namespace EmployeeClasses.Roles
{
    internal class RoleManager : NetworkBehaviour
    {
        public string selectedRole;
        public List<RoleTag> tags;
        public string ability;

        public float maxSprintSpeed = 2.25f;
        public float weightPenalty = 1f;
        public float oxygenReserves = 1f;
        private float speedMultiplier = 1f;
        public float fallDamage = 1f;
        public int attackDamage = 1;
        public float footstepVolume = 1f;
        public float scanRange = 1f;
        private int maxHealth = 100;
        public bool scanWall = false;
        private int threatLevel = 0;
        public int weightPenaltyThreshhold = 0;
        public float bulletDamage = 0;

        public float sprintTime = 1;
        public float sprintMeter = 1;

        public float battery = 1;

        public bool alwaysClock = false;

        public float sprintMultiplier = 0.5f;
        public float drowningTimer = 1f;

        public float extract = 0;
        public int charges = 0;
        public bool freeCharge = true;

        public bool stimmed = false;

        GameObject headlamp;

        private float abilityCooldown = 0;
        private float duration = 0;
        private bool active = false;
        private Coroutine abilityRoutine;

        private PlayerControllerB player;

        public float carryWeight;

        public static RoleManager Instance;

        public int playerID;

        float holdTimer;
        float second = 0;

        Image extractBar;
        Image batteryBar;
        Image chargeBar;

        Dictionary<string, int> extractValues = new Dictionary<string, int>() {
            { "Flowerman", 75 },
            { "Crawler", 50 },
            { "Hoarding bug", 30 },
            { "Centipede", 20 },
            { "Bunker Spider", 60 },
            { "Puffer", 40 },
            { "Nutcracker", 60 },
            { "Masked", 50 },
            { "Butler", 60 },
            { "Clay Surgeon", 50 },
            { "MouthDog", 100 },
            { "ForestGiant", 100 },
            { "Baboon hawk", 50 },
            { "Bush Wolf", 50 },
            { "Manticoil", 20 },
            { "Tulip Snake", 20 }};

        Dictionary<LevelWeatherType, float> batteryRates = new Dictionary<LevelWeatherType, float>() {
            { LevelWeatherType.None, 0.09f },
            { LevelWeatherType.Stormy, 0.1f },
            { LevelWeatherType.Rainy, 0.07f },
            { LevelWeatherType.Eclipsed, 0.2f },
            { LevelWeatherType.Foggy, 0.05f },
            { LevelWeatherType.DustClouds, 0.08f }
        };

        public void Awake()
        {

        }

        public void CreateHUD()
        {
            extractBar = GameObject.Instantiate<GameObject>(ModBase.assets.LoadAsset<GameObject>("ExtractBar"), GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner").transform).transform.Find("Bar").GetComponent<Image>();
            extractBar.transform.parent.gameObject.SetActive(false);
            chargeBar = extractBar.transform.parent.Find("Charges").GetComponent<Image>();

            batteryBar = GameObject.Instantiate<GameObject>(ModBase.assets.LoadAsset<GameObject>("BatteryBar"), GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner").transform).transform.Find("Bar").GetComponent<Image>();
            batteryBar.transform.parent.gameObject.SetActive(false);
        }

        public void reset()
        {
            if (headlamp != null)
                DestroyHeadlamp();

            maxSprintSpeed = 2.25f;
            maxHealth = 100;
            weightPenalty = 1f;
            oxygenReserves = 1f;
            speedMultiplier = 1f;
            fallDamage = 1f;
            attackDamage = 1;
            footstepVolume = 1f;
            scanRange = 1f;
            threatLevel = 0;
            weightPenaltyThreshhold = 0;
            bulletDamage = 1;
            alwaysClock = false;
            scanWall = false;
            holdTimer = -1;
            stimmed = false;
            sprintTime = 1;
            if (charges == 0)
                charges = 1;

            battery = 1f;
        }

        public void changeRole(Role role)
        {
            reset();
            tags = role.tags;
            ability = role.ability == null ? null : role.ability;
            selectedRole = role.name;

            foreach (RoleTag tag in tags.ToArray())
            {
                if (tag.Id == (int)RoleTag.Ids.SPRINT_MULTIPLIER)
                {
                    maxSprintSpeed *= tag.Power;
                }
                else if (tag.Id == (int)RoleTag.Ids.MAX_HEALTH)
                {
                    maxHealth = (int)tag.Power;
                }
                else if (tag.Id == (int)RoleTag.Ids.WEIGHT_PENALTY)
                {
                    weightPenalty = tag.Power;
                }
                else if (tag.Id == (int)RoleTag.Ids.OXYGEN)
                {
                    oxygenReserves = tag.Power;
                }
                else if (tag.Id == (int)RoleTag.Ids.SPEED_MULTIPLIER)
                {
                    speedMultiplier = tag.Power;
                }
                else if (tag.Id == (int)RoleTag.Ids.FALL_DAMAGE)
                {
                    fallDamage = tag.Power;
                }
                else if (tag.Id == (int)RoleTag.Ids.ATTACK_DAMAGE)
                {
                    attackDamage = (int)tag.Power;
                }
                else if (tag.Id == (int)RoleTag.Ids.WEIGHT_PENALTY_THRESH)
                {
                    weightPenaltyThreshhold = (int)tag.Power;
                }
                else if (tag.Id == (int)RoleTag.Ids.FOOTSTEP_VOLUME)
                {
                    footstepVolume = tag.Power;
                }
                else if (tag.Id == (int)RoleTag.Ids.SCAN_RANGE)
                {
                    scanRange = tag.Power;
                }
                else if (tag.Id == (int)RoleTag.Ids.SCAN_WALL)
                {
                    scanWall = true;
                }
                else if (tag.Id == (int)RoleTag.Ids.THREAT_LEVEL)
                {
                    threatLevel = (int)tag.Power;
                }
                else if (tag.Id == (int)RoleTag.Ids.BULLET_DAMAGE)
                {
                    bulletDamage = tag.Power;
                }
                else if (tag.Id == (int)RoleTag.Ids.ALWAYS_CLOCK)
                {
                    alwaysClock = true;
                }
                else if (tag.Id == (int)RoleTag.Ids.HEADLAMP)
                {
                    CreateHeadlamp();
                }
                else if (tag.Id == (int)RoleTag.Ids.SPRINT_TIME)
                {
                    sprintTime = tag.Power;
                }
            }

            if (ability == "STIM")
                extractBar.transform.parent.gameObject.SetActive(true);
            else
                extractBar.transform.parent.gameObject.SetActive(false);
        }

        public void applyRole(PlayerControllerB instance)
        {
            instance.health = maxHealth;
            instance.movementSpeed = 4.6f * speedMultiplier;
            drowningTimer = oxygenReserves;
            abilityCooldown = 0f;
            duration = 0f;
            active = false;

            if (NetworkManager.IsHost)
                ApplyRoleClientRpc(playerID, footstepVolume);
            else
                ApplyRoleServerRpc(playerID, footstepVolume);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ApplyRoleServerRpc(int id, float footstepVolume)
        {
            ApplyRoleClientRpc(id, footstepVolume);
        }

        [ClientRpc]
        public void ApplyRoleClientRpc(int id, float footstepVolume)
        {
            StartOfRound.Instance.allPlayerScripts[id].GetComponent<RoleManager>().footstepVolume = footstepVolume;
        }

        public static float GetScanRange(float range)
        {
            return range * RoleManager.Instance.scanRange;
        }

        public float GetFootstepVolume(float volume)
        {
            return volume * RoleManager.Instance.footstepVolume;
        }

        public static int GetThreatLevel()
        {
            return RoleManager.Instance.threatLevel;
        }

        public static int GetMaxHealth(int health)
        {
            return RoleManager.Instance.maxHealth;
        }

        public static float GetSprintMultiplier()
        {
            return RoleManager.Instance.sprintMultiplier;
        }

        public static float GetSprintMeter()
        {
            return RoleManager.Instance.sprintMeter;
        }

        public static float GetCarryWeight(float weight)
        {
            return RoleManager.Instance.carryWeight;
        }

        public void Revive()
        {
            if (headlamp != null)
                headlamp.SetActive(false);
            player = StartOfRound.Instance.localPlayerController;
            EndSkill();
            abilityCooldown = 0;
            duration = 0;
            active = false;
            extract = 0;
            stimmed = false;
            battery = 1;
            sprintMeter = 1;
            if (!freeCharge && charges < 3)
                charges += 1;
            freeCharge = true;
        }

        public void Update()
        {
            if (player == null)
            {
                if (StartOfRound.Instance.localPlayerController == null)
                    return;
                else
                {
                    player = StartOfRound.Instance.localPlayerController;
                    playerID = Array.IndexOf(StartOfRound.Instance.allPlayerScripts, player);
                }
            }
        }

        public void UpdateIt()
        {

            if (StartOfRound.Instance.localPlayerController == null)
                return;

            if (RolesGUI.instance == null)
                return;

            second += Time.deltaTime;
            if (second >= 0.1f)
            {
                second = 0;
                if (stimmed)
                {
                    if (player.drunkness < 0.1)
                        player.drunkness = .1f;
                    if (player.health <= maxHealth)
                        player.health += maxHealth >= 100 ? maxHealth / 100 : 1;
                    if (player.health > maxHealth)
                    {
                        player.health = maxHealth;
                        stimmed = false;
                    }
                    HUDManager.Instance.UpdateHealthUI(player.health);
                }
            }

            if ((ability == null && headlamp == null) || (((!player.isPlayerControlled) || StartOfRound.Instance.inShipPhase) && !NetworkManager.IsHost))
            {
                RolesGUI.instance.HideTip();
                return;
            }

            List<string> messages = new List<string>();

            if (!player.isInsideFactory && battery < 1)
            {
                battery += batteryRates[TimeOfDay.Instance.currentLevelWeather] * Time.deltaTime;
                if (battery > 1)
                    battery = 1;
            }

            if (headlamp != null && player.isPlayerControlled)
            {
                messages.Add($"TOGGLE LAMP : [Press {ModBase.keybinds.SkillKey.GetBindingDisplayString()[0]}]");

                batteryBar.fillAmount = Mathf.Lerp(batteryBar.fillAmount, battery / 1f, Mathf.SmoothStep(0, 1, 0.1f));

                if (headlamp.activeSelf)
                {
                    battery -= 0.004f * Time.deltaTime;
                    if (battery <= 0)
                        SyncLamp(false);
                }
            }

            HoldSkill();

            if (duration <= 0 && ability != null)
            {
                if (abilityCooldown > -10)
                    abilityCooldown -= Time.deltaTime * 1;

                if (abilityCooldown > 0) {
                    messages.Add("" + (int)abilityCooldown);
                }
                else if (ability == "HACK")
                {
                    if (holdTimer > 0.5f)
                    {
                        string str = "" + Math.Round(3 - holdTimer, 2);
                        if (str.Length < 4)
                            str += "0";
                        messages.Add($"HACKING : {str}");
                    }
                    else if (HackAvailable())
                        messages.Add($"HACK : [Hold {ModBase.keybinds.SkillKey.GetBindingDisplayString()[0]}]");
                }
                else if (ability == "STIM")
                {
                    extractBar.fillAmount = Mathf.Lerp(extractBar.fillAmount, extract / 100f, Mathf.SmoothStep(0, 1, 0.1f));
                    chargeBar.fillAmount = (float)charges / 3f;

                    if (HarvestAvailable())
                        messages.Add($"HARVEST : [Press {ModBase.keybinds.SkillKey.GetBindingDisplayString()[0]}]");
                    else if (SynthesizeAvailable() || StimAvailable())
                    {
                        if (StimAvailable())
                        {
                            if (StimmingTeam())
                                messages.Add($"STIM ALLY : [Press {ModBase.keybinds.SkillKey.GetBindingDisplayString()[0]}]");
                            else
                                messages.Add($"STIM SELF : [Press {ModBase.keybinds.SkillKey.GetBindingDisplayString()[0]}]");
                        }

                        if (SynthesizeAvailable())
                        {
                            messages.Add($"SYNTHESIZE : [Hold {ModBase.keybinds.SkillKey.GetBindingDisplayString()[0]}]");
                        }
                    }
                    else
                        RolesGUI.instance.HideTip();
                }
                else if (abilityCooldown <= 0)
                    messages.Add(ability + $": [Press {ModBase.keybinds.SkillKey.GetBindingDisplayString()[0]}]");
                    

                string txt = "";
                foreach (string s in messages) txt += s+"\n";

                RolesGUI.instance.ShowTip(txt,new Color(50, 50, 50));
            }
            else
            {
                UpdateSkill();
                duration -= Time.deltaTime * 1;
                if (duration <= 0)
                    EndSkill();
            }
        }

        public void PressSkill()
        {
            if (player.inTerminalMenu || player.isTypingChat)
                return;

            if (holdTimer == -1)
                holdTimer = 0;

            if (abilityCooldown < 0 && player.isPlayerControlled && !player.inTerminalMenu && (!StartOfRound.Instance.inShipPhase || StartOfRound.Instance.IsHost))
            {
                if (ability == "KICK")
                    TriggerKick();
                else
                    return;

                if (NetworkManager.Singleton.IsHost)
                    TriggerClientRpc(ability, playerID);
                else
                    TriggerServerRpc(ability, playerID);

                RolesGUI.instance.HideTip();
            }
        }

        public void HoldSkill()
        {
            if (player.inTerminalMenu || player.isTypingChat)
                return;

            if (holdTimer > -1)
                holdTimer += Time.deltaTime;

            if (holdTimer < 0.5f)
                return;

            if (ability == "STIM" && SynthesizeAvailable())
                TriggerSynthesize();
            else if (ability == "HACK")
            {
                if (HackAvailable())
                {
                    RoundManager.Instance.PlayAudibleNoise(player.transform.position, 30f, 1, 0, false);
                    if (holdTimer < 0.6f)
                    {
                        BeginHack();
                        holdTimer = 0.6f;
                        return;
                    }
                    else if (holdTimer > 3f)
                    {
                        TriggerHack();
                    }
                    else return;
                } else if (holdTimer >= 0.6f)
                {
                    StopItemAudio(playerID);
                    PlayClipFromItemAudio(9, playerID, 0.5f);
                    RoundManager.Instance.PlayAudibleNoise(player.transform.position, 30f, 1, 0, false);
                }
            }

            holdTimer = -2;
        }

        public void ReleaseSkill()
        {

            if (player.inTerminalMenu || player.isTypingChat)
                return;

            if (holdTimer == -2)
            {
                holdTimer = -1;
                return;
            } else if (ability == "HACK" && holdTimer >= 0.6f)
            {
                StopItemAudio(playerID);
                PlayClipFromItemAudio(9, playerID, 0.5f);
                RoundManager.Instance.PlayAudibleNoise(player.transform.position, 30f, 1, 0, false);
            }

            holdTimer = -1;

            if (headlamp != null && player.isPlayerControlled && !player.inTerminalMenu)
            {
                SyncLamp(!headlamp.activeSelf);
            }

            if (abilityCooldown < 0 && player.isPlayerControlled && !player.inTerminalMenu && (!StartOfRound.Instance.inShipPhase || StartOfRound.Instance.IsHost))
            {
                if (ability == "STIM")
                {
                    if (HarvestAvailable())
                        TriggerHarvest();
                    else if (StimAvailable())
                        TriggerStim();
                    else return;
                }
                else if (ability == "BEACON")
                    TriggerBeacon();
                else
                    return;

                if (NetworkManager.Singleton.IsHost)
                    TriggerClientRpc(ability, playerID);
                else
                    TriggerServerRpc(ability, playerID);

                RolesGUI.instance.HideTip();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void TriggerServerRpc(string ability, int playerID)
        {
            ModBase.als.LogInfo("SRpc is working");
            TriggerClientRpc(ability, playerID);
        }

        [ClientRpc]
        public void TriggerClientRpc(string ability, int playerID)
        {
            ModBase.als.LogInfo("CRpc is working");
            if (ability == "BEACON")
                TriggerBeaconClient(playerID);
        }

        public void EndSkill()
        {
            if (abilityRoutine != null)
                player.StopCoroutine(abilityRoutine);

            if (ability == "BEACON")
                EndBeacon();
            else if (ability == "KICK")
                EndKick();

            if (NetworkManager.Singleton.IsHost)
                EndClientRpc(ability, playerID);
            else
                EndServerRpc(ability, playerID);
        }

        [ServerRpc(RequireOwnership = false)]
        public void EndServerRpc(string ability, int playerID)
        {
            EndClientRpc(ability, playerID);
        }

        [ClientRpc]
        public void EndClientRpc(string ability, int playerID)
        {
            if (ability == "BEACON")
                EndBeaconClient(playerID);
        }

        public void UpdateSkill()
        {
            if (ability == "BEACON")
                UpdateBeacon();

            if (NetworkManager.Singleton.IsHost)
                UpdateClientRpc(ability, playerID);
            else
                UpdateServerRpc(ability, playerID);
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateServerRpc(string ability, int playerID)
        {
            UpdateClientRpc(ability, playerID);
        }

        [ClientRpc]
        public void UpdateClientRpc(string ability, int playerID)
        {
            if (ability == "BEACON")
                UpdateBeaconClient(playerID);
        }

        /////////////////////////////////
        //////////////////////////////
        // Utility

        public void PlayClipFromItemAudio(int clip, int playerID, float volume)
        {
            if (NetworkManager.Singleton.IsHost)
                PlayClipFromItemAudioClientRpc(clip, playerID, volume);
            else
                PlayClipFromItemAudioServerRpc(clip, playerID, volume);
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayClipFromItemAudioServerRpc(int clip, int playerID, float volume)
        {
            PlayClipFromItemAudioClientRpc(clip, playerID, volume);
        }

        [ClientRpc]
        public void PlayClipFromItemAudioClientRpc(int clip, int playerID, float volume)
        {
            Sounds.PlayClipFromSource(clip, StartOfRound.Instance.allPlayerScripts[playerID].itemAudio, volume);
        }

        public void StopItemAudio(int playerID)
        {
            if (NetworkManager.Singleton.IsHost)
                StopItemAudioClientRpc(playerID);
            else
                StopItemAudioServerRpc(playerID);
        }

        [ServerRpc(RequireOwnership = false)]
        public void StopItemAudioServerRpc(int playerID)
        {
            StopItemAudioClientRpc(playerID);
        }

        [ClientRpc]
        public void StopItemAudioClientRpc(int playerID)
        {
            StartOfRound.Instance.allPlayerScripts[playerID].itemAudio.Stop();
        }

        public void DestroyEnemy(int id)
        {
            if (NetworkManager.Singleton.IsHost)
                DestroyEnemyClientRpc(id);
            else
                DestroyEnemyServerRpc(id);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DestroyEnemyServerRpc(int id)
        {
            DestroyEnemyClientRpc(id);
        }

        [ClientRpc]
        public void DestroyEnemyClientRpc(int id)
        {
            GameObject.Destroy(RoundManager.Instance.SpawnedEnemies[id].gameObject);
        }

        /////////////////////////////////
        //////////////////////////////
        // BEACON

        public void TriggerBeacon()
        {
            ModBase.logOutput("Beacon activated!");

            duration = 5;
            abilityCooldown = 60;
        }

        public void TriggerBeaconClient(int playerID)
        {
            if (playerID == -1)
                return;
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerID];
            player.transform.Find("BeaconLight").gameObject.SetActive(true);
            PlayClipFromItemAudio(0, playerID, 1.2f);
            RoundManager.Instance.PlayAudibleNoise(player.transform.position, 80f, 3, 0, false);
        }

        public void UpdateBeacon()
        {
            player.sprintMeter = 100f;
        }

        public void UpdateBeaconClient(int playerID)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerID];
            foreach (EnemyAI ai in RoundManager.Instance.SpawnedEnemies)
            {
                if ((ai.targetPlayer != player || !ai.movingTowardsTargetPlayer) && Vector3.Distance(player.transform.position, ai.transform.position) <= 70)
                {
                    ModBase.als.LogInfo("target changed");
                    ai.targetPlayer = player;
                    ai.movingTowardsTargetPlayer = true;
                }
            }
            RoundManager.Instance.PlayAudibleNoise(player.transform.position, 80f, 3, 0, false);
        }

        public void EndBeacon()
        {
            ModBase.logOutput("Beacon ended!");
        }

        public void EndBeaconClient(int playerID)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerID];
            player.transform.Find("BeaconLight").gameObject.SetActive(false);
        }

        /////////////////////////////////
        //////////////////////////////
        // KICK

        public void TriggerKick()
        {
            if (!active && player.sprintMeter > 0.1f)
            {
                active = true;
                if (abilityRoutine != null)
                {
                    player.StopCoroutine(abilityRoutine);
                }
                abilityRoutine = player.StartCoroutine(reelKick());
            }
        }

        private IEnumerator reelKick()
        {
            player.activatingItem = true;
            player.twoHanded = true;
            player.playerBodyAnimator.ResetTrigger("shovelHit");
            player.playerBodyAnimator.SetBool("reelingUp", value: true);
            PlayClipFromItemAudio(1, playerID, 1f);
            yield return new WaitForSeconds(0.35f);
            if (!ModBase.keybinds.SkillKey.IsPressed())
            {
                EndKick();
                yield break;
            }
            ModBase.logOutput("pulled back");
            yield return new WaitUntil(() => !ModBase.keybinds.SkillKey.IsPressed());
            ModBase.logOutput("released");
            SwingKick();
            yield return new WaitForSeconds(0.13f);
            yield return new WaitForEndOfFrame();
            ModBase.logOutput("hit");
            HitKick();
            yield return new WaitForSeconds(0.3f);
            ModBase.logOutput("done");
            active = false;
            abilityRoutine = null;
        }

        private void SwingKick()
        {
            player.playerBodyAnimator.SetBool("reelingUp", value: false);
            PlayClipFromItemAudio(2, playerID, 1f);
            player.UpdateSpecialAnimationValue(specialAnimation: true, (short)player.transform.localEulerAngles.y, 0.4f);
        }

        private void HitKick()
        {
            if (player.sprintMeter >= 0.1f)
                player.sprintMeter -= 0.1f;
            else
                player.sprintMeter = 0;

            player.activatingItem = false;
            player.twoHanded = false;
            bool flag = false;
            bool flag2 = false;
            int num = -1;

            RaycastHit[] objectsHitByShovel = Physics.SphereCastAll(player.gameplayCamera.transform.position + player.gameplayCamera.transform.right * -0.35f, 0.8f, player.gameplayCamera.transform.forward, 1.5f, 11012424, QueryTriggerInteraction.Collide);
            List<RaycastHit> objectsHitByShovelList = objectsHitByShovel.OrderBy((RaycastHit x) => x.distance).ToList();

            for (int i = 0; i < objectsHitByShovelList.Count; i++)
            {
                IHittable component;
                RaycastHit hitInfo;
                if (objectsHitByShovelList[i].transform.gameObject.layer == 8 || objectsHitByShovelList[i].transform.gameObject.layer == 11)
                {
                    flag = true;
                    string text = objectsHitByShovelList[i].collider.gameObject.tag;
                    for (int j = 0; j < StartOfRound.Instance.footstepSurfaces.Length; j++)
                    {
                        if (StartOfRound.Instance.footstepSurfaces[j].surfaceTag == text)
                        {
                            num = j;
                            break;
                        }
                    }
                }
                else if (objectsHitByShovelList[i].transform.TryGetComponent<IHittable>(out component) && !(objectsHitByShovelList[i].transform == player.transform) && (objectsHitByShovelList[i].point == Vector3.zero || !Physics.Linecast(player.gameplayCamera.transform.position, objectsHitByShovelList[i].point, out hitInfo, StartOfRound.Instance.collidersAndRoomMaskAndDefault)))
                {
                    flag = true;
                    Vector3 forward = player.gameplayCamera.transform.forward;
                    component.Hit(1, forward, player, playHitSFX: true, 1);

                    if (component.GetType() == typeof(EnemyAICollisionDetect))
                    {
                        ((EnemyAICollisionDetect)component).mainScript.SetEnemyStunned(true, 2f);
                        ((EnemyAICollisionDetect)component).mainScript.postStunInvincibilityTimer = 1f;
                    }
                    flag2 = true;
                }
            }

            RaycastHit[] doorObjects = Physics.SphereCastAll(player.gameplayCamera.transform.position + player.gameplayCamera.transform.right * -0.35f, 0.8f, player.gameplayCamera.transform.forward, 1.5f, 1 << LayerMask.NameToLayer("InteractableObject"), QueryTriggerInteraction.Collide);
            List<RaycastHit> doorObjectsList = doorObjects.OrderBy((RaycastHit x) => x.distance).ToList();
            for (int i = 0; i < doorObjectsList.Count; i++)
            {
                DoorLock door;
                RaycastHit hitInfo;

                if (doorObjectsList[i].transform.TryGetComponent<DoorLock>(out door))
                {
                    flag = true;

                    AnimatedObjectTrigger component = door.gameObject.GetComponent<AnimatedObjectTrigger>();
                    if (component.boolValue == false && (!door.isLocked || UnityEngine.Random.Range(1, 3) == 1))
                    {
                        door.UnlockDoorSyncWithServer();
                        door.OpenOrCloseDoor(player);
                        door.SetDoorAsOpen(true);

                        flag2 = true;
                    }
                }
            }

            if (flag)
            {
                PlayClipFromItemAudio(3, playerID, 1f);
                UnityEngine.Object.FindObjectOfType<RoundManager>().PlayAudibleNoise(player.transform.position, 17f, 0.8f);
                if (!flag2 && num != -1)
                {
                    player.itemAudio.PlayOneShot(StartOfRound.Instance.footstepSurfaces[num].hitSurfaceSFX);
                    WalkieTalkie.TransmitOneShotAudio(player.itemAudio, StartOfRound.Instance.footstepSurfaces[num].hitSurfaceSFX);
                }
                player.playerBodyAnimator.SetTrigger("shovelHit");
            }

            if (flag2)
                abilityCooldown = 7;
        }

        private void EndKick()
        {
            active = false;
            abilityRoutine = null;
            player.activatingItem = false;
            player.twoHanded = false;
            player.playerBodyAnimator.SetBool("reelingUp", value: false);
        }

        /////////////////////////////////
        //////////////////////////////
        // STIM

        private bool HarvestAvailable()
        {
            return HarvestTargetExists() && player.isCrouching;
        }

        private bool SynthesizeAvailable()
        {
            return extract >= 50 && charges < 3;
        }

        private bool StimAvailable()
        {
            return charges > 0;
        }

        private bool StimmingTeam()
        {
            RaycastHit[] hits = Physics.RaycastAll(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward, 5f, 11012424, QueryTriggerInteraction.Collide);
            foreach (RaycastHit hit in hits)
            {
                PlayerControllerB component;
                if (hit.transform.TryGetComponent<PlayerControllerB>(out component) && !(hit.transform == player.transform))
                    if (!component.isPlayerDead)
                        return true;
            }
            return false;
        }

        private bool HarvestTargetExists()
        {
            RaycastHit[] hits = Physics.RaycastAll(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward, 2f, 11012424, QueryTriggerInteraction.Collide);
            foreach (RaycastHit hit in hits)
            {
                EnemyAICollisionDetect component;
                if (hit.transform.TryGetComponent<EnemyAICollisionDetect>(out component))
                    if (component.mainScript.isEnemyDead)
                        return true;
            }
            return false;
        }

        public void TriggerHarvest()
        {
            RaycastHit[] hits = Physics.RaycastAll(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward, 2f, 11012424, QueryTriggerInteraction.Collide);
            foreach (RaycastHit hit in hits)
            {
                EnemyAICollisionDetect component;
                if (hit.transform.TryGetComponent(out component))
                    if (component.mainScript.isEnemyDead)
                    {
                        extract += extractValues[component.mainScript.enemyType.enemyName];
                        if (extract > 100)
                            extract = 100;
                        DestroyEnemy(Array.IndexOf(RoundManager.Instance.SpawnedEnemies.ToArray(), component.mainScript));
                        PlayClipFromItemAudio(4, playerID, 1);
                        return;
                    }
            }
        }

        public void TriggerSynthesize()
        {
            extract -= 50;
            PlayClipFromItemAudio(5, playerID, 1);
            charges += 1;
        }

        public void TriggerStim()
        {
            freeCharge = false;
            if (!StimmingTeam())
            {
                charges -= 1;
                SelfStimmed();
            }
            else
            {
                charges -= 1;
                int id = -1;

                RaycastHit[] hits = Physics.RaycastAll(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward, 5f, 11012424, QueryTriggerInteraction.Collide);
                List<RaycastHit> ordered = hits.OrderBy((RaycastHit x) => x.distance).ToList<RaycastHit>();
                foreach (RaycastHit hit in ordered)
                {
                    PlayerControllerB component;
                    if (hit.transform.TryGetComponent(out component) && !(hit.transform == player.transform))
                    {
                        if (!component.isPlayerDead)
                        {
                            id = Array.IndexOf(StartOfRound.Instance.allPlayerScripts, component);
                            ModBase.als.LogInfo("Found component on " + id);
                            break;
                        }
                    }
                }

                if (id == -1)
                {
                    charges += 1;
                    return;
                }

                if (NetworkManager.Singleton.IsHost)
                    TriggerStimClientRpc(id);
                else
                    TriggerStimServerRpc(id);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void TriggerStimServerRpc(int playerID)
        {
            TriggerStimClientRpc(playerID);
        }

        [ClientRpc]
        public void TriggerStimClientRpc(int id)
        {
            if (id == Instance.playerID)
                Instance.SelfStimmed();
        }

        public void SelfStimmed()
        {
            PlayClipFromItemAudio(6, playerID, 1);

            if (player.health >= maxHealth)
                player.drunkness += .5f;
            else
                player.drunkness += .2f;

            player.sprintMeter += 0.1f;
            if (player.sprintMeter > 1)
                player.sprintMeter = 1;

            player.health += (int)(maxHealth * 0.3f);
            if (player.health > maxHealth)
                player.health = maxHealth;

            player.criticallyInjured = false;

            stimmed = true;
        }

        /////////////////////////////////
        //////////////////////////////
        // HACK
        
        private bool HackAvailable()
        {
            return HackTargetExists() && battery >= 0.15f && abilityCooldown <= 0;
        }

        private bool HackTargetExists()
        {
            RaycastHit[] hits = Physics.RaycastAll(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward, 6f, 1 << LayerMask.NameToLayer("MapHazards"), QueryTriggerInteraction.Collide);
            List<RaycastHit> hitList = hits.ToList();
            hitList.AddRange(Physics.RaycastAll(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward, 6f, 1 << LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Collide));
            foreach (RaycastHit hit in hitList)
            {
                TerminalAccessibleObject component;
                if (hit.transform.TryGetComponent<TerminalAccessibleObject>(out component))
                        return true;
            }
            return false;
        }

        private void BeginHack()
        {
            PlayClipFromItemAudio(8, playerID, 0.5f);
            RoundManager.Instance.PlayAudibleNoise(player.transform.position, 30f, 1, 0, false);
        }

        public void TriggerHack()
        {
            RaycastHit[] hits = Physics.RaycastAll(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward, 6f, 1 << LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Collide);
            List<RaycastHit> hitList = hits.ToList();
            hitList.AddRange(Physics.RaycastAll(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward, 6f, 1 << LayerMask.NameToLayer("MapHazards"), QueryTriggerInteraction.Collide));
            foreach (RaycastHit hit in hitList)
            {
                if (hit.transform.TryGetComponent<TerminalAccessibleObject>(out TerminalAccessibleObject component))
                {
                    component.CallFunctionFromTerminal();
                    battery -= 0.15f;
                    abilityCooldown = 5f;
                }
            }
        }

        /////////////////////////////////
        //////////////////////////////
        // HEADLAMP

        public void DestroyHeadlamp()
        {
            if (NetworkManager.IsHost)
                DestroyHeadlampClientRpc(playerID);
            else
                DestroyHeadlampServerRpc(playerID);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DestroyHeadlampServerRpc(int playerID)
        {
            DestroyHeadlampClientRpc(playerID);
        }

        [ClientRpc]
        public void DestroyHeadlampClientRpc(int playerID)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerID];

            Destroy(player.GetComponent<RoleManager>().headlamp);
            player.GetComponent<RoleManager>().headlamp = null;

            if (playerID == this.playerID)
                batteryBar.transform.parent.gameObject.SetActive(false);
        }

        public void CreateHeadlamp()
        {
            if (NetworkManager.IsHost)
                CreateHeadlampClientRpc(playerID);
            else
                CreateHeadlampServerRpc(playerID);
        }

        [ServerRpc(RequireOwnership = false)]
        public void CreateHeadlampServerRpc(int playerID)
        {
            CreateHeadlampClientRpc(playerID);
        }

        [ClientRpc]
        public void CreateHeadlampClientRpc(int playerID)
        {

            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerID];

            GameObject headlamp = Instantiate(ModBase.assets.LoadAsset<GameObject>("Headlamp"));

            player.GetComponent<RoleManager>().headlamp = headlamp;

            Light light = headlamp.GetComponent<Light>();
            light.colorTemperature = 4677;
            light.useColorTemperature = true;
            light.lightShadowCasterMode = LightShadowCasterMode.Everything;

            light.cookie = Resources.Load<Texture2D>("flashlightCookie2 4");

            headlamp.transform.parent = player.transform.Find("PlayerEye");
            headlamp.transform.localPosition = new Vector3(-0.1f, 0.3f, 0);
            headlamp.transform.rotation = new Quaternion(0, 0, 0, 0);

            headlamp.SetActive(false);

            if (playerID == this.playerID)
                batteryBar.transform.parent.gameObject.SetActive(true);

        }

        public void SyncLamp(bool active)
        {
            PlayClipFromItemAudio(7, playerID, 1f);
            if (NetworkManager.IsHost)
                SyncLampClientRpc(playerID, active);
            else
                SyncLampServerRpc(playerID, active);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SyncLampServerRpc(int playerID, bool active)
        {
            SyncLampClientRpc(playerID, active);
        }

        [ClientRpc]
        public void SyncLampClientRpc(int playerID, bool active)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerID];

            player.GetComponent<RoleManager>().headlamp.SetActive(active);
        }

    }
}
