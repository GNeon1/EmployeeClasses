using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeClasses.Roles
{
    public class RoleTag
    {
        public enum Ids {
            SPRINT_MULTIPLIER = 0,
            SPEED_MULTIPLIER = 1,
            MAX_HEALTH = 2,
            WEIGHT_PENALTY = 3,
            OXYGEN = 4,
            FALL_DAMAGE = 5,
            ATTACK_DAMAGE = 6,
            WEIGHT_PENALTY_THRESH = 7,
            FOOTSTEP_VOLUME = 8,
            SCAN_RANGE = 9,
            SCAN_WALL = 10,
            THREAT_LEVEL = 11,
            BULLET_DAMAGE = 12,
            ALWAYS_CLOCK = 13,
            HEADLAMP = 14,
            SPRINT_TIME = 15,
            FOG_VISIBILITY = 16,
            ATTACK_SPEED = 17,
            WEIGHT_MULTIPLIER_1H = 18,
            WEIGHT_MULTIPLIER_2H = 19
        }

        public int Id;

        public float Power;

        public RoleTag(int id, float power)
        {
            this.Id = id;
            this.Power = power;
        }

        public RoleTag(int id)
        {
            this.Id = id;
            this.Power = 1;
        }
    }
}
