using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitcher : MonoBehaviour
{
    public bool buttonToSwitch = true;
    public bool canSwitch = true;
    public bool lockAfterUse = false;
    public LevelSwitcher nextLevel;

    public CameraController CameraController { get; set; }
    public bool AtDoor { get; set; }

    private void Start()
    {
        CameraController = Camera.main.GetComponent<CameraController>();
    }

    private void Update()
    {
        if (buttonToSwitch && AtDoor && Input.GetKeyDown(KeyCode.E))
        {
            MoveToNextLevel(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>());
            if (lockAfterUse) nextLevel.canSwitch = false;
            CameraController.levelSpriteRenderer = nextLevel.transform.parent.GetComponent<SpriteRenderer>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!canSwitch) return;
            AtDoor = true;
            if(!buttonToSwitch)
            {
                MoveToNextLevel(collision);
                if (lockAfterUse) nextLevel.canSwitch = false;
                CameraController.levelSpriteRenderer = nextLevel.transform.parent.GetComponent<SpriteRenderer>();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AtDoor = false;
        }
    }

    protected void MoveToNextLevel(Collider2D collision)
    {
        Camera.main.transform.position = new Vector3(nextLevel.transform.position.x, nextLevel.transform.position.y, Camera.main.transform.position.z);
        collision.transform.position = nextLevel.transform.position;
    }
}
