using UnityEngine;

public class Barrel_Stock : Barrel
{
    private readonly float radiusRadians = 0.14f;
    public override void AttachTo(Shotgun shotgun)
    {
        throw new System.NotImplementedException();
    }

    public override void DetachFrom(Shotgun shotgun)
    {
        throw new System.NotImplementedException();
    }



    public override Vector2[] GetPelletSpread(int numPellets)
    {
        //TODO Do some sort of distribution to concentrate pellets in the center of the reticle
        Vector2[] sol = new Vector2[numPellets];
        for (int i = 0; i < numPellets; i++)
        {
            sol[i] = Random.insideUnitCircle * radiusRadians;
        }
        return sol;
    }
}
