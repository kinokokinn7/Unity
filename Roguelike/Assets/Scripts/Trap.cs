using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Trap : MapObjectBase
{
    public enum Type
    {
        LifeDown,
        FoodDown,
    }

    public Type CurrentType = Type.LifeDown;
    public int Value = 5;
}
