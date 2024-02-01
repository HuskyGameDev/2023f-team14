using UnityEngine;

public class Barrel_Stock : Barrel
{
    private readonly float radiusRadians = 0.2f;

    public override AttachmentID ID => AttachmentID.Barrel_Stock;

    public override void AttachTo(Shotgun shotgun)
    {
    }

    public override void DetachFrom(Shotgun shotgun)
    {
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
