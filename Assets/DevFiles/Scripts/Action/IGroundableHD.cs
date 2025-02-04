using System.Collections.Generic;
using UnityEngine;

namespace clrev01.ClAction
{
    public interface IGroundableHD
    {
        public List<ContactPoint?> groundContacts { get; }
        public bool touchGround { get; set; }
    }
}