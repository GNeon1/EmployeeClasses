using LethalCompanyInputUtils.Api;
using LethalCompanyInputUtils.BindingPathEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace EmployeeClasses.Inputs
{
    internal class Keybinds : LcInputActions
    {
        [InputAction(KeyboardControl.F, Name = "Use Skill")]
        public InputAction SkillKey { get; set; }
    }
}
