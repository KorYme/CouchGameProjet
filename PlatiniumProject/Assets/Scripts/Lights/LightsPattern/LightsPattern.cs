using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LightsPattern
{
    public abstract void UpdatePattern();
    public abstract bool IsThisLightEnlighted(int index);
}
