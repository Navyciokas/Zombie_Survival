using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFloat : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float floatHeight = 0.3f;
    public float rotateSpeed = 90f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        startPosition.y = startPosition.y + 0.5f;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );

        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}
