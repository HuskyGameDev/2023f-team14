using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRecoil : MonoBehaviour
{
    public Vector3 RecoilDegree { get; private set; }
    [Header("Recoil values")]
    [SerializeField]
    private float verticalRecoilAmount;
    [SerializeField]
    private float verticalRecoilVariation;
    [SerializeField]
    private float horizontalRecoilAmount;
    [SerializeField]
    private float horizontalRecoilVariation;
    [SerializeField]
    [Range(0f, 1f)]
    private float returnToCenterSpeed;

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Recoil recovery
        RecoilDegree -= RecoilDegree * returnToCenterSpeed;
        if (RecoilDegree.magnitude < 0.1f)
        {
            RecoilDegree = Vector3.zero;
        }

    }

    public void Recoil()
    {
        var pattern = GenerateRecoilPattern();
        RecoilDegree += pattern;
    }

    private Vector3 GenerateRecoilPattern() => new(
        UnityEngine.Random.Range(verticalRecoilAmount - verticalRecoilVariation, verticalRecoilAmount + verticalRecoilVariation) //Up
        , UnityEngine.Random.Range(horizontalRecoilAmount - horizontalRecoilVariation, horizontalRecoilAmount + horizontalRecoilVariation) //L/R
        , 0f //No tilt
    );
}
