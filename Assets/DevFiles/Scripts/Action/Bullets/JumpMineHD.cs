using System;
using clrev01.Bases;
using clrev01.ClAction.ObjectSearch;
using clrev01.Extensions;
using clrev01.Programs;
using clrev01.Programs.FieldPar;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Extensions.ExPrediction;

namespace clrev01.ClAction.Bullets
{
    public class JumpMineHD : MineHD<JumpMineCD, JumpMineLD, JumpMineHD>
    {
        private readonly ObjectSearchTgt[] _lockOnArray = new ObjectSearchTgt[1];

        protected override void ExeMove()
        {
            ManageLockOn(ld.cd.searchIntervalFrame, ref ld.nowTarget, _lockOnArray, ld.cd.proximityFuseRange, ld.cd.maxTrackingDistance);
            ExeMove(ld.nowTarget);
        }

        private void ExeMove(ObjectSearchTgt lockOnTgt)
        {
            switch (ld.moveState)
            {
                case MineMoveState.InAir:
                    rigidBody.AddForce(-rigidBody.linearVelocity * (ld.cd.DragCoefficient * ACM.actionEnvPar.globalAirBreakPar), ForceMode.Acceleration);
                    break;
                case MineMoveState.Grounded:
                    if (lockOnTgt != null)
                    {
                        var posAccuracy = lockOnTgt.GetPosAccuracy(pos, ld.cd.searchParameterData, objectSearchTgt.jammedSize);
                        var tgtPos = lockOnTgt.GetTargetPosition(teamID, uniqueID, ld.cd.searchParameterData, posAccuracy);
                        if (Vector3.Angle(rigidBody.linearVelocity.normalized, (tgtPos - pos).normalized) <= 180)
                        {
                            if (ld.latestJumpFrame + ld.cd.jumpInterval <= ACM.actionFrame)
                            {
                                Vector3 groundNormal = new();
                                foreach (var t in groundContacts)
                                {
                                    if (!t.HasValue) continue;
                                    var contactV = (t.Value.point - t.Value.thisCollider.bounds.center).normalized;
                                    var dot = Mathf.Max(Vector3.Dot(rigidBody.linearVelocity, contactV), 0.01f);
                                    groundNormal += t.Value.normal * dot;
                                }
                                groundNormal = groundNormal.normalized;
                                var v = LinePrediction(
                                    tgtPos,
                                    lockOnTgt.GetTargetVelocity(teamID, uniqueID, ld.cd.searchParameterData, posAccuracy),
                                    ld.cd.jumpPow,
                                    ld.cd.DragCoefficient * ACM.actionEnvPar.globalAirBreakPar,
                                    ld.hd.pos,
                                    ld.hd.rigidBody.linearVelocity,
                                    ACM.actionEnvPar.globalGPowMSec
                                );
                                var jVector = (v - pos).normalized;
                                var angleDif = Vector3.SignedAngle(groundNormal, jVector.normalized, Vector3.Cross(groundNormal, jVector.normalized));
                                var verticalAngle = 90 - ld.cd.normalJumpAngle;
                                if (angleDif > verticalAngle)
                                {
                                    jVector = Vector3.RotateTowards(groundNormal, jVector.normalized, verticalAngle * Mathf.Deg2Rad, 0) * jVector.magnitude;
                                }
                                rigidBody.AddForce(jVector * ld.cd.jumpPow, ForceMode.VelocityChange);
                                ld.latestJumpFrame = ACM.actionFrame;
                            }
                        }
                        else
                        {
                            if (rigidBody.linearVelocity.magnitude > 10)
                            {
                                rigidBody.AddForce(-rigidBody.linearVelocity * 0.5f, ForceMode.VelocityChange);
                            }
                        }
                    }
                    else
                    {
                        if (rigidBody.linearVelocity.magnitude > 10)
                        {
                            rigidBody.AddForce(-rigidBody.linearVelocity * 0.5f, ForceMode.VelocityChange);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}