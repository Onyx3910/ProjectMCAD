using UnityEngine;

public class LevelSwitcher : MonoBehaviour
{
    [Header("Door Settings")]

    public bool buttonToSwitch = true;
    public bool canSwitch = true;
    public bool lockAfterUse = false;
    public LevelSwitcher nextLevel;

    [Header("Camera Settings")]

    public float zoomAfterUse = 5f;

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
            UseDoor(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!canSwitch) return;
            AtDoor = true;
            if(!buttonToSwitch) UseDoor(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AtDoor = false;
        }
    }

    private void MoveToNextLevel(Collider2D collision)
    {
        Camera.main.transform.position = new Vector3(nextLevel.transform.position.x, nextLevel.transform.position.y, Camera.main.transform.position.z);
        collision.transform.position = nextLevel.transform.position;
    }

    protected void UseDoor(Collider2D collision)
    {
        MoveToNextLevel(collision);
        if (lockAfterUse) nextLevel.canSwitch = false;
        CameraController.levelSpriteRenderer = nextLevel.transform.parent.GetComponent<SpriteRenderer>();
        Camera.main.orthographicSize = zoomAfterUse;
    }
}
