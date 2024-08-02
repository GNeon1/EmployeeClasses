using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeClasses.Patches
{
    public class Keybinds : LcInputActions
    {
        [InputAction(KeyboardControl.Y, Name = "Yodel")]
        public InputAction YodelKey { get; set; }
    }
}
