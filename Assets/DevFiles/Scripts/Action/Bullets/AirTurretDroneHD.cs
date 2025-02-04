using System.Collections.Generic;
using clrev01.ClAction.Bullets;
using clrev01.ClAction.Effect.Thruster;
using clrev01.ClAction.Machines.Motion;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Extensions.ExPrediction;

namespace clrev01.ClAction.Bullets
{
    public class AirTurretDroneHD : MineHD<AirTurretDroneCD, AirTurretDroneLD, AirTurretDroneHD>
    {
        [SerializeField]
        private List<ThrusterVfxControl> thrusters = new();

        protected override void ExeMove()
        {
            if (ld.firedCount >= ld.cd.ammoNum || ld.maxLiveFrame < ACM.actionFrame - ld.spawnFrame)
            {
                Disable();
            }

            Vector3 thrustV = new();
            var nowAltitude = Physics.Raycast(pos, Vector3.down, out var hit, 10000, layerOfGround) ? hit.distance : 0;
            var altitudeThrustFlag = nowAltitude < ld.cd.normalAltitude;
            if (altitudeThrustFlag)
            {
                thrustV += Vector3.up * ld.cd.stayAirThrustRatio;
            }

            var target = ld.target;
            if (target != null)
            {
                var posAccuracy = target.GetPosAccuracy(pos, ld.cd.searchParameterData, objectSearchTgt.jammedSize);
                var tgtPos = target.GetTargetPosition(teamID, uniqueID, ld.cd.searchParameterData, posAccuracy);
                var tgtVelocity = target.GetTargetVelocity(teamID, uniqueID, ld.cd.searchParameterData, posAccuracy);

                if (!altitudeThrustFlag && pos.y < tgtPos.y + ld.cd.normalAltitude)
                {
                    thrustV += Vector3.up * ld.cd.stayAirThrustRatio;
                }

                var moveTgtPos = tgtPos;
                var toTargetVector = moveTgtPos - pos;
                bool fireStartFlag = toTargetVector.magnitude < ld.cd.fireStartDistance;
                if (!fireStartFlag)
                {
                    if (Physics.Raycast(pos, toTargetVector.normalized, out _, thrustV.magnitude * ld.cd.obstacleAvoidDistanceRatio, layerOfGround))
                    {
                        thrustV += ld.cd.obstacleAvoidThrustRatio * (ld.moveLeftOrRight ? 1 : -1) * Vector3.Cross(rigidBody.linearVelocity.normalized, Vector3.up) + Vector3.back;
                    }
                    else
                    {
                        thrustV += toTargetVector.normalized * ld.cd.toTargetThrustRatio;
                        thrustV += transform.right * ((ld.moveLeftOrRight ? 1 : -1) * ld.cd.sideMoveRate * ld.randomize);
                    }
                }
                else
                {
                    thrustV += transform.right * ((ld.moveLeftOrRight ? 1 : -1) * ld.cd.sideMoveRate * ld.randomize);
                }

                if (fireStartFlag)
                {
                    var predictionPos = LinePrediction(
                        tgtPos + tgtVelocity / 60,
                        tgtVelocity,
                        ld.cd.origBullet.speed,
                        ld.cd.origBullet.DragCoefficient * ACM.actionEnvPar.globalAirBreakPar,
                        pos + rigidBody.linearVelocity / 60,
                        rigidBody.linearVelocity,
                        ACM.actionEnvPar.globalGPowMSec
                    );
                    transform.rotation = Quaternion.LookRotation((predictionPos - pos).normalized);
                    if (ld.latestFireFrame + ld.cd.origBullet.MinimumFiringInterval < ACM.actionFrame)
                    {
                        ld.cd.origBullet.Shoot(pos, (predictionPos - pos).normalized, rigidBody.linearVelocity, target, objectSearchTgt, teamID, uniqueID);
                        ld.latestFireFrame = ACM.actionFrame;
                    }
                }
                else
                {
                    if (rigidBody.linearVelocity.sqrMagnitude > Vector3.kEpsilonNormalSqrt)
                    {
                        transform.rotation = Quaternion.LookRotation(rigidBody.linearVelocity.normalized);
                    }
                }
            }

            var thrustVRes = (thrustV.normalized * ld.cd.thrustMaxSpeed - rigidBody.linearVelocity) * ld.cd.thrustGain;
            var magnitude = Vector3.Dot(rigidBody.linearVelocity, thrustVRes) >= 0 ? Vector3.Project(rigidBody.linearVelocity, thrustVRes).magnitude : 0;
            var maxAcceleNowSpeed = ld.cd.thrustAcceleCurve.Evaluate(magnitude / ld.cd.thrustMaxSpeed) * ld.cd.thrustMaxSpeed;
            if (thrustVRes.magnitude > maxAcceleNowSpeed)
            {
                thrustVRes = thrustVRes.normalized * maxAcceleNowSpeed;
            }
            rigidBody.AddForce(thrustVRes, ForceMode.VelocityChange);

            foreach (var thruster in thrusters)
            {
                var tp = Mathf.Max(Vector3.Project(-thrustVRes, thruster.transform.up).magnitude * Vector3.Dot(-thrustVRes.normalized, thruster.transform.up), 0) / ld.cd.thrustMaxSpeed * 100;
                thruster.ThrusterExe(tp);
            }
        }
    }
}