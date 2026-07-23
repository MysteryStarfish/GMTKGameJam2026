using System;

interface InputReader
{
    public event Action OnAttack;
    public event Action OnCharge;
    public event Action OnCounter;
}
