using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TurnBattleUI : MonoBehaviour
{
    [Header("행동 선택")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button skillButton;
    [SerializeField] private Button itemButton;
    [SerializeField] private Button fleeButton;

    [Header("턴 표시")]
    [SerializeField] private TextMeshProUGUI turnText;

    [Header("적 정보")]
    [SerializeField] private TextMeshProUGUI enemyNameText; // 적 이름 텍스트 추가!
    [SerializeField] private Slider enemyHpBar;
    [SerializeField] private Image enemyImage;

    [Header("파티 정보 (1~4번 슬롯)")]
    [SerializeField] private GameObject[] partySlots; // 파티원이 없으면 UI를 통째로 끄기 위한 배열
    [SerializeField] private TextMeshProUGUI[] partyNameTexts; // 파티원 이름 텍스트 배열
    [SerializeField] private Slider[] partyHpBars; // 파티원 체력바 배열
    [SerializeField] private Slider[] partyMpBars;
    [SerializeField] private Image[] partyImages;


    [Header("데미지 텍스트")]
    [SerializeField] private DamageText damageTextPrefab; // 아까 만든 프리팹을 넣을 곳
    [SerializeField] private Transform damageCanvas; // 데미지 텍스트가 생성될 부모 캔버스

    [Header("액션 버튼 텍스트")]
    [SerializeField] private TextMeshProUGUI[] actionButtonTexts;

    private void Start()
    {
        attackButton.onClick.AddListener(() => BattleManager.Instance.OnAction("attack"));
        skillButton.onClick.AddListener(() => BattleManager.Instance.OnAction("skill"));
        itemButton.onClick.AddListener(() => BattleManager.Instance.OnAction("item"));
        fleeButton.onClick.AddListener(() => BattleManager.Instance.OnAction("flee"));
    }

    public void SetTurnText(string text)
    {
        turnText.text = text;
    }

    // 적의 이름과 체력을 한 번에 업데이트하는 함수
    public void SetEnemyUI(string name, float current, float max)
    {
        enemyNameText.text = name;
        enemyHpBar.maxValue = max;
        enemyHpBar.value = current;

     }

    public void SetEnemyImage(string id)
    {
        Sprite sprite = Resources.Load<Sprite>("Sprites/Enemy/" + id);
        if (sprite != null)
            enemyImage.sprite = sprite;
    }

    // 파티 리스트를 넘겨받아 있는 명수만큼만 UI를 켜고 이름을 세팅하는 함수
    public void SetPartyUI(List<BattleUnit> players)
    {
        for (int i = 0; i < partySlots.Length; i++)
        {
            if (i < players.Count) // 파티원이 존재하는 슬롯
            {
                partySlots[i].SetActive(true); // 슬롯 켜기
                partyNameTexts[i].text = players[i].Name; // 이름 적용
                partyHpBars[i].maxValue = players[i].MaxHp; // 최대 체력 적용
                partyHpBars[i].value = players[i].CurrentHp; // 현재 체력 적용
                partyMpBars[i].maxValue = players[i].MaxMp;
                partyMpBars[i].value = players[i].CurrentMp;

                Sprite sprite = Resources.Load<Sprite>("Sprites/Companion/" + players[i].Id);
                if (sprite != null)
                    partyImages[i].sprite = sprite;
            }
            else // 파티원이 비어있는 슬롯 (예: 파티가 3명뿐일 때 4번째 슬롯)
            {
                partySlots[i].SetActive(false); // 슬롯 숨기기
            }
        }
    }





    public void SpawnDamageText(float damage, bool isEnemy, int partyIndex = 0)
    {
        // 1. 프리팹을 생성해서 캔버스 안에 넣음
        DamageText dmgText = Instantiate(damageTextPrefab, damageCanvas);

        // 2. 누구를 때렸는지에 따라 생성 위치를 다르게 설정
        if (isEnemy)
        {
            // 적 체력바 위치에서 살짝 위로 띄워서 생성
            dmgText.transform.position = enemyNameText.transform.position + Vector3.up * 50f;
        }
        else
        {
            // 맞은 파티원의 슬롯 위치에서 생성
            dmgText.transform.position = partySlots[partyIndex].transform.position + Vector3.up * 20f;
        }

        // 3. 데미지 수치 전달하여 애니메이션 시작!
        dmgText.Setup(damage);
    }
    public void ShowDefaultActions()
    {
        actionButtonTexts[0].text = "공격";
        actionButtonTexts[1].text = "스킬";
        actionButtonTexts[2].text = "아이템";
        actionButtonTexts[3].text = "도망";

        attackButton.onClick.RemoveAllListeners();
        skillButton.onClick.RemoveAllListeners();
        itemButton.onClick.RemoveAllListeners();
        fleeButton.onClick.RemoveAllListeners();

        attackButton.onClick.AddListener(() => BattleManager.Instance.OnAction("attack"));
        skillButton.onClick.AddListener(() => BattleManager.Instance.OnAction("skill"));
        itemButton.onClick.AddListener(() => BattleManager.Instance.OnAction("item"));
        fleeButton.onClick.AddListener(() => BattleManager.Instance.OnAction("flee"));
    }

    public void ShowSkillActions(List<SkillData> skills)
    {
        Button[] buttons = { attackButton, skillButton, itemButton, fleeButton };

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();

            if (i < skills.Count)
            {
                int index = i;
                actionButtonTexts[i].text = skills[i].Name;
                buttons[i].onClick.AddListener(() => BattleManager.Instance.UseSkill(skills[index]));
            }
            else
            {
                actionButtonTexts[i].text = "뒤로";
                buttons[i].onClick.AddListener(() => ShowDefaultActions());
            }
        }
    }
    public void SetActionPanel(bool active)
    {
        attackButton.interactable = active;
        skillButton.interactable = active;
        itemButton.interactable = active;
        fleeButton.interactable = active;
    }
}