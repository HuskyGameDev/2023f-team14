using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each attachment has a unique ID.
/// Barrels range from 0-99.
/// Underbarrels range from 100-199.
/// Accessories range from 200-299.
/// Ammo Types range from 300-399.
/// </summary>
public enum AttachmentID : uint
{
    //Barrels
    Barrel_Stock = 0,
    // Underbarrels
    Underbarrel_Stock = AttachmentDepot.AttachmentIDBlockSize + 0,
    /* Accessories */
    Accessory_Stock = AttachmentDepot.AttachmentIDBlockSize * 2 + 0,
    /* Ammo Types */
    AmmoType_Stock = AttachmentDepot.AttachmentIDBlockSize * 3 + 0,
    AmmoType_Fireball = AttachmentDepot.AttachmentIDBlockSize * 3 + 1,
}

public interface IAttachment
{
    public AttachmentID ID { get; }
    /// <summary>
    /// This is called whenever this attachment is attached to a shotgun.
    /// </summary>
    /// <param name="shotgun">The shotgun attached to</param>
    public void AttachTo(Shotgun shotgun);
    /// <summary>
    /// This is called whenever this attachment is detached from a shotgun.
    /// </summary>
    /// <param name="shotgun">The shotgun detached from</param>
    public void DetachFrom(Shotgun shotgun);
}

public class AttachmentDepot
{
    private static readonly string pathToPrefabDirectory = "";
    public const uint AttachmentIDBlockSize = 100;
    private readonly Dictionary<AttachmentID, GameObject> attachmentDict = new();

    public AttachmentDepot()
    {
        var attachments = Resources.LoadAll(pathToPrefabDirectory + "Attachments");
        foreach (var attachment in attachments)
        {
            attachmentDict.Add(((GameObject)attachment).GetComponent<IAttachment>().ID, ((GameObject)attachment));
            Debug.Log("Attachment registered: " + ((GameObject)attachment).GetComponent<IAttachment>().ID);
        }
    }
    /// <summary>
    /// Finds the type of a given attachment.
    /// </summary>
    /// <param name="id">The id of the attachment.</param>
    /// <returns>Barrel, Underbarrel, Accessory, or AmmoType</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the id is not within the bounds of AttachmentID</exception>
    public static Type GetTypeOfAttachment(AttachmentID id)
    {
        if ((uint)id < AttachmentIDBlockSize)
            return typeof(Barrel);
        if ((uint)id < AttachmentIDBlockSize * 2)
            return typeof(Underbarrel);
        if ((uint)id < AttachmentIDBlockSize * 3)
            return typeof(Accessory);
        if ((uint)id < AttachmentIDBlockSize * 4)
            return typeof(AmmoType);
        throw new ArgumentOutOfRangeException("Cannot find attachment type for id " + id + "!");
    }

    /// <summary>
    /// Returns reference to a prefab assigned to the given id
    /// </summary>
    /// <param name="id">the attachment to find</param>
    /// <returns>the prefab gameobject</returns>
    public GameObject GetGameObject(AttachmentID id)
    {
        if (attachmentDict.TryGetValue(id, out var go))
        {
            return go;
        }
        throw new KeyNotFoundException();
    }

    public List<AttachmentID> GetAttachmentIDs<T>() where T : IAttachment
    {
        List<AttachmentID> list = new();
        foreach (var id in attachmentDict.Keys)
        {
            if (GetTypeOfAttachment(id) == typeof(T))
            {
                list.Add(id);
            }
        }
        return list;
    }
}