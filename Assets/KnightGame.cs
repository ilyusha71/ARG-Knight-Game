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
    public class PlayerInfo
    {
        public Player name;
        public Faction nowFaction;
        public Character nowCharacter;
        public int nowPrestige;
        public Faction lastFaction;
        public Character lastCharacter;
        public int lastPrestige;

        public List<string> privacy = new List<string>(); // 私人訊息

        public bool infoConfirm =false;
        public bool actionConfirm =false;
        public int target = 0;
        public Action action = Action.None;
        public Action hasBeen = Action.None;
        public List<int> guard = new List<int>();
        public List<int> assassin = new List<int>();
        public Faction peerageByKing = Faction.KocmocA;
        public Faction peerage = Faction.KocmocA;

        public void RecordLastInfo()
        {
            lastFaction = nowFaction;
            lastCharacter = nowCharacter;
            lastPrestige = nowPrestige;
        }
        public void ClearPast()
        {
            infoConfirm = false;
            actionConfirm = false;
            target = 0;
            action = Action.None;
            hasBeen = Action.None;
            guard.Clear();
            assassin.Clear();
            peerageByKing = Faction.KocmocA;
            peerage = Faction.KocmocA;
        }
    }
    public class Kingdom
    {
        public int king;
        public int successor;
        public List<int> duke = new List<int>(); // 如果沒有繼承人，會依此列表依序繼承
        public List<int> friends = new List<int>(); // 如果是大公會根據此列表篩選對象
        public string kingdomFriendsInfo;
        public int castle = 3;

        public string GetKingdomFriendsInfo()
        {
            string info = "你的盟友：";
            for (int i = 0; i < friends.Count; i++)
            {
                if (friends[i] != king)
                {
                    info += friends[i].ToString();
                    if (i < friends.Count - 1)
                        info += ",";
                }
            }
            return info;
        }
    }
    public enum GameStep
    {
        Ready = 0,
        KingdomNews = 1,
        FreeTime = 2,
        Action = 3,
        Gameover = 4,
    }
    public enum DayNight
    {
        晝 = 1,
        夜 = 0,
    }
    public partial class KnightGame : MonoBehaviour 
    {
        public List<PlayerInfo> partyPlayer = new List<PlayerInfo>();
        public Kingdom sola = new Kingdom();
        public Kingdom luna = new Kingdom();
        [Header("RFID")]
        public RFID_State stateRFID = RFID_State.Ready;
        public float delayRFID;
        [Header("Function")]
        public GameStep step = GameStep.Ready;
        private int round = 2;
        public DayNight daynight;
        public float countdownTimer;
        public int nowIndex;
        public List<string> kingdomNews = new List<string>();
        public List<int> listBeExcuted = new List<int>();
        public List<int> listBeAssassinated = new List<int>();
        bool gameover = false;
        [Header("UGUI")]
        public GameObject panelPlayerCount;
        public Text textCountdownTimer;
        [Header("Audio")]
        public AudioSource source;
        public AudioClip[] clips;
        [Header("Kingdom Info")]
        public Image background;
        public Sprite[] spriteBG;
        public Text textDate;
        public Text textSolaCastleCount;
        public Text textSolaPlayerCount;
        public Text textLunaCastleCount;
        public Text textLunaPlayerCount;
        public Text[] textAllPlayerName;
        public Text[] textAllPlayerPrestige;
        public Transform panelKingdomNews;
        public Text textStep;
        public Text textKingdomNews;
        [Header("Player Info")]
        public GameObject panelPlayerInfo;
        public Text textPlayerIndex;
        public Text textPlayerNowFaction;
        public Text textPlayerNowChracter;
        public Text textPlayerNowPrestige;
        public Text textPlayerLastFaction;
        public Text textPlayerLastChracter;
        public Text textPlayerLastPrestige;
        public Text textOperation;
        public Text textPrivacyMessage;
        public GameObject[] actionOption;
        public GameObject[] targetOption;


        void ArduinoMsg()
        {
            for (int i = 0; i < ArduinoController.msgQueue.Count; i++)
            {
                string msg = ArduinoController.msgQueue.Dequeue();
                if (msg.Contains("is Player "))
                {
                    nowIndex = int.Parse(msg.Replace("is Player ", ""));
                    stateRFID = RFID_State.Reading;
                    delayRFID = 0;
                }
                else
                {
                    //Debug.LogWarning(msg);
                    stateRFID = RFID_State.Ready;
                }
            }
        }
        private void Awake()
        {
            if (ArduinoController.instance == null)
            {
                PlayerPrefs.SetInt("lastScene", SceneManager.GetActiveScene().buildIndex + 100);
                SceneManager.LoadScene("Arduino Controller");
                return;
            }
            InitializeKingdom();
            HidePlayerInfo();
        }
        private void Start()
        {
            Cursor.visible = true;
        }
        private void Update()
        {
            if (Input.GetButtonDown("Fire1"))
                Cursor.visible = true;
            if (ArduinoController.msgQueue.Count > 0)
                ArduinoMsg();
            if (stateRFID == RFID_State.Reading)
            {
                delayRFID += Time.deltaTime;
                if (delayRFID > 0.3f)
                    stateRFID = RFID_State.Missing;
            }
            if (Input.GetKeyDown(KeyCode.Return))
                HidePlayerInfo();
            // 王國傳聞發佈
            if (Input.GetKeyDown(KeyCode.K))
                KingdomNews();
            // 進入自由交流時間
            if (Input.GetKeyDown(KeyCode.F))
                FreeTime();
            // 進入行動安排時間
            if (Input.GetKeyDown(KeyCode.A))
                ArrangeAction();
            // 行動計算結果
            if (Input.GetKeyDown(KeyCode.R))
                CalculateAction();

            if (step == GameStep.Action)
                PlayerAction();
            else
            {
                PrivacyMessage();
                if (step == GameStep.KingdomNews)
                {
                    bool hasConfirmed = true;
                    for (int i = 0; i < partyPlayer.Count; i++)
                    {
                        if (!partyPlayer[i].infoConfirm)
                            hasConfirmed = false;
                    }
                    if (hasConfirmed)
                        FreeTime();
                }
                else if (step == GameStep.FreeTime)
                {
                    countdownTimer = Mathf.Max(countdownTimer - Time.deltaTime, 0);
                    textCountdownTimer.text = "" + (int)countdownTimer;
                    if (countdownTimer == 0)
                        ArrangeAction();
                }
            }
        }


        // 初始化王國
        void InitializeKingdom()
        {
            int month = Mathf.CeilToInt(round * 0.5f);
            daynight = (DayNight)(round % 2);
            source.PlayOneShot(clips[(int)daynight]);
            background.sprite = spriteBG[(int)daynight];
            textDate.text = "163年 " + month + "月 " + daynight.ToString();
            textSolaPlayerCount.text = "0";
            textLunaPlayerCount.text = "0";
            textSolaCastleCount.text = "0";
            textLunaCastleCount.text = "0";
            textStep.text = "挑戰者決定";
            textKingdomNews.text = "請主持人確認本場遊戲挑戰者";
            for (int i = 0; i < 12; i++)
            {
                textAllPlayerName[i].transform.parent.gameObject.SetActive(false);
            }
            panelPlayerCount.SetActive(true);
        }
        // 設定玩家數量
        public void SetPlayerCount(int countPlayer)
        {
            // 玩家角色生成
            panelPlayerCount.SetActive(false);

            // 抽取【晝日國王】代號
            List<int> listNumberPlayer = new List<int>();
            for (int i = 1; i <= countPlayer; i++) { listNumberPlayer.Add(i); }
            int whoIsTheKingOfSola = listNumberPlayer[Random.Range(1, listNumberPlayer.Count+1)-1];
            listNumberPlayer.Remove(whoIsTheKingOfSola);
            // 抽取【夜月國王】代號
            int whoIsTheKingOfLuna = listNumberPlayer[Random.Range(1, listNumberPlayer.Count+1)-1];
            listNumberPlayer.Remove(whoIsTheKingOfLuna);
            // 抽取【晝日公爵】代號
            int whoIsTheDukeOfSola = listNumberPlayer[Random.Range(1, listNumberPlayer.Count + 1) - 1];
            listNumberPlayer.Remove(whoIsTheDukeOfSola);
            // 抽取【夜月公爵】代號
            int whoIsTheDukeOfLuna = listNumberPlayer[Random.Range(1, listNumberPlayer.Count + 1) - 1];
            listNumberPlayer.Remove(whoIsTheDukeOfLuna);
            listNumberPlayer.Clear();

            // 初始化角色
            for (int i = 1; i <= countPlayer; i++)
            {
                PlayerInfo player = new PlayerInfo();
                player.name = (Player)i;
                textAllPlayerName[i - 1].text = player.name.ToString();
                if (i == whoIsTheKingOfSola)
                {
                    player.nowFaction = Faction.Sola;
                    player.nowCharacter = Character.King;
                    Debug.LogWarning("Player " + i + " is the King of Sola.");
                    player.lastFaction = Faction.SolaDuchy;
                    player.lastCharacter = Character.Duke;
                    sola.king = i;
                    sola.friends.Add(i);
                }
                else if (i == whoIsTheKingOfLuna)
                {
                    player.nowFaction = Faction.Luna;
                    player.nowCharacter = Character.King;
                    Debug.LogWarning("Player " + i + " is the King of Luna.");
                    player.lastFaction = Faction.LunaDuchy;
                    player.lastCharacter = Character.Duke;
                    luna.king = i;
                    luna.friends.Add(i);
                }
                else if (i == whoIsTheDukeOfSola)
                {
                    player.nowFaction = Faction.Sola;
                    player.nowCharacter = Character.Duke;
                    Debug.LogWarning("Player " + i + " is the Duke of Sola.");
                    player.lastFaction = Faction.KocmocA;
                    player.lastCharacter = Character.Duke;
                    sola.duke.Add(i);
                    sola.friends.Add(i);
                }
                else if (i == whoIsTheDukeOfLuna)
                {
                    player.nowFaction = Faction.Luna;
                    player.nowCharacter = Character.Duke;
                    Debug.LogWarning("Player " + i + " is the Duke of Luna.");
                    player.lastFaction = Faction.KocmocA;
                    player.lastCharacter = Character.Duke;
                    luna.duke.Add(i);
                    luna.friends.Add(i);
                }
                else
                {
                    player.nowFaction = Faction.KocmocA;
                    player.nowCharacter = Character.Knight;
                    Debug.Log("Player " + i + " is the Knight of KocmocA.");
                    player.lastFaction = Faction.KocmocA;
                    player.lastCharacter = Character.Knight;
                    sola.friends.Add(i);
                    luna.friends.Add(i);
                }
                player.nowPrestige = 10 + countPlayer;
                player.lastPrestige = 0;
                textAllPlayerPrestige[i-1].text = "0";
                player.privacy.Add("你是一名卡斯摩沙王國的騎士\n現在王國已分裂\n是時候加入王位爭奪了");
                partyPlayer.Add(player);
            }
            StartCoroutine(ShowPlayer());

            // 重設國王與公爵私人訊息
            partyPlayer[whoIsTheKingOfSola-1].privacy.Clear();
            partyPlayer[whoIsTheKingOfSola - 1].privacy.Add("卡斯摩沙王國已無繼承人\n你宣佈自己為卡斯摩沙唯一的統治者\n晝日王國 - 偉大的國王");
            partyPlayer[whoIsTheKingOfSola - 1].privacy.Add(sola.GetKingdomFriendsInfo());
            partyPlayer[whoIsTheKingOfLuna-1].privacy.Clear();
            partyPlayer[whoIsTheKingOfLuna - 1].privacy.Add("卡斯摩沙王國已無繼承人\n你宣佈自己為卡斯摩沙唯一的統治者\n夜月王國 - 偉大的國王");
            partyPlayer[whoIsTheKingOfLuna - 1].privacy.Add(luna.GetKingdomFriendsInfo());
            partyPlayer[whoIsTheDukeOfSola - 1].privacy.Clear();
            partyPlayer[whoIsTheDukeOfSola - 1].privacy.Add("卡斯摩沙王國已分裂\n你為了保留自己的爵位\n投靠了晝日王國");
            partyPlayer[whoIsTheDukeOfLuna - 1].privacy.Clear();
            partyPlayer[whoIsTheDukeOfLuna - 1].privacy.Add("卡斯摩沙王國已分裂\n你為了保留自己的爵位\n投靠了夜月王國");

            // 重設王國傳聞
            kingdomNews.Clear();
            kingdomNews.Add("卡斯摩沙王國的國王驟然逝世，但是國王沒有子嗣成為繼承人，");
            kingdomNews.Add("此時晝日公爵與夜月公爵均宣稱接到密詔繼承王位，");
            kingdomNews.Add("一場席捲卡斯摩沙的王位爭奪戰即將爆發！");
        }
        IEnumerator ShowPlayer()
        {
            string news = "本場遊戲挑戰者為\n";
            int cln = Mathf.CeilToInt(partyPlayer.Count * 0.5f);
            textAllPlayerName[0].transform.parent.parent.GetComponent<GridLayoutGroup>().constraintCount = cln;
            for (int i = 0; i < partyPlayer.Count; i++)
            {
                yield return new WaitForSeconds(0.37f);
                source.PlayOneShot(clips[2]);
                textAllPlayerName[i].transform.parent.gameObject.SetActive(true);
                news += partyPlayer[i].name.ToString();
                news += ",";
                if (i == cln - 1)
                    news += "\n";
                textKingdomNews.text = news;
            }
            yield return new WaitForSeconds(1.0f);
            if (step == GameStep.Ready)
                panelKingdomNews.DOLocalMoveY(panelKingdomNews.localPosition.y - 596, 0.7f).SetEase(Ease.InBack).OnComplete(OpenKingdomNews);
        }


        public void ChangePrestige()
        {
            if (gameover && nowIndex != 0)
            {
                source.PlayOneShot(clips[6]);
                partyPlayer[nowIndex - 1].nowPrestige += 2;
                textAllPlayerPrestige[nowIndex - 1].text = "" + partyPlayer[nowIndex - 1].nowPrestige;
            }
        }


    }
    public enum RFID_State
    {
        Ready = 0,
        Reading = 1,
        Missing = 2,
    }

    public enum Faction
    {
        Luna = 0,
        Sola = 1,
        SolaDuchy = 11,
        LunaDuchy = 10,
        KocmocA = 99,
    }
    public enum Character
    {
        Knight = 1,
        Duke = 2,
        King = 3,
    }
    public enum Action
    {
        None = 0,
        Abdicate = 1,
        Execute = 2,
        Imprison =3,
        Guard = 4,
        Assassinate = 5,
        Defense = 6,
        Conquer = 7,
        Peerage = 8,
        Diplomacy = 9,
    }
    public enum Player
    {
        雙魚1= 1,
        寶瓶2 = 2,
        摩羯3 = 3,
        人馬4 = 4,
        天蝎5 = 5,
        天秤6 = 6,
        室女7 = 7,
        獅子8 = 8,
        巨蟹9 = 9,
        雙子10 = 10,
        金牛11 = 11,
        白羊12 = 12,
    }
    public enum FactionCht
    {
        夜月王國 = 0,
        晝日王國 = 1,
        晝日公國 = 11,
        夜月公國 = 10,
        卡斯摩沙 = 99,
    }
    public enum CharacterCht
    {
        騎士 = 1,
        公爵 = 2,
        國王 = 3,
    }
}