
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Highscore : UdonSharpBehaviour
{
    [SerializeField] Player[] players;

    [SerializeField] GameObject[] entries;
    [SerializeField] Text[] lugarText;
    [SerializeField] Text[] puntajeText;
    [SerializeField] Text[] nombreText;

    int[] highscores;
    int[] highscoreIndex;

    public void Start()
    {
        highscoreIndex = new int[entries.Length];
        highscores = new int[entries.Length];
    }

    public void _UpdateScores()
    {

        for(int i = 0; i < highscores.Length; i++)
        {
            highscores[i] = players[i].GetScore();
            highscoreIndex[i] = i;
        }

        for(int i = 0; i < highscores.Length; i++)
        {
            for(int j = i+1; j<highscores.Length; j++)
            {
                if(highscores[i] < highscores[j])
                {
                    int temp = highscores[i];
                    int indexTemp = highscoreIndex[i];
                    highscores[i] = highscores[j];
                    highscoreIndex[i] = highscoreIndex[j];
                    highscores[j] = temp;
                    highscoreIndex[j] = indexTemp;
                }
            }
        }

        for(int i = 0; i < entries.Length; i++)
        {
            if (!Utilities.IsValid(players[highscoreIndex[i]])) break;

            if (players[highscoreIndex[i]].isAssigned)
            {
                entries[i].SetActive(true);
                lugarText[i].text = (i + 1).ToString();
                puntajeText[i].text = $"{highscores[i]}";
                Debug.Log($"{highscoreIndex[i]}");
                nombreText[i].text = $"{players[highscoreIndex[i]].Owner.displayName}";
            } else
            {
                entries[i].SetActive(false);
            }
        }


        /*int bruh = 0;
        foreach(Player player in players)
        {
            if (!Utilities.IsValid(player)) break;

            if (player.isAssigned)
            {
                entries[bruh].SetActive(true);
                lugarText[bruh].text = bruh.ToString();
                puntajeText[bruh].text = player.GetScore().ToString();
                nombreText[bruh].text = player.Owner.displayName;
            } else
            {
                entries[bruh].SetActive(false);
            }
            bruh++;
            if (bruh >= entries.Length) break;
        }*/
    }

}
