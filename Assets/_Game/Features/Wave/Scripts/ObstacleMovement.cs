using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The ObstacleMovement class controls the scaling, rotation, and movement of an obstacle.
/// It handles changes in size, rotation, side-to-side movement, and up-down movement based on configurable parameters.
/// </summary>
public class ObstacleMovement : MonoBehaviour
{

    [SerializeField] private float scaleSizeValue;

    [SerializeField] private float scaleChangeSpeedX;
    [SerializeField] private float scaleChangeSpeedY;

    [SerializeField] private float rotationSpeed; 

    [Space(10)]
    [SerializeField] private float angleForSideMove = 0;
    [SerializeField] private float sideMoveDistance;
    [SerializeField] private float sideMoveSpeed;

    [Space(10)]
    [SerializeField] private float angleForUpDownMove = 0;
    [SerializeField] private float upDownMoveDistance;
    [SerializeField] private float upDownMoveSpeed;
    
    private float scaleAngleX = 0;
    private float scaleAngleY = 0;
    
    private float originalScaleX;
    private float originalScaleY;
    private Vector2 originalPosition;


    private void Start()
    {
        originalScaleX = transform.localScale.x;
        originalScaleY = transform.localScale.y;
        originalPosition = transform.position;
    }


    private void Update()
    {
        if (scaleChangeSpeedX != 0 || scaleChangeSpeedY != 0)
        {
            ChangeScale();
        }

        if (rotationSpeed != 0)
        {
            RotateObstacle();
        }

        if (sideMoveSpeed != 0)
        {
            MoveSideToSide();
        }

        if (upDownMoveDistance != 0)
        {
            MoveUpDown();
        }
    }

    // Changes the scale of the obstacle over time
    private void ChangeScale()
    {
        transform.localScale = new Vector2(originalScaleX + Mathf.Sin(scaleAngleX) * scaleSizeValue, originalScaleY + Mathf.Sin(scaleAngleY) * scaleSizeValue);
        scaleAngleX += Time.deltaTime * scaleChangeSpeedX;
        scaleAngleY += Time.deltaTime * scaleChangeSpeedY;
    }

    // Rotates the obstacle over time
    private void RotateObstacle()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed, Space.World);
    }

    // Moves the obstacle side-to-side over time
    private void MoveSideToSide()
    {
        Vector2 pos = transform.position;
        pos.x = originalPosition.x + Mathf.Sin(angleForSideMove) * sideMoveDistance;
        transform.position = pos;

        angleForSideMove += Time.deltaTime * sideMoveSpeed;
    }

    // Moves the obstacle up and down over time
    private void MoveUpDown()
    {
        Vector2 pos = transform.position;
        pos.y = originalPosition.y + Mathf.Sin(angleForUpDownMove) * upDownMoveDistance;
        transform.position = pos;

        angleForUpDownMove += Time.deltaTime * upDownMoveSpeed;
    }




}
