using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Barrel : MonoBehaviour, IAttachment
{
    public abstract void AttachTo(Shotgun shotgun);
    public abstract void DetachFrom(Shotgun shotgun);
    /// <summary>
    /// Generates an array of screen-point coordinates corresponding to each pellet fired.
    /// </summary>
    /// <param name="numPellets">The number of pellets to fire.</param>
    /// <returns>An array of coordinates, where (0,0) is the center of the screen.</returns>
    public abstract Vector2[] GetPelletSpread(int numPellets);
}
