using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{


    // -------------------------- Display Bound -------------------------- // 
    private float mapWidth = 100.0f;
    private float mapHeight = 100.0f;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private Vector2 topLeft;
    private Vector2 bottomLeft;
    private Vector2 topRight;
    private Vector2 bottomRight;

    private float displayWidth;
    private float displayHeight;

    private float leftBoundary;
    private float rightBoundary;
    private float bottomBoundary;
    private float topBoundary;


    private void Awake()
    {
        InitializeDisplayBoundaries();
    }


    // Initializes the display boundaries based on the camera's view
    private void InitializeDisplayBoundaries()
    {
        float verticalExtent = Camera.main.orthographicSize;
        float horizontalExtent = verticalExtent * Screen.width / Screen.height;


        minX = horizontalExtent - mapWidth / 2.0f;
        maxX = mapWidth / 2.0f - horizontalExtent;
        minY = verticalExtent - mapHeight / 2.0f;
        maxY = mapHeight / 2.0f - verticalExtent;

        topLeft = new Vector2(maxX - 50, minY + 50);
        bottomLeft = new Vector2(maxX - 50, maxY - 50);
        topRight = new Vector2(minX + 50, minY + 50);
        bottomRight = new Vector2(minX + 50, maxY - 50);

        displayWidth = bottomRight.x - bottomLeft.x;
        displayHeight = topLeft.y - bottomLeft.y;

        leftBoundary = topLeft.x;
        rightBoundary = topRight.x;
        topBoundary = topLeft.y;
        bottomBoundary = bottomLeft.y;
    }


    // Returns the width of the display area
    public float GetDisplayWidth()
    {
        return displayWidth;
    }

}