
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The FollowPlayer class makes the camera smoothly follow the player game object with a specified offset.
/// The camera's position is updated every frame to follow the player's position with a smoothing effect.
/// </summary>
public class FollowPlayer : MonoBehaviour
{

    [SerializeField] private GameObject player; // The player game object the camera will follow
    // [SerializeField] private float smoothTime = 0.3F; // The time it takes for the camera to catch up to the player's position
    [SerializeField] private int yOffset; // The vertical offset from the player's position

    
    private Vector3 velocity = Vector3.zero; // The velocity of the camera, used by SmoothDamp



    private void LateUpdate()
    {
        // Calculate the target position for the camera
        Vector3 targetPosition = player.transform.TransformPoint(new Vector3(0, yOffset, -10));
        targetPosition = new Vector3(0, targetPosition.y, targetPosition.z);

        // Smoothly move the camera towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0);
    }

}