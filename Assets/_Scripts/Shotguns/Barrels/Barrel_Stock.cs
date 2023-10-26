using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel_Stock : Barrel
{
    private float radius = 50;
    public override void AttachTo(Shotgun shotgun)
    {
        throw new System.NotImplementedException();
    }

    public override void DetachFrom(Shotgun shotgun)
    {
        throw new System.NotImplementedException();
    }



    public override Vector2[] GetScreenPointSpread(int numPellets)
    {
        Vector2[] sol = new Vector2[numPellets];
        for (int i = 0; i < numPellets; i++)
        {
            sol[i] = (Vector2)new Vector3(((Random.value * 2) - 1) * radius, ((Random.value * 2) - 1) * radius, ((Random.value * 2) - 1) * radius);
        }
        return sol;
    }
}
