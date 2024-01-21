using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Conversable : MonoBehaviour
{
    public string conversation;
    public TextMeshPro textMeshPro;

    public bool CanTalk { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        textMeshPro.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanTalk && Input.GetKeyDown(KeyCode.E))
        {
            ConversationManager.Instance.StartConversation(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            textMeshPro.text = "Press E to talk";
            CanTalk = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            textMeshPro.text = string.Empty;
            CanTalk = false;
        }
    }
}
