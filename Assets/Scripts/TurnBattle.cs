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

    [Header("파티 정보 (1~4번 슬롯)")]
    [SerializeField] private GameObject[] partySlots; // 파티원이 없으면 UI를 통째로 끄기 위한 배열
    [SerializeField] private TextMeshProUGUI[] partyNameTexts; // 파티원 이름 텍스트 배열
    [SerializeField] private Slider[] partyHpBars; // 파티원 체력바 배열

    [Header("데미지 텍스트")]
    [SerializeField] private DamageText damageTextPrefab; // 아까 만든 프리팹을 넣을 곳
    [SerializeField] private Transform damageCanvas; // 데미지 텍스트가 생성될 부모 캔버스

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

    public void SetActionPanel(bool active)
    {
        attackButton.interactable = active;
        skillButton.interactable = active;
        itemButton.interactable = active;
        fleeButton.interactable = active;
    }
}