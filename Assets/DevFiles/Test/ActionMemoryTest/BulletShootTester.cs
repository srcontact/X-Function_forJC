using clrev01.Bases;
using clrev01.ClAction;
using clrev01.ClAction.Bullets;
using UnityEngine;

public class BulletShootTester : HardBase
{
    public BulletCD bullet;
    public int interval = 5;
    public int count = 0;

    void Start()
    {
        ActionManager.Inst.AddList.Add(this);
    }

    public override void RunBeforePhysics()
    { }
    public override void RunAfterPhysics()
    {
        if (count % interval == 0)
        {
            bullet.Shoot(transform.position, transform.forward, Vector3.zero, null, null);
        }
        count++;
    }
    public override void OnDotonExe()
    { }
}