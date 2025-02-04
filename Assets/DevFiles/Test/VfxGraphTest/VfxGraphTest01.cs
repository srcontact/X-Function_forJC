using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class VfxGraphTest01 : MonoBehaviour
{
    [SerializeField]
    VisualEffect vfx;
    [SerializeField]
    bool exeVfx;
    ExposedProperty lifetime = "Lifetime";
    ExposedProperty positionCurve = "PositionCurve";
    ExposedProperty sizeCurve = "SizeCurve";

    [SerializeField, Range(0, 20)]
    float enginePower = 1;

    [SerializeField, Range(0f, 20f)]
    float lifetimeValue = 1;

    [SerializeField]
    AnimationCurve positionCurveValue;
    [SerializeField, Range(0, 20)]
    float posLength = 1;

    [SerializeField]
    AnimationCurve sizeCurveValue;
    [SerializeField]
    float size = 1;

    [SerializeField, Range(1, 60)]
    int exeFrameRate = 1;

    private void Start()
    {
        vfx.pause = true;
    }

    private void FixedUpdate()
    {
        if (exeVfx && Time.frameCount % exeFrameRate == 0)
        {
            vfx.SetFloat(lifetime, lifetimeValue);

            var key1 = positionCurveValue.keys[1];
            key1.value = posLength * enginePower;
            positionCurveValue.MoveKey(1, key1);
            positionCurveValue.SmoothTangents(1, 1);
            vfx.SetAnimationCurve(positionCurve, positionCurveValue);

            var sizeKey1 = sizeCurveValue.keys[1];
            sizeKey1.value = size * enginePower;
            sizeCurveValue.MoveKey(1, sizeKey1);
            vfx.SetAnimationCurve(sizeCurve, sizeCurveValue);

            vfx.Simulate(1f / 60f, 1);
        }
    }
}