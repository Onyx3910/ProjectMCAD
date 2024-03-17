using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public string targetTag = "Player";
    public SpriteRenderer levelSpriteRenderer;

    protected Vector3 velocity;

    protected Bounds LevelBounds => levelSpriteRenderer.bounds;
    protected Bounds ViewBounds => new(new Vector2(transform.position.x, transform.position.y), Camera.main.orthographicSize * 2f * new Vector2(Camera.main.aspect, 1f));
    private Transform Target { get; set; }

    void Start()
    {
        Target = GameObject.FindGameObjectWithTag(targetTag).transform;
    }

    void LateUpdate()
    {
        if (Target == null) return;
        transform.position = Vector3.SmoothDamp(transform.position, Target.position - transform.forward, ref velocity, 0.15f);
        MoveCameraBackToLevel();
    }

    protected void MoveCameraBackToLevel()
    {
        var cameraPosition = transform.position;

        if (ViewBounds.min.x < LevelBounds.min.x)
        {
            cameraPosition.x = LevelBounds.min.x + ViewBounds.extents.x;
        }
        else if (ViewBounds.max.x > LevelBounds.max.x)
        {
            cameraPosition.x = LevelBounds.max.x - ViewBounds.extents.x;
        }
        if (ViewBounds.min.y < LevelBounds.min.y)
        {
            cameraPosition.y = LevelBounds.min.y + ViewBounds.extents.y;
        }
        else if (ViewBounds.max.y > LevelBounds.max.y)
        {
            cameraPosition.y = LevelBounds.max.y - ViewBounds.extents.y;
        }

        transform.position = cameraPosition;
    }
}
