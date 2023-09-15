using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //인게임에 있는 모든 ui요소를 불러오는 거임
    public GameObject menuCam;
    public GameObject gameCam;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject stageZone;
    public Player player; //Player.cs를 가져 온것. 퍼블릭만 여기서 쓸수 있다
    public Boss boss;
    public int stage; //스테이지 인트
    public float playTime; //플레이시간 플루트값 
    public bool isBattle; //지금 배틀 중인가 불값 
    public int enemyCntA; //적 몇마리 남았나 체크용 
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;
    //Enemy리스폰 배열
    public Transform[] enemyZones;
    //Enemy오브젝트 배열
    public GameObject[] enemies; //인덱스 0부터 EnemyA 가 담겨 있다. 
    //스테이지에 비례한 int값이 들어감. 이것은 Enemy가 리스폰횟수 난이도를 높이기 위함임.
    public List<int> enemyList; 

    //판넬 ui 가져오기 
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public Text maxScoreText;//메뉴판넬에서 가운데 최고점수 텍스트
    public Text curScoreText;
    public Text bestText;

    //게임 판넬 정보 가져오기
    public Text scoreText; //스코어 
    public Text stageText; //스테이지 
    public Text playTimeText; //플레이시간 
    public Text playerHealthText; //체력
    public Text playerAmmoText; //탄약
    public Text playerCoinText; //코인

    //장비 가져오기
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;

    //오른쪽 하단 적 몇명죽였나 표시ui가져오기
    public Text enemyAText;
    public Text enemyBText;
    public Text enemyCText;

    //보스피는 보스가 나올때만 rect로 이동 시킬거임
    public RectTransform bossHealthG;
    public RectTransform bossHealthBar;//이것은 체력이 줄어드는 애임

    private void Awake()
    {
        //List 형식이기 때문에 초기화를 해준다
        enemyList = new List<int>();
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("maxScore"));

        if (PlayerPrefs.HasKey("maxScore"))
            PlayerPrefs.SetInt("maxScore", 0);
    }
    //게임시작버튼 눌렀을때
    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }
    //스테이지 시작
    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        stageZone.SetActive(false);

        //애너미리스폰 활성화
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true;
        StartCoroutine(InBattle());
    }
   
    //배틀 코루틴
    IEnumerator InBattle()
    {
        if (stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instE = Instantiate(enemies[3], enemyZones[3].position, enemyZones[3].rotation);
            Enemy e = instE.GetComponent<Enemy>();
            e.target = player.transform;
            e.manager = this;
            boss = instE.GetComponent<Boss>();
        }
        else
        {
            //애너미 리스트에 랜덤값을 넣는다. 스테이지가 올라갈수록 더 많은 값을 넣을것이다.
            for(int index=0; index<stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);
                //리스트에 적이 할당될때마다 카운트가 올라간다.
                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }
            //리스트에 들어가 있는 값만큼 없어질때까지 소환한다.
            while (enemyList.Count > 0)
            {
                int ranZ = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZ].position, enemyZones[ranZ].rotation);
                //애너미를 인스턴트하고 타겟을 플레이어로 붙혀준다.
                Enemy e = instantEnemy.GetComponent<Enemy>();
                e.target = player.transform;
                //애너미에게 게임매니저를 붙혀야한다. 
                e.manager = this;
                //리스트 다시 삭제
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(3f);

            }
        }

        //남은 몬스터 숫자를 검사하는 while문 추가 
        while(enemyCntA+ enemyCntB+ enemyCntC+ enemyCntD > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        boss = null;
        StageEnd();
    }
    //스테이지 종료
    public void StageEnd()
    {
        //플레이어 원위치
        player.transform.position = Vector3.up * 0.8f;
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        stageZone.SetActive(true);

        //애너미리스폰 not 활성화
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++;
    }
    //게임종료
    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoreText.text;

        int maxScore = PlayerPrefs.GetInt("maxScore");
        if(player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("maxScore", player.score);
        }
    }
    public void ReStart()
    {
        SceneManager.LoadScene(0);
    }
    //시간계산
    void Update()
    {
        if(isBattle)
        playTime += Time.deltaTime;    
    }
    //lateUpdate()는 업뎃이 끝난후 호출되는 생명주기
    void LateUpdate()
    {
        //스코어를 업뎃할것이!
        scoreText.text = string.Format("{0:n0}", player.score);
        //스테이지 표시 ui
        stageText.text = "STAGE " + stage;
        //배틀시간 표시 ui
        int hour = (int)(playTime / 3600); //3600초는 1시간이다.
        int min = (int)((playTime - hour*3600)/ 60);//1시간을 60으로 나누면 분이된다.
        int second = (int)(playTime % 60);

        playTimeText.text = string.Format("{0:00}", hour) +":"+ string.Format("{0:00}", min) + ":"+
            string.Format("{0:00}", second);

        //체력ui = 플레이어가 가지고 있는 체력
        playerHealthText.text = player.health + " / " + player.maxHealth;
        playerCoinText.text = string.Format("{0:n0}", player.coin);
        if(player.equipObj == null)
        {
            playerAmmoText.text = "- / " + player.ammo;
        }else if(player.equipObj.weaponType == Weapon.type.melee)
        {
            playerAmmoText.text = "- / " + player.ammo;
        }else
        {
            playerAmmoText.text = player.equipObj.curAmmo+" / " + player.ammo;

        }
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapon[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapon[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapon[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenade > 0 ? 1 : 0);

        enemyAText.text = enemyCntA.ToString();
        enemyBText.text = enemyCntB.ToString();
        enemyCText.text = enemyCntC.ToString();

        //boss
        if (boss != null)
        {
            bossHealthG.anchoredPosition = Vector3.down * -720;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthG.anchoredPosition = Vector3.up * 230;

        }
    }

}
