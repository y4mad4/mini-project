using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [SerializeField] private TurnBattleUI battleUI;
    [SerializeField] private GameObject _turnBattleCanvas;

    private List<BattleUnit> _turnOrder = new List<BattleUnit>();
    private List<BattleUnit> _players = new List<BattleUnit>();
    private List<BattleUnit> _enemies = new List<BattleUnit>();
    private int _currentTurnIndex;
    private BattleUnit _currentUnit;
    private EnemyUnit _currentEnemy;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartBattle(string enemyId)
    {
       

        // 캔버스가 확실히 확인되었으니 켜기
        _turnBattleCanvas.SetActive(true);

        // 1. 플레이어 생성 (방어 코드 추가)
        PlayerData playerData = DataManager.Instance.GetPlayer("player_01");
        if (playerData != null)
        {
            _players.Add(new PlayerUnit(playerData));
        }
        else
        {
            Debug.LogError("플레이어 데이터를 찾을 수 없습니다!");
            return; // 플레이어가 없으면 전투 진행 불가
        }

        // 2. 동료 생성 (★ 빈 데이터 무시하기 적용)
        for (int i = 1; i <= 3; i++)
        {
            CompanionData companionData = DataManager.Instance.GetCompanion($"companion_0{i}");
            if (companionData == null) continue; // 데이터가 없으면 뻗지 말고 다음 번호로!

            _players.Add(new CompanionUnit(companionData));
        }

        // 3. 적 생성
        EnemyData enemyData = DataManager.Instance.GetEnemy(enemyId);
        if (enemyData == null)
        {
            Debug.LogError($"{enemyId} 적 데이터를 찾을 수 없습니다!");
            return;
        }
        _currentEnemy = new EnemyUnit(enemyData);
        _enemies.Add(_currentEnemy);
        // 이미지 로드
        battleUI.SetEnemyImage(enemyData.Id);
        battleUI.SetPartyImages(playerData.Id,
            DataManager.Instance.GetCompanion("companion_01").Id,
            DataManager.Instance.GetCompanion("companion_02").Id,
            DataManager.Instance.GetCompanion("companion_03").Id);

        // 턴 순서 정렬
        _turnOrder.AddRange(_players);
        _turnOrder.AddRange(_enemies);
        _turnOrder.Sort((a, b) =>
        {
            if (a.Speed == b.Speed) return Random.Range(0, 2) == 0 ? 1 : -1;
            return b.Speed.CompareTo(a.Speed);
        });

        // 4. UI 초기화 (★ 주의 요망 구역)
        // 수정 전
        // battleUI.SetEnemyHp(_currentEnemy.CurrentHp, _currentEnemy.MaxHp);
        // if (_players.Count == 4) { battleUI.SetPartyHp(_players[0], ...); }

        // 수정 후
        // 4. UI 초기화
        battleUI.SetEnemyUI(_currentEnemy.Name, _currentEnemy.CurrentHp, _currentEnemy.MaxHp);
        battleUI.SetPartyUI(_players); // 리스트를 통째로 던져줌!

        _currentTurnIndex = 0;
        Debug.Log("전투 시작!");
        StartTurn();
    }

    private void StartTurn()
    {
        _turnBattleCanvas.SetActive(true);

        while (!_turnOrder[_currentTurnIndex].IsAlive)
            _currentTurnIndex = (_currentTurnIndex + 1) % _turnOrder.Count;

        _currentUnit = _turnOrder[_currentTurnIndex];
        battleUI.SetTurnText($"{_currentUnit.Name} 의 턴!");

        if (_currentUnit is EnemyUnit)
        {
            StartCoroutine(EnemyTurnRoutine());
            return;
        }

        battleUI.SetActionPanel(true);
    }

    // ★ 수정됨: void 대신 IEnumerator를 쓰면 중간에 시간을 멈출 수 있습니다.
    private IEnumerator EnemyTurnRoutine()
    {
        battleUI.SetActionPanel(false);

        // 1. "적의 턴!" 글씨를 플레이어가 읽을 수 있도록 1초 기다림
        yield return new WaitForSeconds(2.0f);

        // EnemyTurnRoutine 코루틴 내부
        List<BattleUnit> alive = _players.FindAll(u => u.IsAlive);
        if (alive.Count > 0)
        {
            BattleUnit target = alive[Random.Range(0, alive.Count)];
            float damage = _currentEnemy.Attack; // 공격력 저장

            target.TakeDamage(damage);

            // ★ 추가: 적이 공격했을 때 아군 머리 위로 숫자 띄우기
            int targetIndex = _players.IndexOf(target); // 아군 리스트 중 몇 번째인지 찾기
            battleUI.SpawnDamageText(damage, false, targetIndex); // false는 적이 아님(아군)을 의미

            battleUI.SetPartyUI(_players);
            battleUI.SetTurnText($"{target.Name}이(가) {damage}의 피해를 입었다!");
        }

        // 2. 아군이 맞아서 체력이 깎인 모습을 확인할 수 있도록 1초 더 기다림
        yield return new WaitForSeconds(1.0f);

        // 대기 시간이 모두 끝나면 비로소 다음 턴으로 넘김
        EndTurn();
    }

    public void OnAction(string action)
    {
        battleUI.SetActionPanel(false);

        switch (action)
        {
            case "attack":
                // 1. 데미지 계산
                float damage = _currentUnit.Attack;

                // 2. 적에게 데미지 적용
                _currentEnemy.TakeDamage(damage);

                // 3. 적 체력바 UI 갱신
                battleUI.SetEnemyUI(_currentEnemy.Name, _currentEnemy.CurrentHp, _currentEnemy.MaxHp);

                // 4. ★ 핵심: 데미지 텍스트 띄우기 (적을 때렸으니 isEnemy는 true!)
                battleUI.SpawnDamageText(damage, true);

                // 5. 로그 출력
                battleUI.SetTurnText($"{_currentUnit.Name}의 공격! {damage}의 데미지!");
                Debug.Log($"{_currentUnit.Name} 이 적을 공격!");
                break;
            case "skill":
                Debug.Log("스킬!");
                break;
            case "item":
                Debug.Log("아이템!");
                break;
            case "flee":
                Debug.Log("도망!");
                break;
        }

        EndTurn();
    }

    private void EndTurn()
    {
        if (CheckBattleEnd()) return;
        _currentTurnIndex = (_currentTurnIndex + 1) % _turnOrder.Count;
        StartTurn();
    }

    private bool CheckBattleEnd()
    {
        bool allEnemiesDead = _enemies.TrueForAll(u => !u.IsAlive);
        bool allPlayersDead = _players.TrueForAll(u => !u.IsAlive);

        if (allEnemiesDead)
        {
            _turnBattleCanvas.SetActive(false);
            Debug.Log("승리!");
            return true;
        }
        if (allPlayersDead)
        {
            _turnBattleCanvas.SetActive(false);
            Debug.Log("패배!");
            return true;
        }
        return false;
    }
}