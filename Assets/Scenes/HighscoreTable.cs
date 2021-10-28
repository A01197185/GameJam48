using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreTable : MonoBehaviour{

    private Transform entryContainer;
    private Transform entryTemplate;
    private List<HighscoreEntry> highscoreEntryList;
    private List<Transform> highscoreEntryTransformList;

    private void Awake() {
        // Init
        entryContainer = transform.Find("EntryContainer");
        entryTemplate = entryContainer.Find("EntryTemplate");
        entryTemplate.gameObject.SetActive(false);

        // Testing
        highscoreEntryList = new List<HighscoreEntry>() {
            new HighscoreEntry{score = 120, name = "Diego"},
            new HighscoreEntry{score = 250, name = "Brandon"},
            new HighscoreEntry{score = 340, name = "Adrian"},
            new HighscoreEntry{score = 100, name = "Silver"},
            new HighscoreEntry{score = 86, name = "Nose"},
            new HighscoreEntry{score = 15, name = "Joseph"}
        };

        // Sort
        for (int i = 0; i < highscoreEntryList.Count; i++) {
            for (int j = i + 1; j < highscoreEntryList.Count; j++) {
                if (highscoreEntryList[j].score > highscoreEntryList[i].score) {
                    HighscoreEntry temp = highscoreEntryList[i];
                    highscoreEntryList[i] = highscoreEntryList[j];
                    highscoreEntryList[j] = temp;
                }
            }
        }

        // Add enties to UI
        highscoreEntryTransformList = new List<Transform>();
        int cont = 0;
        foreach (HighscoreEntry highscoreEntry in highscoreEntryList) {
            if (cont < 8) {
                createHighscoreEntry(highscoreEntry, entryContainer, highscoreEntryTransformList);
            }
            cont++;
        }
    }

    private void Update() {
        // if alguien se conecta
        // addEntryToList(highscoreEntryList, nombre)

        // Actualiza tabla
    }

    private void addEntryToList(string eName) {
        highscoreEntryList.Add(new HighscoreEntry { score = 0, name = eName });
    }

    private void createHighscoreEntry(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList) {
        float templateHeight = 30f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryRectTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        entryTransform.Find("posText").GetComponent<Text>().text = "" + rank;

        int score = highscoreEntry.score;
        entryTransform.Find("scoreText").GetComponent<Text>().text = "" + score;

        string name = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = name;

        transformList.Add(entryTransform);
    }

    // Single entry
    private class HighscoreEntry {
        public int score;
        public string name;
    }
}
