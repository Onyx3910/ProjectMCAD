using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class ConversationManager : MonoBehaviour
{
    public float conversationDistance = 5f;

    public static ConversationManager Instance { get; private set; }
    public Conversable CurrentCharacter { get; set; }
    public List<Conversation> CurrentConversation { get; set; }
    public Conversation ConversationPointer { get; set; }
    public Dictionary<Conversable, (int index, List<Conversation> conversation)> Conversations { get; set; }
    private VolitilePlayerController PlayerController { get; set; }

    public static void ButtonClicked(int index)
    {
        var nextId = Instance.ConversationPointer.PlayerOptions[index].nextId;
        var exit = Instance.ConversationPointer.PlayerOptions[index].exit;

        if(nextId == -1)
        {
            Instance.ExitConversation();
            return;
        }

        Instance.ConversationPointer = Instance.CurrentConversation[nextId];
        Instance.CurrentCharacter.textMeshPro.text = Instance.ConversationPointer.CharacterText;
        Instance.PlayerController.SetDialogOptions(Instance.ConversationPointer.PlayerOptions[0].playerText,
                                                   Instance.ConversationPointer.PlayerOptions[1].playerText,
                                                   Instance.ConversationPointer.PlayerOptions[2].playerText);
        if (exit) Instance.ExitConversation();
    }

    public void StartConversation(Conversable conversable)
    {
        CurrentCharacter = conversable;
        var (index, conversation) = Conversations[conversable];
        CurrentConversation = conversation;
        ConversationPointer = CurrentConversation[index];
        Instance.CurrentCharacter.textMeshPro.text = Instance.ConversationPointer.CharacterText;
        PlayerController.SetConversationState(true);
        Instance.SetPlayerPosition();
        PlayerController.SetDialogOptions(ConversationPointer.PlayerOptions[0].playerText,
                                          ConversationPointer.PlayerOptions[1].playerText,
                                          ConversationPointer.PlayerOptions[2].playerText);
    }

    public void ExitConversation()
    {
        Instance.CurrentCharacter.textMeshPro.text = "Press E to talk";
        Instance.PlayerController.SetConversationState(false);
        Conversations[CurrentCharacter] = (Instance.ConversationPointer.Id, CurrentConversation);
    }

    protected void SetPlayerPosition()
    {
        if(CurrentCharacter.transform.position.x < PlayerController.transform.position.x)
        {
            PlayerController.transform.position = new Vector3(CurrentCharacter.transform.position.x + conversationDistance, PlayerController.transform.position.y, PlayerController.transform.position.z);
        }
        else
        {
            PlayerController.transform.position = new Vector3(CurrentCharacter.transform.position.x - conversationDistance, PlayerController.transform.position.y, PlayerController.transform.position.z);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Singleton pattern
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Get the player controller
        PlayerController = FindObjectOfType<VolitilePlayerController>();

        // Load all conversations
        var conversables = FindObjectsOfType<Conversable>();
        Conversations = new Dictionary<Conversable, (int index, List<Conversation> conversation)>(conversables.Length);
        foreach (var conversable in conversables)
        {
            var conversation = Conversation.Load(conversable.conversation);
            Conversations.Add(conversable, (0, conversation));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
