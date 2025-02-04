namespace clrev01.ClAction.Machines
{
    public enum CharMoveState
    {
        inAir, //空中にいる状態
        isGrounded, //接地中
        uncontrollable, //被弾、スピードオーバーなどで制御不能な状態
        recovery, //制御不能からの回復中。
    }
}