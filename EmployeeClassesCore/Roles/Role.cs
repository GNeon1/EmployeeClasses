using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeClasses.Roles
{
    public class Role
    {
        public string name;
        public List<RoleTag> tags;
        public string ability;
        public string description;

        public Role(string name, List<RoleTag> tags, string ability, string description)
        {
            this.name = name;
            this.tags = tags;
            this.ability = ability;
            this.description = description;
        }

        public Role(string name, List<RoleTag> tags, string description)
        {
            this.name = name;
            this.tags = tags;
            this.ability = null;
            this.description = description;
        }
    }
}
