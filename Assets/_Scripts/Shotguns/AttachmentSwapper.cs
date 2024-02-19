using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AttachmentSwapper
{
    /// <summary>
    /// Swaps the from attachment off of the shotgun model and replaces it with the to attachment
    /// </summary>
    /// <param name="shotgun">the shotgun to swap on</param>
    /// <param name="from">the attachment to take off</param>
    /// <param name="to">the attachment to swap to</param>
    /// <returns>the id of the to attachment</returns>
    public static void SwapTo(this Shotgun shotgun, IAttachment from, IAttachment to)
    {
        from.DetachFrom(shotgun);
        to.AttachTo(shotgun);

        //TODO: Implement visual despawning and spawning
    }
}
