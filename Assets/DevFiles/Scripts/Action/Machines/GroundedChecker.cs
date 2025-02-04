using clrev01.Bases;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines
{
    public class GroundedChecker : BaseOfCL
    {
        [SerializeField]
        private IGroundableHD groundableHd;
        private int _latestCollisionFrame = -1;

        private void OnCollisionStay(Collision collision)
        {
            if (1 << collision.gameObject.layer != layerOfGround) return;
            if (_latestCollisionFrame != ACM.actionFrame)
            {
                _latestCollisionFrame = ACM.actionFrame;
                for (int i = 0; i < groundableHd.groundContacts.Count; i++)
                {
                    groundableHd.groundContacts[i] = null;
                }
            }
            groundableHd.touchGround = true;
            var b = false;
            for (var i = 0; i < collision.contactCount; i++)
            {
                var contactPoint = collision.GetContact(i);
                var moveParGroundContacts = groundableHd.groundContacts;
                for (var j = 0; j < moveParGroundContacts.Count; j++)
                {
                    if (moveParGroundContacts[j].HasValue) continue;
                    moveParGroundContacts[j] = contactPoint;
                    b = true;
                    break;
                }
                if (!b) moveParGroundContacts.Add(contactPoint);
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            if (1 << collision.gameObject.layer == layerOfGround)
            {
                groundableHd.touchGround = false;
            }
        }
    }
}