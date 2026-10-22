using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Every frame, checks if the player is touching a collider (i.e. is able to jump, either from the ground or off of a wall).
/// </summary>
public class PrimaGroundedChecker : MonoBehaviour {

    private const float feetRange = 0.10f;
    private readonly Vector3 feetOffset = new Vector3(0, -.25f, 0);

    public bool IsGrounded { get { return StandingOnCollider != null; } }

    public Collider StandingOnCollider { get; private set; } = null;
    public Vector3 Point { get; private set; }
    public Vector3 Normal { get; private set; }

    // The best surface touched during the current physics step.
    // When touching several surfaces (e.g. rolling from a floor onto a ramp, or a floor and a wall),
    // the one facing most "up" wins, so jumps use the surface you're actually on.
    private float bestScoreThisStep = float.NegativeInfinity;
    private Rigidbody rb;

    private void Awake() {
        rb = GetComponentInParent<Rigidbody>();
    }

    private void FixedUpdate() {
        // Collision callbacks for this step run after FixedUpdate, so start a new comparison.
        bestScoreThisStep = float.NegativeInfinity;

        // A surface that was destroyed or disabled never sends OnTriggerExit, so forget it here.
        if (StandingOnCollider != null && (!StandingOnCollider.enabled || !StandingOnCollider.gameObject.activeInHierarchy)) {
            StandingOnCollider = null;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        OnCollisionStay(collision);
    }

    /*
     * As long as the player is touching a surface, the player will be able to jump off of that surface.
     */
    private void OnCollisionStay(Collision collision) {
        if (collision.collider.isTrigger)
            return;

        Vector3 up = Prima.PlayerPewter != null ? Prima.PlayerPewter.GravityUp : Vector3.up;
        for (int i = 0; i < collision.contactCount; i++) {
            ContactPoint contact = collision.GetContact(i);
            float score = Vector3.Dot(contact.normal, up);
            if (score > bestScoreThisStep) {
                bestScoreThisStep = score;
                StandingOnCollider = collision.collider;
                Point = contact.point;
                Normal = contact.normal;
            }
        }
    }

    /*
     * If the player's body stops touching a surface, they can still jump off of it for a brief time...
     * ...Until the player gets slightly farther away from the surface.
     * This facilitates wall-jumping, because the player may not be perfectly touching the wall when they try to jump.
     * Only the surface being stood on counts: other objects (coins, trigger volumes) leaving shouldn't unground the player.
     */
    private void OnTriggerExit(Collider other) {
        if (other == StandingOnCollider) {
            StandingOnCollider = null;
        }
    }

    /// <summary>
    /// Raycast downward; if there's a ground very close to your feet, always try to jump from that.
    /// </summary>
    public void UpdateStanding() {
        // Use the Rigidbody's position: with interpolation, the transform can lag behind the physics position.
        Vector3 position = rb != null ? rb.position : transform.position;
        if (Physics.Raycast(position + feetOffset, Vector3.down, out RaycastHit hit, feetRange, GameManager.Layer_IgnorePlayer)) {
            if (StandingOnCollider != hit.collider) {
                StandingOnCollider = hit.collider;
                Normal = hit.normal;
                Point = hit.point;
            }
        }
    }
}
