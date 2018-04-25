using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DemoSpace
{
    public partial class KnightGame : MonoBehaviour
    {
        void OpenKingdomNews()
        {
            for (int i = 0; i < partyPlayer.Count; i++)
            {
                partyPlayer[i].infoConfirm = false;
            }
            KingdomNews();
            panelKingdomNews.DOLocalMoveY(panelKingdomNews.localPosition.y + 596, 0.7f).SetEase(Ease.OutBack);            
        }
        void KingdomNews()
        {
            step = GameStep.KingdomNews;
            round++;
            int month = Mathf.CeilToInt(round * 0.5f);
            daynight = (DayNight)(round % 2);
            if (gameover)
            {
                step = GameStep.Gameover;
                textStep.text = "遊戲結束";
                source.PlayOneShot(clips[5]);
            }
            else
            {
                textStep.text = daynight == DayNight.夜 ? "夜間傳聞" : "晝間傳聞";
                source.PlayOneShot(clips[(int)daynight]);
            }
            background.sprite = spriteBG[(int)daynight];
            textCountdownTimer.text = "---";
            textDate.text = "163年 " + month + "月 " + daynight.ToString();
            textSolaPlayerCount.text = "" + (partyPlayer.Count - luna.friends.Count);
            textLunaPlayerCount.text = "" + (partyPlayer.Count - sola.friends.Count);
            textSolaCastleCount.text = "" + sola.castle;
            textLunaCastleCount.text = "" + luna.castle;
            for (int i = 0; i < partyPlayer.Count; i++)
            {
                PlayerInfo player = partyPlayer[i];
                player.ClearPast();
                textAllPlayerPrestige[i].text = "" + player.nowPrestige;
            }

            // 傳聞發佈
            string news = "";
            for (int i = 0; i < kingdomNews.Count; i++)
            {
                news += kingdomNews[i];
                news += "\n";
            }
            textKingdomNews.text = news;
            kingdomNews.Clear();
        }
        void FreeTime()
        {
            step = GameStep.FreeTime;
            source.PlayOneShot(clips[3]);
            countdownTimer = 300;
            textStep.text = daynight == DayNight.夜 ? "夜間交流" : "晝間交流";
        }
        void ArrangeAction()
        {
            step = GameStep.Action;
            source.PlayOneShot(clips[4]);
            textCountdownTimer.text = "---";
            textStep.text = daynight == DayNight.夜 ? "夜間行動" : "晝間行動";
        }
    }
}
