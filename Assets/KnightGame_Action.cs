/* * * * * * * * * * * * * * * * * * * * *
 * 
 *    Title: "項目"
 * 
 *    Dsecription:
 *                  功能: 
 *                   1. 
 * 
 *     Author: iLYuSha
 *     
 *     Date: 2018.03.24
 *     
 *     Modify:
 *                  03.24 修改: 
 *                   1. 
 *     
 * * * * * * * * * * * * * * * * * * * * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace DemoSpace
{
    public partial class KnightGame : MonoBehaviour
    {
        [Header("Action")]
        public int targetPlayer;
        public List<int> actionAbdicate = new List<int>(); // 進行退位的玩家
        public List<int> actionExecute = new List<int>(); // 進行處決的玩家
        public List<int> actionImprison = new List<int>(); // 進行守衛的玩家
        public List<int> actionGuard = new List<int>(); // 進行守衛的玩家
        public List<int> actionAssassinate = new List<int>(); // 進行暗殺的玩家
        public List<int> actionDefense = new List<int>(); // 進行防禦的玩家
        public List<int> actionConquer = new List<int>(); // 進行征服的玩家
        public List<int> actionPeerage = new List<int>(); // 進行封爵的玩家
        public List<int> actionDiplomacy = new List<int>(); // 進行外交的玩家

        public delegate void MyAciton();
        public MyAciton myAciton;

        void PrivacyMessage()
        {
            if (stateRFID == RFID_State.Reading)
            {
                if (nowIndex > 0 && nowIndex <= partyPlayer.Count)
                {
                    panelPlayerInfo.SetActive(true);
                    PlayerInfo player = partyPlayer[nowIndex - 1];
                    textPlayerIndex.text = player.name.ToString();
                    textPlayerNowFaction.text = ((FactionCht)player.nowFaction).ToString();
                    textPlayerNowChracter.text = ((CharacterCht)player.nowCharacter).ToString();
                    textPlayerNowPrestige.text = player.nowPrestige.ToString();
                    textPlayerLastFaction.text = ((FactionCht)player.lastFaction).ToString();
                    textPlayerLastChracter.text = ((CharacterCht)player.lastCharacter).ToString();
                    textPlayerLastPrestige.text = player.lastPrestige.ToString();
                    partyPlayer[nowIndex - 1].infoConfirm = true;
                    textOperation.text = "私人訊息發佈";
                    string msg = "";
                    for (int i = 0; i < player.privacy.Count; i++)
                    {
                        msg += player.privacy[i];
                        msg += "\n";
                    }
                    textPrivacyMessage.text = msg;
                }
                else
                    HidePlayerInfo();
            }
            else if (stateRFID == RFID_State.Missing)
            {
                nowIndex = 0;
                HidePlayerInfo();
                stateRFID = RFID_State.Ready;
            }
        }
        void PlayerAction()
        {
            if (stateRFID == RFID_State.Reading)
            {
                if (nowIndex > 0 && nowIndex <= partyPlayer.Count)
                {
                    panelPlayerInfo.SetActive(true);
                    PlayerInfo player = partyPlayer[nowIndex - 1];
                    textPlayerIndex.text = player.name.ToString();
                    textPlayerNowFaction.text = ((FactionCht)player.nowFaction).ToString();
                    textPlayerNowChracter.text = ((CharacterCht)player.nowCharacter).ToString();
                    textPlayerNowPrestige.text = player.nowPrestige.ToString();
                    textPlayerLastFaction.text = ((FactionCht)player.lastFaction).ToString();
                    textPlayerLastChracter.text = ((CharacterCht)player.lastCharacter).ToString();
                    textPlayerLastPrestige.text = player.lastPrestige.ToString();
                    textOperation.text = "行動決定";

                    if (player.actionConfirm)
                    {
                        textPrivacyMessage.text = player.privacy[0];
                        return;
                    }

                    if (player.nowCharacter == Character.King) // 國王
                    {
                        actionOption[0].SetActive(true); // 退位
                        actionOption[1].SetActive(true); // 處決
                        if ((int)daynight != (int)player.nowFaction) actionOption[5].SetActive(true); // 防禦
                        if ((int)daynight == (int)player.nowFaction) actionOption[6].SetActive(true); // 征服
                        actionOption[7].SetActive(true); // 封爵
                    }
                    else if (player.nowCharacter == Character.Duke) // 公爵
                    {
                        actionOption[2].SetActive(true); // 囚禁
                        if ((int)daynight != (int)player.nowFaction) actionOption[5].SetActive(true); // 防禦
                        if ((int)daynight == (int)player.nowFaction) actionOption[6].SetActive(true); // 征服
                        actionOption[8].SetActive(true); // 外交
                    }
                    else if (player.nowCharacter == Character.Knight) // 騎士
                    {
                        actionOption[3].SetActive(true); // 守衛
                        actionOption[4].SetActive(true); // 暗殺
                        actionOption[5].SetActive(true); // 防禦
                        actionOption[6].SetActive(true); // 征服
                    }
                }
                else
                    HidePlayerInfo();
            }
            else if (stateRFID == RFID_State.Missing)
            {
                nowIndex = 0;
                HidePlayerInfo();
                stateRFID = RFID_State.Ready;
            }            
        }
        void HidePlayerInfo()
        {
            textPlayerIndex.text = "";
            textPlayerNowFaction.text = "";
            textPlayerNowChracter.text = "";
            textPlayerNowPrestige.text = "";
            textPlayerLastFaction.text = "";
            textPlayerLastChracter.text = "";
            textPlayerLastPrestige.text = "";
            textPrivacyMessage.text = "";
            CloseAllActionOption();
            CloseAllTargetOption();
            panelPlayerInfo.SetActive(false);
        }
        public void SelectAbdicate()
        {
            FocusActionOption(actionOption[0]);
            CloseAllTargetOption();
            PlayerInfo player = partyPlayer[nowIndex - 1];
            if (player.nowFaction == Faction.Sola)
            {
                for (int i = 0; i < sola.friends.Count; i++) { targetOption[(sola.friends[i] - 1)].SetActive(true); }
            }
            else if (player.nowFaction == Faction.Luna)
            {
                for (int i = 0; i < luna.friends.Count; i++) { targetOption[(luna.friends[i] - 1)].SetActive(true); }
            }
            targetOption[nowIndex - 1].SetActive(false);
            myAciton = new MyAciton(SelectAbdicate);
            if (targetPlayer == 0) return;

            if (!actionAbdicate.Contains(nowIndex)) actionAbdicate.Add(nowIndex);
            player.action = Action.Abdicate;
            player.target = targetPlayer;
            player.actionConfirm = true;
            player.privacy.Clear();
            player.privacy.Add("你決定退位並指定" + partyPlayer[targetPlayer - 1].name.ToString() + "為" + ((FactionCht)player.nowFaction).ToString() + "的新國王");
            targetPlayer = 0;
            CloseAllActionOption();
            CloseAllTargetOption();
        }
        public void SelectExecute()
        {
            FocusActionOption(actionOption[1]);
            CloseAllTargetOption();
            PlayerInfo player = partyPlayer[nowIndex - 1];
            for (int i = 0; i < partyPlayer.Count; i++) { targetOption[i].SetActive(true); }
            targetOption[nowIndex - 1].SetActive(false);
            myAciton = new MyAciton(SelectExecute);
            if (targetPlayer == 0) return;

            if (!actionExecute.Contains(nowIndex)) actionExecute.Add(nowIndex);
            player.action = Action.Execute;
            player.target = targetPlayer;
            player.actionConfirm = true;
            player.privacy.Clear();
            player.privacy.Add("你下令處決" + partyPlayer[targetPlayer - 1].name.ToString());
            targetPlayer = 0;
            CloseAllActionOption();
            CloseAllTargetOption();
        }
        public void SelectImprison()
        {
            FocusActionOption(actionOption[2]);
            CloseAllTargetOption();
            PlayerInfo player = partyPlayer[nowIndex - 1];
            for (int i = 0; i < partyPlayer.Count; i++) { targetOption[i].SetActive(true); }
            targetOption[nowIndex - 1].SetActive(false);
            myAciton = new MyAciton(SelectImprison);
            if (targetPlayer == 0) return;

            if (!actionImprison.Contains(nowIndex)) actionImprison.Add(nowIndex);
            player.action = Action.Imprison;
            player.target = targetPlayer;
            player.actionConfirm = true;
            player.privacy.Clear();
            player.privacy.Add("你下令囚禁" + partyPlayer[targetPlayer - 1].name.ToString());
            targetPlayer = 0;
            CloseAllActionOption();
            CloseAllTargetOption();
        }
        public void SelectGuard()
        {
            FocusActionOption(actionOption[3]);
            CloseAllTargetOption();
            PlayerInfo player = partyPlayer[nowIndex - 1];
            for (int i = 0; i < partyPlayer.Count; i++) { targetOption[i].SetActive(true); }
            myAciton = new MyAciton(SelectGuard);
            if (targetPlayer == 0) return;

            if (!actionGuard.Contains(nowIndex)) actionGuard.Add(nowIndex);
            player.action = Action.Guard;
            player.target = targetPlayer;
            player.actionConfirm = true;
            player.privacy.Clear();
            player.privacy.Add("你決定守衛" + partyPlayer[targetPlayer - 1].name.ToString());
            targetPlayer = 0;
            CloseAllActionOption();
            CloseAllTargetOption();
        }
        public void SelectAssassinate()
        {
            FocusActionOption(actionOption[4]);
            CloseAllTargetOption();
            PlayerInfo player = partyPlayer[nowIndex - 1];
            for (int i = 0; i < partyPlayer.Count; i++) { targetOption[i].SetActive(true); }
            targetOption[nowIndex - 1].SetActive(false);
            myAciton = new MyAciton(SelectAssassinate);
            if (targetPlayer == 0) return;

            if (!actionAssassinate.Contains(nowIndex)) actionAssassinate.Add(nowIndex);
            player.action = Action.Assassinate;
            player.target = targetPlayer;
            player.actionConfirm = true;
            player.privacy.Clear();
            player.privacy.Add("你決定暗殺" + partyPlayer[targetPlayer - 1].name.ToString());
            targetPlayer = 0;
            CloseAllActionOption();
            CloseAllTargetOption();
        }
        public void SelectDefense()
        {
            CloseAllTargetOption();
            PlayerInfo player = partyPlayer[nowIndex - 1];
            if (!actionDefense.Contains(nowIndex)) actionDefense.Add(nowIndex);
            player.action = Action.Defense;
            player.actionConfirm = true;
            player.privacy.Clear();
            player.privacy.Add("你決定防禦城堡");
            CloseAllActionOption();
        }
        public void SelectConquer()
        {
            CloseAllTargetOption();
            PlayerInfo player = partyPlayer[nowIndex - 1];
            if (!actionConquer.Contains(nowIndex)) actionConquer.Add(nowIndex);
            player.action = Action.Conquer;
            player.actionConfirm = true;
            player.privacy.Clear();
            player.privacy.Add("你決定征服城堡");
            CloseAllActionOption();
        }
        public void SelectPeerage()
        {
            FocusActionOption(actionOption[7]);
            CloseAllTargetOption();
            PlayerInfo player = partyPlayer[nowIndex - 1];
            if (player.nowFaction == Faction.Sola)
            {
                for (int i = 0; i < sola.friends.Count; i++) { targetOption[(sola.friends[i] - 1)].SetActive(true); }
            }
            else if (player.nowFaction == Faction.Luna)
            {
                for (int i = 0; i < luna.friends.Count; i++) { targetOption[(luna.friends[i] - 1)].SetActive(true); }
            }
            targetOption[nowIndex - 1].SetActive(false);
            myAciton = new MyAciton(SelectPeerage);
            if (targetPlayer == 0) return;

            if (!actionPeerage.Contains(nowIndex)) actionPeerage.Add(nowIndex);
            player.action = Action.Peerage;
            player.target = targetPlayer;
            player.actionConfirm = true;
            player.privacy.Clear();
            player.privacy.Add("你決定冊封" + partyPlayer[targetPlayer - 1].name.ToString() + "為" + ((FactionCht)player.nowFaction).ToString() + "公爵");
            targetPlayer = 0;
            CloseAllActionOption();
            CloseAllTargetOption();
        }
        public void SelectDiplomacy()
        {
            FocusActionOption(actionOption[8]);
            CloseAllTargetOption();
            PlayerInfo player = partyPlayer[nowIndex - 1];
            for (int i = 0; i < partyPlayer.Count; i++) { targetOption[i].SetActive(true); }
            targetOption[nowIndex - 1].SetActive(false);
            myAciton = new MyAciton(SelectDiplomacy);
            if (targetPlayer == 0) return;

            if (!actionDiplomacy.Contains(nowIndex)) actionDiplomacy.Add(nowIndex);
            player.action = Action.Diplomacy;
            player.target = targetPlayer;
            player.actionConfirm = true;
            player.privacy.Clear();
            player.privacy.Add("你決定探聽" + partyPlayer[targetPlayer - 1].name.ToString() + "的身份");
            targetPlayer = 0;
            CloseAllActionOption();
            CloseAllTargetOption();
        }
        public void ActionTarget(int indexTarget)
        {
            targetPlayer = indexTarget;
            myAciton();
        }
        public void CalculateAction()
        {
            listBeExcuted.Clear();
            listBeAssassinated.Clear();
            kingdomNews.Clear();

            for (int i = 0; i < partyPlayer.Count; i++)
            {
                partyPlayer[i].privacy.Clear();
                partyPlayer[i].RecordLastInfo();

                if (!partyPlayer[i].actionConfirm)
                {
                    partyPlayer[i].nowPrestige--;
                    partyPlayer[i].privacy.Add("你未作出任何行動失去1點威望");
                }
            }

            Abdicate();
            Execute();
            Imprison();
            GuardAndAssassin();
            DefenseAndConquer();
            Peegage();
            WhoIsSuccessor();
            Diplomacy();
            // 王國信息
            if (listBeExcuted.Count > 0)
            {
                string newsExcute = "很不幸地，";
                for (int i = 0; i < listBeExcuted.Count; i++)
                {
                    newsExcute += ((Player)listBeExcuted[i]).ToString();
                    if (i < listBeExcuted.Count - 1)
                        newsExcute += ", ";
                }
                newsExcute += "遭到處決";
                kingdomNews.Add(newsExcute);
            }
            if (listBeAssassinated.Count > 0)
            {
                string newsAssasinate = "";
                for (int i = 0; i < listBeAssassinated.Count; i++)
                {
                    newsAssasinate += ((Player)listBeAssassinated[i]).ToString();
                    if (i < listBeAssassinated.Count - 1)
                        newsAssasinate += ", ";
                }
                newsAssasinate += "遭到暗殺";
                kingdomNews.Add(newsAssasinate);
            }
            // 遊戲是否結束
            IsGameOver();
            // 國王盟友
            if (!gameover)
            {
                partyPlayer[sola.king - 1].privacy.Add(sola.GetKingdomFriendsInfo());
                partyPlayer[luna.king - 1].privacy.Add(luna.GetKingdomFriendsInfo());
            }
            // 清空行動記錄
            actionAbdicate.Clear(); // 退位
            actionExecute.Clear(); // 處決
            actionImprison.Clear(); // 囚禁
            actionGuard.Clear(); // 守衛
            actionAssassinate.Clear(); // 暗殺
            actionDefense.Clear(); // 防禦
            actionConquer.Clear(); // 征服
            actionPeerage.Clear(); // 封爵
            actionDiplomacy.Clear(); // 外交
            panelKingdomNews.DOLocalMoveY(panelKingdomNews.localPosition.y - 596, 0.7f).SetEase(Ease.InBack).OnComplete(OpenKingdomNews);
        }
        void Abdicate()
        {
            for (int i = 0; i < actionAbdicate.Count; i++)
            {
                int indexPlayer = actionAbdicate[i];
                PlayerInfo player = partyPlayer[indexPlayer - 1];
                int indexTarget = player.target;
                PlayerInfo target = partyPlayer[indexTarget - 1];

                // 現任國王退位程序
                player.nowCharacter = Character.Duke;
                if (player.nowFaction == Faction.Sola)
                {
                    sola.successor = indexPlayer;
                    sola.duke.Add(indexPlayer);
                }
                else if (player.nowFaction == Faction.Luna)
                {
                    luna.successor = indexPlayer;
                    luna.duke.Add(indexPlayer);
                }
                player.privacy.Add("你已退位為" + ((FactionCht)player.nowFaction).ToString() + "的公爵兼第一順位繼承人");

                // 新任國王上位程序
                Succeed(indexTarget, player.nowFaction);
                target.privacy.Add("你繼任為" + ((FactionCht)target.nowFaction).ToString() + "的新國王");
                // 繼任國王後無法執行下列行動
                switch (target.action)
                {
                    case Action.Imprison: actionImprison.Remove(indexTarget); break; // 公爵限定
                    case Action.Guard: actionGuard.Remove(indexTarget); break; // 騎士限定
                    case Action.Assassinate: actionAssassinate.Remove(indexTarget); break; // 騎士限定
                    case Action.Defense: if ((int)daynight == (int)player.nowFaction) actionDefense.Remove(indexTarget); break; // 進行限定
                    case Action.Conquer: if ((int)daynight != (int)player.nowFaction) actionConquer.Remove(indexTarget); break; // 防禦限定
                    case Action.Diplomacy: actionDiplomacy.Remove(indexTarget); break; // 公爵限定
                }
            }
            actionAbdicate.Clear();
        }
        void Execute()
        {
            for (int i = 0; i < actionExecute.Count; i++)
            {
                int indexPlayer = actionExecute[i];
                PlayerInfo player = partyPlayer[indexPlayer - 1];
                int indexTarget = player.target;
                PlayerInfo target = partyPlayer[indexTarget - 1];

                // 不能處決國王
                if (target.nowCharacter == Character.King)
                {
                    player.nowPrestige--;
                    player.privacy.Add("處決" + ((FactionCht)target.nowFaction).ToString() + "國王失敗失去1點威望");
                }
                else
                {
                    // 國王選擇一名玩家處決，自己失去1點威望。
                    player.nowPrestige--;
                    player.privacy.Add("處決" + target.name + "失去1點威望");

                    if (target.hasBeen != Action.Execute) // 無論被多少人處決都只計算一次
                    {
                        // 受到處決的玩家             
                        Demote(indexTarget); // 降為騎士失去繼任順位
                        target.hasBeen = Action.Execute; // 本輪行動無法被囚禁、守衛與暗殺
                        target.nowPrestige = Mathf.Max(target.nowPrestige - 3, 0); // 失去3點威望
                        target.privacy.Add("遭到處決失去3點威望");
                        switch (target.action) // 無法執行後續行動
                        {
                            case Action.Imprison: actionImprison.Remove(indexTarget); break;
                            case Action.Guard: actionGuard.Remove(indexTarget); break;
                            case Action.Assassinate: actionAssassinate.Remove(indexTarget); break;
                            case Action.Defense: actionDefense.Remove(indexTarget); break;
                            case Action.Conquer: actionConquer.Remove(indexTarget); break;
                            case Action.Diplomacy: actionDiplomacy.Remove(indexTarget); break;
                        }
                        if (!listBeExcuted.Contains(indexTarget)) // 處決消息會於下一次王國傳聞公開
                            listBeExcuted.Add(indexTarget); // 記錄至下一次王國傳聞
                    }
                }
            }
            actionExecute.Clear();
        }
        void Imprison()
        {
            for (int i = 0; i < actionImprison.Count; i++)
            {
                int indexPlayer = actionImprison[i];
                PlayerInfo player = partyPlayer[indexPlayer - 1];
                int indexTarget = player.target;
                Debug.Log(indexPlayer + "求禁" + indexTarget);
                PlayerInfo target = partyPlayer[indexTarget - 1];

                if (target.hasBeen == Action.Execute)
                    player.privacy.Add("無法囚禁，" + target.name.ToString() + "已遭到處決");
                else if (target.nowCharacter == Character.King)
                {
                    // 若目標此時為國王，則囚禁失敗自己失去1點威望。
                    player.nowPrestige--;
                    player.privacy.Add("囚禁" + ((FactionCht)target.nowFaction).ToString() + "國王失敗失去1點威望");
                }
                else
                {
                    if (target.hasBeen != Action.Imprison) // 無論被多少人囚禁都只計算一次
                    {
                        // 被囚禁玩家
                        target.hasBeen = Action.Imprison; // 本輪行動無法被守衛與暗殺
                        target.nowPrestige = Mathf.Max(target.nowPrestige - 1, 0); // 失去1點威望
                        target.privacy.Add("遭到囚禁失去1點威望");
                        switch (target.action) // 無法執行後續行動
                        {
                            case Action.Guard: actionGuard.Remove(indexTarget); break;
                            case Action.Assassinate: actionAssassinate.Remove(indexTarget); break;
                            case Action.Defense: actionDefense.Remove(indexTarget); break;
                            case Action.Conquer: actionConquer.Remove(indexTarget); break;
                            case Action.Diplomacy: actionDiplomacy.Remove(indexTarget); break;
                        }
                    }
                }
            }
            actionImprison.Clear();
        }
        void GuardAndAssassin()
        {
            // 守衛行動
            for (int i = 0; i < actionGuard.Count; i++)
            {
                int indexPlayer = actionGuard[i];
                PlayerInfo player = partyPlayer[indexPlayer - 1];
                int indexTarget = player.target;
                PlayerInfo target = partyPlayer[indexTarget - 1];
                if (target.hasBeen == Action.Execute)
                    player.privacy.Add("無法守衛，" + target.name.ToString() + "已遭到處決");
                else if (target.hasBeen == Action.Imprison)
                    player.privacy.Add("無法守衛，" + target.name.ToString() + "已遭到囚禁");
                else
                    target.guard.Add(indexPlayer);
            }
            actionGuard.Clear();

            // 暗殺行動
            for (int i = 0; i < actionAssassinate.Count; i++)
            {
                int indexPlayer = actionAssassinate[i];
                PlayerInfo player = partyPlayer[indexPlayer - 1];
                int indexTarget = player.target;
                PlayerInfo target = partyPlayer[indexTarget - 1];
                if (target.hasBeen == Action.Execute)
                    player.privacy.Add("無法暗殺，" + target.name.ToString() + "已遭到處決");
                else if (target.hasBeen == Action.Imprison)
                    player.privacy.Add("無法暗殺，" + target.name.ToString() + "已遭到囚禁");
                else
                    target.assassin.Add(indexPlayer);
            }
            actionAssassinate.Clear();

            // 守衛與暗殺結果
            for (int i = 0; i < partyPlayer.Count; i++)
            {
                // 行動成功可獲得的威望
                int indexPlayer = i + 1;
                PlayerInfo player = partyPlayer[i];
                int prestige = (int)player.nowCharacter;
                // 因暗殺為同時進行，但迴圈會產生前後順序，故如果公爵是本輪騎士暗殺成功所封，仍以騎士計算
                if (player.nowCharacter == Character.Duke && player.lastCharacter == Character.Knight)
                    prestige = 1;

                if (player.assassin.Count == 0) { } // 無人暗殺
                else if (player.guard.Count > 0)
                {
                    // 守衛成功                    
                    for (int g = 0; g < player.guard.Count; g++)
                    {
                        PlayerInfo guard = partyPlayer[player.guard[g] - 1];
                        guard.nowPrestige += prestige;
                        guard.privacy.Add("守護" + player.name + "成功獲得" + prestige + "點威望");
                        if (prestige == 3)
                            guard.peerage = player.nowFaction;
                    }
                    for (int a = 0; a < player.assassin.Count; a++)
                    {
                        // 所有暗殺者-1威望
                        PlayerInfo assassin = partyPlayer[player.assassin[a] - 1];
                        assassin.nowPrestige = Mathf.Max(assassin.nowPrestige - 1, 0);
                        assassin.privacy.Add("暗殺" + player.name + "失敗失去1點威望");
                    }
                }
                else if (player.assassin.Count > 0 && player.guard.Count == 0)
                {
                    // 暗殺成功
                    for (int a = 0; a < player.assassin.Count; a++)
                    {
                        PlayerInfo assassin = partyPlayer[player.assassin[a] - 1];
                        assassin.nowPrestige = Mathf.Max(assassin.nowPrestige + prestige, 0);
                        assassin.privacy.Add("暗殺" + player.name + "成功獲得" + prestige + "點威望");
                        if (prestige == 3)
                        {
                            if (player.nowFaction == Faction.Sola)
                                assassin.peerage = Faction.Luna;
                            else if (player.nowFaction == Faction.Luna)
                                assassin.peerage = Faction.Sola;
                        }
                    }

                    // 遭受暗殺-N威望
                    Demote(indexPlayer); // 降為騎士失去繼任順位
                    player.nowPrestige = Mathf.Max(player.nowPrestige - prestige, 0); // 失去N點威望
                    player.privacy.Add("遭到" + player.assassin.Count + "名刺客暗殺失去" + prestige + "點威望");
                    listBeAssassinated.Add(indexPlayer); // 暗殺消息會於下一次王國傳聞公開
                    switch (player.action) // 無法執行後續行動
                    {
                        case Action.Defense: actionDefense.Remove(indexPlayer); break;
                        case Action.Conquer: actionConquer.Remove(indexPlayer); break;
                        case Action.Peerage: actionPeerage.Remove(indexPlayer); break;
                        case Action.Diplomacy: actionDiplomacy.Remove(indexPlayer); break;
                    }
                }
            }
        }
        void DefenseAndConquer()
        {
            string targetCastle = round % 2 == 0 ? "晝日城堡" : "夜月城堡";
            int conqueror = actionConquer.Count;
            int defender = actionDefense.Count;
            int powerConquer = conqueror;
            int powerDefense = defender;
            if (conqueror == 1)
            {
                if (partyPlayer[actionConquer[0] - 1].nowCharacter == Character.King)
                    powerConquer = 3;
            }
            if (defender == 1)
            {
                if (partyPlayer[actionDefense[0] - 1].nowCharacter == Character.King)
                    powerDefense = 3;
            }

            if (powerConquer > powerDefense)
            {
                kingdomNews.Add(targetCastle + "爭奪戰結果：征服者以" + powerConquer + "比" + powerDefense + "擊敗防禦者。");
                if (daynight == DayNight.晝)
                {
                    sola.castle++;
                    luna.castle--;
                    // 未參夜月城堡防禦的公爵降級
                    for (int i = 0; i < luna.duke.Count; i++)
                    {
                        int indexPlayer = luna.duke[i];
                        PlayerInfo player = partyPlayer[indexPlayer - 1];

                        if (player.action != Action.Defense)
                        {
                            Demote(indexPlayer); // 降為騎士失去繼任順位
                            player.nowPrestige = Mathf.Max(player.nowPrestige - defender, 0); // 失去N點威望
                            player.privacy.Add("未參與夜月城堡防禦失去" + defender + "點威望");
                            switch (player.action) // 無法執行後續行動
                            {
                                case Action.Diplomacy: actionDiplomacy.Remove(indexPlayer); break;
                            }
                        }
                    }
                }
                else if (daynight == DayNight.夜)
                {
                    luna.castle++;
                    sola.castle--;
                    // 未參晝日城堡防禦的公爵降級
                    for (int i = 0; i < sola.duke.Count; i++)
                    {
                        int indexPlayer = sola.duke[i];
                        PlayerInfo player = partyPlayer[indexPlayer - 1];

                        if (player.action != Action.Defense)
                        {
                            Demote(indexPlayer); // 降為騎士失去繼任順位
                            player.nowPrestige = Mathf.Max(player.nowPrestige - defender, 0); // 失去N點威望
                            player.privacy.Add("未參與晝日城堡防禦失去" + defender + "點威望");
                            switch (player.action) // 無法執行後續行動
                            {
                                case Action.Diplomacy: actionDiplomacy.Remove(indexPlayer); break;
                            }
                        }
                    }
                }
                // 征服成功
                for (int i = 0; i < conqueror; i++)
                {
                    int indexPlayer = actionConquer[i];
                    PlayerInfo player = partyPlayer[indexPlayer - 1];
                    player.nowPrestige += conqueror;
                    player.privacy.Add(targetCastle + "征服成功獲得" + conqueror + "點威望");
                    if (player.nowCharacter == Character.Knight)
                        player.peerage = (Faction)daynight;
                }
                // 防禦失敗
                for (int i = 0; i < defender; i++)
                {
                    int indexPlayer = actionDefense[i];
                    PlayerInfo player = partyPlayer[indexPlayer - 1];
                    player.nowPrestige = Mathf.Max(player.nowPrestige - defender, 0);
                    player.privacy.Add(targetCastle + "防禦失敗失去" + defender + "點威望");
                }
            }
            else if (conqueror > 0)
            {
                kingdomNews.Add(targetCastle + "爭奪戰結果：防禦者以" + powerDefense + "比" + powerConquer + "擊敗征服者。");
                if (daynight == DayNight.晝)
                {
                    // 未參夜月城堡征服的公爵降級
                    for (int i = 0; i < sola.duke.Count; i++)
                    {
                        int indexPlayer = sola.duke[i];
                        PlayerInfo player = partyPlayer[indexPlayer - 1];

                        if (player.action != Action.Conquer)
                        {
                            Demote(indexPlayer); // 降為騎士失去繼任順位
                            player.nowPrestige = Mathf.Max(player.nowPrestige - conqueror, 0); // 失去N點威望
                            player.privacy.Add("未參與夜月城堡征服失去" + conqueror + "點威望");
                            switch (player.action) // 無法執行後續行動
                            {
                                case Action.Diplomacy: actionDiplomacy.Remove(indexPlayer); break;
                            }
                        }
                    }
                }
                else if (daynight == DayNight.夜)
                {
                    // 未參晝日城堡征服的公爵降級
                    for (int i = 0; i < luna.duke.Count; i++)
                    {
                        int indexPlayer = luna.duke[i];
                        PlayerInfo player = partyPlayer[indexPlayer - 1];

                        if (player.action != Action.Conquer)
                        {
                            Demote(indexPlayer); // 降為騎士失去繼任順位
                            player.nowPrestige = Mathf.Max(player.nowPrestige - conqueror, 0); // 失去N點威望
                            player.privacy.Add("未參與晝日城堡征服失去" + conqueror + "點威望");
                            switch (player.action) // 無法執行後續行動
                            {
                                case Action.Diplomacy: actionDiplomacy.Remove(indexPlayer); break;
                            }
                        }
                    }
                }
                // 防禦成功
                for (int i = 0; i < defender; i++)
                {
                    int indexPlayer = actionDefense[i];
                    PlayerInfo player = partyPlayer[indexPlayer - 1];
                    player.nowPrestige += defender;
                    player.privacy.Add(targetCastle + "防禦成功獲得" + defender + "點威望");
                    if (player.nowCharacter == Character.Knight)
                    {
                        if (daynight == DayNight.晝)
                            player.peerage = Faction.Luna;
                        else if (daynight == DayNight.夜)
                            player.peerage = Faction.Sola;
                    }
                }
                // 征服失敗
                for (int i = 0; i < conqueror; i++)
                {
                    int indexPlayer = actionConquer[i];
                    PlayerInfo player = partyPlayer[indexPlayer - 1];
                    player.nowPrestige = Mathf.Max(player.nowPrestige - conqueror, 0);
                    player.privacy.Add(targetCastle + "征服失敗失去" + conqueror + "點威望");
                }
            }
            else
            {
                kingdomNews.Add(targetCastle + "似乎一切都很和平");
            }
            actionDefense.Clear();
            actionConquer.Clear();
        }
        void Peegage()
        {
            // 先處理雙重冊封問題
            int[] indexByKing = new int[2];
            bool allow = true;
            for (int i = 0; i < actionPeerage.Count; i++)
            {
                int indexPlayer = actionPeerage[i];
                PlayerInfo player = partyPlayer[indexPlayer - 1];
                int indexTarget = player.target;
                PlayerInfo target = partyPlayer[indexTarget - 1];

                target.peerageByKing = player.nowFaction;
                indexByKing[i] = indexTarget;
            }
            if (indexByKing[0] == indexByKing[1])
                allow = false;

            for (int i = 0; i < actionPeerage.Count; i++)
            {
                int indexPlayer = actionPeerage[i];
                PlayerInfo player = partyPlayer[indexPlayer - 1];
                int indexTarget = player.target;
                PlayerInfo target = partyPlayer[indexTarget - 1];

                if (!allow)
                {
                    player.privacy.Add("你與敵國皆欲冊封" + target.name + "為公爵，冊封無效");
                }
                else
                {
                    if (target.peerage == Faction.KocmocA)
                    {
                        player.nowPrestige++;
                        player.privacy.Add("冊封" + target.name + "為公爵獲得1點威望");
                        target.peerage = player.nowFaction;
                    }
                    else
                    {
                        if (target.peerage != player.nowFaction)
                            player.privacy.Add("無法冊封，" + target.name + "已因戰功晉升為敵方公爵");
                        else
                        {
                            player.nowPrestige++;
                            player.privacy.Add("冊封" + target.name + "為公爵獲得1點威望");
                        }
                    }
                }
            }
            actionPeerage.Clear();
            // 封爵
            for (int i = 0; i < partyPlayer.Count; i++)
            {
                int indexPlayer = i + 1;
                PlayerInfo player = partyPlayer[i];
                if (player.nowCharacter == Character.Knight)
                {
                    if (player.peerage == Faction.Sola)
                    {
                        player.nowFaction = Faction.Sola;
                        player.nowCharacter = Character.Duke;
                        luna.friends.Remove(indexPlayer);
                        if (!sola.duke.Contains(indexPlayer))
                            sola.duke.Add(indexPlayer);
                        player.privacy.Add("你被封為" + ((FactionCht)player.nowFaction).ToString() + "公爵");
                    }
                    else if (player.peerage == Faction.Luna)
                    {
                        player.nowFaction = Faction.Luna;
                        player.nowCharacter = Character.Duke;
                        sola.friends.Remove(indexPlayer);
                        if (!luna.duke.Contains(indexPlayer))
                            luna.duke.Add(indexPlayer);
                        player.privacy.Add("你被封為" + ((FactionCht)player.nowFaction).ToString() + "公爵");
                    }
                }
            }
        }
        void Diplomacy()
        {
            for (int i = 0; i < actionDiplomacy.Count; i++)
            {
                int indexPlayer = actionDiplomacy[i];
                PlayerInfo player = partyPlayer[indexPlayer - 1];
                int indexTarget = player.target;
                PlayerInfo target = partyPlayer[indexTarget - 1];

                player.privacy.Add(target.name + "的身份是" + ((FactionCht)target.nowFaction).ToString() + ((CharacterCht)target.nowCharacter).ToString());
            }
        }
        void WhoIsSuccessor()
        {
            if (sola.king == 0)
            {
                if (sola.successor != 0)
                {
                    Succeed(sola.successor, Faction.Sola);
                    partyPlayer[sola.king - 1].privacy.Add("晝日國王遭遇暗殺，你繼承了王位");
                }
                else
                {
                    if (sola.duke.Count > 0)
                    {
                        Succeed(sola.duke[0], Faction.Sola);
                        partyPlayer[sola.king - 1].privacy.Add("晝日國王遭遇暗殺，你繼承了王位");
                    }
                }
            }
            if (luna.king == 0)
            {
                if (luna.successor != 0)
                {
                    Succeed(luna.successor, Faction.Luna);
                    partyPlayer[luna.king - 1].privacy.Add("夜月國王遭遇暗殺，你繼承了王位");
                }
                else
                {
                    if (luna.duke.Count > 0)
                    {
                        Succeed(luna.duke[0], Faction.Luna);
                        partyPlayer[luna.king - 1].privacy.Add("夜月國王遭遇暗殺，你繼承了王位");
                    }
                }
            }
        }
        // 繼任王位
        void Succeed(int indexPlayer, Faction kingdom)
        {
            if (kingdom == Faction.Sola)
            {
                sola.king = indexPlayer;
                if (!sola.friends.Contains(indexPlayer))
                    sola.friends.Add(indexPlayer);
                luna.friends.Remove(indexPlayer);
            }
            else if (kingdom == Faction.Luna)
            {
                luna.king = indexPlayer;
                if (!luna.friends.Contains(indexPlayer))
                    luna.friends.Add(indexPlayer);
                sola.friends.Remove(indexPlayer);
            }
            if (sola.successor == indexPlayer)
                sola.successor = 0;
            if (sola.duke.Contains(indexPlayer))
                sola.duke.Remove(indexPlayer);
            if (luna.successor == indexPlayer)
                luna.successor = 0;
            if (luna.duke.Contains(indexPlayer))
                luna.duke.Remove(indexPlayer);
            PlayerInfo player = partyPlayer[indexPlayer - 1];
            player.nowFaction = kingdom;
            player.nowCharacter = Character.King;
        }
        // 降為騎士
        void Demote(int indexPlayer)
        {
            if (!sola.friends.Contains(indexPlayer))
                sola.friends.Add(indexPlayer);
            if (!luna.friends.Contains(indexPlayer))
                luna.friends.Add(indexPlayer);
            if (sola.king == indexPlayer)
                sola.king = 0;
            if (sola.successor == indexPlayer)
                sola.successor = 0;
            if (sola.duke.Contains(indexPlayer))
                sola.duke.Remove(indexPlayer);
            if (luna.king == indexPlayer)
                luna.king = 0;
            if (luna.successor == indexPlayer)
                luna.successor = 0;
            if (luna.duke.Contains(indexPlayer))
                luna.duke.Remove(indexPlayer);
            PlayerInfo player = partyPlayer[indexPlayer - 1];
            player.nowFaction = Faction.KocmocA;
            player.nowCharacter = Character.Knight;
        }
        // 遊戲結束
        void GameOver()
        {
            gameover = true;

            Debug.LogError("GameOver");
        }
        void IsGameOver()
        {
            // 雙退位成為唯一國王
            if (sola.king == luna.king)
            {
                PlayerInfo player = partyPlayer[sola.king - 1];
                int addtion = partyPlayer.Count * 6;
                player.nowPrestige += addtion;
                player.privacy.Add("你從城堡獲得" + addtion + "點威望");
                kingdomNews.Add("<color=yellow>" + player.name.ToString() + "成為卡斯摩沙唯一的國王</color>");
                GameOver();
            }
            // 一方王國失去所有城堡
            else if (sola.castle == 0)
            {
                PlayerInfo player;
                int lunaAddition = partyPlayer.Count * luna.castle;
                int lunaCount = luna.duke.Count + 1;
                // 計算夜月王國收益
                int part = Mathf.FloorToInt(lunaAddition / lunaCount);
                for (int i = 0; i < luna.duke.Count; i++)
                {
                    player = partyPlayer[luna.duke[i] - 1];
                    player.nowPrestige += part;
                    player.privacy.Add("你從城堡獲得" + part + "點威望");
                    lunaAddition -= part;
                }
                player = partyPlayer[luna.king - 1];
                player.nowPrestige += lunaAddition;
                player.privacy.Add("你從城堡獲得" + lunaAddition + "點威望");

                kingdomNews.Add("<color=yellow>夜月王國已取得卡斯摩沙所有城堡</color>");
                GameOver();
            }
            else if (luna.castle == 0)
            {
                PlayerInfo player;
                int solaAddition = partyPlayer.Count * sola.castle;
                int solaCount = sola.duke.Count + 1;
                // 計算夜月王國收益
                int part = Mathf.FloorToInt(solaAddition / solaCount);
                for (int i = 0; i < sola.duke.Count; i++)
                {
                    player = partyPlayer[sola.duke[i] - 1];
                    player.nowPrestige += part;
                    player.privacy.Add("你從城堡獲得" + part + "點威望");
                    solaAddition -= part;
                }
                player = partyPlayer[sola.king - 1];
                player.nowPrestige += solaAddition;
                player.privacy.Add("你從城堡獲得" + solaAddition + "點威望");

                kingdomNews.Add("<color=yellow>晝日王國已取得卡斯摩沙所有城堡</color>");
                GameOver();
            }
            // 一方國王死亡無繼承人
            else if (sola.king == 0)
            {
                PlayerInfo player;
                int lunaAddition = partyPlayer.Count * luna.castle;
                int knightAddition = partyPlayer.Count * sola.castle;
                int lunaCount = luna.duke.Count + 1;
                int knightCount = partyPlayer.Count - lunaCount;
                // 計算夜月王國收益
                int part = Mathf.FloorToInt(lunaAddition / lunaCount);
                for (int i = 0; i < luna.duke.Count; i++)
                {
                    player = partyPlayer[luna.duke[i] - 1];
                    player.nowPrestige += part;
                    player.privacy.Add("你從城堡獲得" + part + "點威望");
                    lunaAddition -= part;
                }
                player = partyPlayer[luna.king - 1];
                player.nowPrestige += lunaAddition;
                player.privacy.Add("你從城堡獲得" + lunaAddition + "點威望");

                // 計算騎士收益
                part = Mathf.FloorToInt(knightAddition / knightCount);
                for (int i = 0; i < partyPlayer.Count; i++)
                {
                    if (partyPlayer[i].nowFaction != Faction.Luna)
                    {
                        player = partyPlayer[i];
                        player.nowPrestige += part;
                        player.privacy.Add("你從城堡獲得" + part + "點威望");
                    }
                }
                kingdomNews.Add("<color=yellow>晝日王國已無國王與繼承人</color>");
                GameOver();
            }
            else if (luna.king == 0)
            {
                PlayerInfo player;
                int solaAddition = partyPlayer.Count * sola.castle;
                int knightAddition = partyPlayer.Count * luna.castle;
                int solaCount = sola.duke.Count + 1;
                int knightCount = partyPlayer.Count - solaCount;
                // 計算晝日王國收益
                int part = Mathf.FloorToInt(solaAddition / solaCount);
                for (int i = 0; i < sola.duke.Count; i++)
                {
                    player = partyPlayer[sola.duke[i] - 1];
                    player.nowPrestige += part;
                    player.privacy.Add("你從城堡獲得" + part + "點威望");
                    solaAddition -= part;
                }
                player = partyPlayer[sola.king - 1];
                player.nowPrestige += solaAddition;
                player.privacy.Add("你從城堡獲得" + solaAddition + "點威望");

                // 計算騎士收益
                part = Mathf.FloorToInt(knightAddition / knightCount);
                for (int i = 0; i < partyPlayer.Count; i++)
                {
                    if (partyPlayer[i].nowFaction != Faction.Sola)
                    {
                        player = partyPlayer[i];
                        player.nowPrestige += part;
                        player.privacy.Add("你從城堡獲得" + part + "點威望");
                    }
                }
                kingdomNews.Add("<color=yellow>夜月王國已無國王與繼承人</color>");
                GameOver();
            }
            // 失去所有騎士
            else if (sola.friends.Count + luna.friends.Count == partyPlayer.Count)
            {
                PlayerInfo player;
                int solaAddition = partyPlayer.Count * sola.castle;
                int solaCount = sola.duke.Count + 1;
                int part = Mathf.FloorToInt(solaAddition / solaCount);
                for (int i = 0; i < sola.duke.Count; i++)
                {
                    player = partyPlayer[sola.duke[i] - 1];
                    player.nowPrestige += part;
                    player.privacy.Add("你從城堡獲得" + part + "點威望");
                    solaAddition -= part;
                }
                player = partyPlayer[sola.king - 1];
                player.nowPrestige += solaAddition;
                player.privacy.Add("你從城堡獲得" + solaAddition + "點威望");

                int lunaAddition = partyPlayer.Count * luna.castle;
                int lunaCount = luna.duke.Count + 1;
                part = Mathf.FloorToInt(lunaAddition / lunaCount);
                for (int i = 0; i < luna.duke.Count; i++)
                {
                    player = partyPlayer[luna.duke[i] - 1];
                    player.nowPrestige += part;
                    player.privacy.Add("你從城堡獲得" + part + "點威望");
                    lunaAddition -= part;
                }
                player = partyPlayer[luna.king - 1];
                player.nowPrestige += lunaAddition;
                player.privacy.Add("你從城堡獲得" + lunaAddition + "點威望");

                kingdomNews.Add("<color=yellow>卡斯摩沙已失去所有騎士</color>");
                GameOver();
            }
            // 失去所有過往 = 所有人成為騎士
            else if (sola.king == 0 && luna.king == 0)
            {
                kingdomNews.Add("<color=yellow>卡斯摩沙已失去所有國王</color>");
                GameOver();
            }
        }
        void OnlyKing(int indexPlayer)
        {

        }
        // 關注特定行動選項
        void FocusActionOption(GameObject option)
        {
            for (int i = 0; i < actionOption.Length; i++)
            {
                if (actionOption[i] != option)
                    actionOption[i].GetComponent<CanvasGroup>().alpha = 0.37f;
                else
                    actionOption[i].GetComponent<CanvasGroup>().alpha = 1.0f;
            }
        }
        // 關閉所有行動選項
        void CloseAllActionOption()
        {
            for (int i = 0; i < actionOption.Length; i++)
            {
                actionOption[i].GetComponent<CanvasGroup>().alpha = 1.0f;
                actionOption[i].SetActive(false);
            }
        }
        // 關閉所有目標選項
        void CloseAllTargetOption()
        {
            for (int i = 0; i < targetOption.Length; i++) { targetOption[i].SetActive(false); }
        }

        //if (partyPlayer[nowIndex - 1].character == Character.King)
        //{
        //    if ((partyPlayer[otherPlayer - 1].faction == partyPlayer[nowIndex - 1].faction) || (partyPlayer[otherPlayer - 1].faction == Faction.Stella))
        //    {

        //    }
        //}
        //else
        //    return;

        /*
盟友：玩家E、玩家X、玩家F
玩家Z的身份為晝日公國公爵
處決玩家M，+2威望
遭到處決，-3威望
玩家M守衛成功，+3威望
守衛無果
玩家M暗殺成功，+3威望
玩家M暗殺失敗，-1威望
遭到暗殺，-3威望
晝日公國防禦成功，+3威望
晝日公國防禦失敗，-3威望
晝日公國防禦無果
晝日公國征服成功，+3威望
晝日公國征服失敗，-3威望         
         */
    }
}