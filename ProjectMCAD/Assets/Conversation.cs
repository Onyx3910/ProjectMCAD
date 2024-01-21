using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class Conversation
{
    private Conversation(int id, string characterText, params (string playerText, int nextId, bool exit)[] playerChoices)
    {
        Id = id;
        CharacterText = characterText;
        PlayerOptions = new List<(string playerText, int nextId, bool exit)>();
        foreach(var playerChoice in playerChoices)
        {
            PlayerOptions.Add(playerChoice);
        }
    }

    public int Id { get; set; }
    public string CharacterText { get; set; }
    public List<(string playerText, int nextId, bool exit)> PlayerOptions { get; set; }

    public static List<Conversation> Load(string filePath)
    {
        var textAsset = Resources.Load<TextAsset>($"Conversations\\{filePath}");
        var lines = textAsset.text.Split("\r\n");

        var conversationList = new List<Conversation>(lines.Length);
        foreach (var line in lines)
        {
            var columns = line.Split('|');
            var id = columns[0];
            var characterText = columns[1];
            var playerChoice1 = GetPlayerChoice(columns[2]);
            var playerChoice2 = GetPlayerChoice(columns[3]);
            var playerChoice3 = GetPlayerChoice(columns[4]);
            conversationList.Add(new Conversation(int.Parse(id), characterText, playerChoice1, playerChoice2, playerChoice3));
        }

        return conversationList;
    }

    protected static (string, int, bool) GetPlayerChoice(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return (string.Empty, -1, false);
        }

        var columns = data.Split(',');
        return (columns[0], int.Parse(columns[1]), !string.IsNullOrEmpty(columns[2]));
    }
}
