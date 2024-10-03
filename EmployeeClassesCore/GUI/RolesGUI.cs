using EmployeeClasses.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EmployeeClasses.GUI
{
    internal class RolesGUI
    {

        public static RolesGUI instance;

        public GameObject mainPanel;
        public GameObject classSelect;
        public GameObject buttons;

        public TextMeshProUGUI currentRole;
        public TextMeshProUGUI abilityTip;

        private List<Role> roles = new List<Role>{
            new Role("Scout", new List<RoleTag>()
            {
                new RoleTag((int)RoleTag.Ids.SPRINT_MULTIPLIER, 1.3f),
                new RoleTag((int)RoleTag.Ids.SPRINT_TIME, 1.15f),
                new RoleTag((int)RoleTag.Ids.MAX_HEALTH, 50),
                new RoleTag((int)RoleTag.Ids.WEIGHT_PENALTY, 1.7f),
                new RoleTag((int)RoleTag.Ids.WEIGHT_PENALTY_THRESH, 20),
                new RoleTag((int)RoleTag.Ids.OXYGEN, 1.8f),
                new RoleTag((int)RoleTag.Ids.FALL_DAMAGE, 0.5f)
            }, "BEACON", ""),
            new Role("Brute", new List<RoleTag>()
            {
                new RoleTag((int)RoleTag.Ids.FOOTSTEP_VOLUME, 1.05f),
                new RoleTag((int)RoleTag.Ids.MAX_HEALTH, 200),
                new RoleTag((int)RoleTag.Ids.WEIGHT_PENALTY, 0.5f),
                new RoleTag((int)RoleTag.Ids.SPEED_MULTIPLIER, 0.85f),
                new RoleTag((int)RoleTag.Ids.FALL_DAMAGE, 2),
                new RoleTag((int)RoleTag.Ids.ATTACK_DAMAGE, 2),
                new RoleTag((int)RoleTag.Ids.THREAT_LEVEL, 4)
            }, "KICK", ""),
            new Role("Researcher", new List<RoleTag>()
            {
                new RoleTag((int)RoleTag.Ids.FOOTSTEP_VOLUME, 0.5f),
                new RoleTag((int)RoleTag.Ids.MAX_HEALTH, 80),
                new RoleTag((int)RoleTag.Ids.FALL_DAMAGE, 1.12f),
                new RoleTag((int)RoleTag.Ids.SCAN_RANGE, 2f),
                new RoleTag((int)RoleTag.Ids.WEIGHT_PENALTY, 1.2f),
                new RoleTag((int)RoleTag.Ids.SCAN_WALL)
            }, "STIM", ""),
            new Role("Maintenance", new List<RoleTag>()
            {
                new RoleTag((int)RoleTag.Ids.FOOTSTEP_VOLUME, 1.1f),
                new RoleTag((int)RoleTag.Ids.SPRINT_MULTIPLIER, 0.9f),
                new RoleTag((int)RoleTag.Ids.BULLET_DAMAGE, 0.75f),
                new RoleTag((int)RoleTag.Ids.ALWAYS_CLOCK),
                new RoleTag((int)RoleTag.Ids.HEADLAMP),
                new RoleTag((int)RoleTag.Ids.SPRINT_TIME, 1.15f),
                new RoleTag((int)RoleTag.Ids.FOG_VISIBILITY, 0.5f)
            }, "HACK", ""),
            new Role("Employee", new List<RoleTag>(), "Just a boring old asset")
        };

        public RolesGUI() {
            instance = this;
            CreateMenu();
        }

        public void ShowTip(string text)
        {
            abilityTip.text = text;
            abilityTip.gameObject.SetActive(true);
        }

        public void ShowTip(string text, Color color)
        {
            abilityTip.text = text;
            abilityTip.color = color;
            abilityTip.gameObject.SetActive(true);
        }

        public void HideTip()
        {
            abilityTip.gameObject.SetActive(false);
        }

        public void CreateMenu() {
            mainPanel = UnityEngine.Object.Instantiate<GameObject>(ModBase.assets.LoadAsset<GameObject>("ClassesHUD"), GameObject.Find("Systems/UI").transform);

            mainPanel.transform.Find("Ability").gameObject.SetActive(true);
            abilityTip = mainPanel.transform.Find("Ability/Tip").GetComponent<TextMeshProUGUI>();

            classSelect = mainPanel.transform.Find("ClassSelect").gameObject;
            currentRole = classSelect.transform.Find("Current").GetComponent<TextMeshProUGUI>();

            buttons = classSelect.transform.Find("Buttons").gameObject;

            Transform childTransform = buttons.transform.Find("Button");
            GameObject child = childTransform.gameObject;

            child.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            child.GetComponent<Button>().onClick.AddListener(delegate { SelectClass(roles[0]); });

            child.name = roles[0].name;
            child.GetComponentInChildren<TextMeshProUGUI>().text = roles[0].name;

            for (int i = 1; i < roles.Count; i++) {
                child = UnityEngine.Object.Instantiate(child,buttons.transform);
                child.transform.position = new Vector3(child.transform.position.x,child.transform.position.y-40*i,child.transform.position.z);

                int j = i;
                child.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                child.GetComponent<Button>().onClick.AddListener(delegate { SelectClass(roles[j]); });

                child.name = roles[i].name;
                child.GetComponentInChildren<TextMeshProUGUI>().text = roles[i].name;
            }
        }

        public void OpenMenu()
        {
            classSelect.SetActive(true);
            if (StartOfRound.Instance.inShipPhase || StartOfRound.Instance.IsHost)
                buttons.SetActive(true);
            else
                buttons.SetActive(false);
        }

        public void CloseMenu()
        {
            classSelect.SetActive(false);
        }

        private void SelectClass(Role role)
        {
            currentRole.text = "Role: "+role.name+" ";
            RoleManager.Instance.changeRole(role);
            RoleManager.Instance.applyRole(StartOfRound.Instance.localPlayerController);

            HUDManager.Instance.AddTextToChatOnServer("<color=#00FF00>" + StartOfRound.Instance.localPlayerController.playerUsername+ "</color>: <color=#FFFF00>" + role.name+"</color>");
        }
    }
}
