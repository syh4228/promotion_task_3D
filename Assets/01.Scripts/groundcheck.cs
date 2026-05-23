using UnityEngine;
using System;

public class Groundcheck : MonoBehaviour
{
    public LayerMask GroundLayer;
    public float GroundCheckDistance = 2f;

    public bool IsGrounded { get; private set; }

    private void Update()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        RaycastHit hit;

        Vector3 rayStart = transform.position + Vector3.up * 0.1f;

        IsGrounded = Physics.Raycast(rayStart, Vector3.down, out hit, GroundCheckDistance + 0.1f, GroundLayer);
    }
}
