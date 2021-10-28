
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

    public void _UpdateScores()
    {
        int i = 0;
        foreach(Player player in players)
        {
            if (!Utilities.IsValid(player)) break;

            if (player.isAssigned)
            {
                entries[i].SetActive(true);
                lugarText[i].text = i.ToString();
                puntajeText[i].text = player.GetScore().ToString();
                nombreText[i].text = player.Owner.displayName;
            } else
            {
                entries[i].SetActive(false);
            }
            i++;
            if (i >= entries.Length) break;
        }
    }
    
}
