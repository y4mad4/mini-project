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

    public bool IsInBattle { get; private set; } // 배틀시 이동방지



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

    private void Start()
    {
        // 플레이어/동료 한 번만 생성
        PlayerData playerData = DataManager.Instance.GetPlayer("player_01");
        if (playerData != null)
            _players.Add(new PlayerUnit(playerData));

        for (int i = 1; i <= 3; i++)
        {
            CompanionData companionData = DataManager.Instance.GetCompanion($"companion_0{i}");
            if (companionData == null) continue;
            _players.Add(new CompanionUnit(companionData));
        }
    }

    public void StartBattle(string enemyId)
    {
        _enemies.Clear();
        _turnOrder.Clear();
        IsInBattle = true;

        // 캔버스가 확실히 확인되었으니 켜기
        _turnBattleCanvas.SetActive(true);

        // 1. 플레이어 생성 (방어 코드 추가)
        //PlayerData playerData = DataManager.Instance.GetPlayer("player_01");
        //if (playerData != null)
        //{
        //    _players.Add(new PlayerUnit(playerData));
        //}
        //else
        //{
        //    Debug.LogError("플레이어 데이터를 찾을 수 없습니다!");
        //    return; // 플레이어가 없으면 전투 진행 불가
        //}

        //// 2. 동료 생성 (★ 빈 데이터 무시하기 적용)
        //for (int i = 1; i <= 3; i++)
        //{
        //    CompanionData companionData = DataManager.Instance.GetCompanion($"companion_0{i}");
        //    if (companionData == null) continue; // 데이터가 없으면 뻗지 말고 다음 번호로!

        //    _players.Add(new CompanionUnit(companionData));
        //}

        // 3. 적 생성
        EnemyData enemyData = DataManager.Instance.GetEnemy(enemyId);
        if (enemyData == null)
        {
            Debug.LogError($"{enemyId} 적 데이터를 찾을 수 없습니다!");
            return;
        }
        _currentEnemy = new EnemyUnit(enemyData);
        _enemies.Add(_currentEnemy);
      
        // 턴 순서 정렬
        _turnOrder.AddRange(_players);
        _turnOrder.AddRange(_enemies);
        _turnOrder.Sort((a, b) =>
        {
            if (a.Speed == b.Speed) return Random.Range(0, 2) == 0 ? 1 : -1;
            return b.Speed.CompareTo(a.Speed);
        });

     
        // UI 초기화
        battleUI.SetEnemyUI(_currentEnemy.Name, _currentEnemy.CurrentHp, _currentEnemy.MaxHp);
        battleUI.SetEnemyImage(_currentEnemy.Id);
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
            

            float final = target.TakeDamage(_currentEnemy.Attack);

            // ★ 추가: 적이 공격했을 때 아군 머리 위로 숫자 띄우기
            int targetIndex = _players.IndexOf(target); // 아군 리스트 중 몇 번째인지 찾기
            battleUI.SpawnDamageText(final, false, targetIndex); // false는 적이 아님(아군)을 의미

            battleUI.SetPartyUI(_players);
            battleUI.SetTurnText($"{target.Name}이(가) {final}의 피해를 입었다!");
        }

        // 2. 아군이 맞아서 체력이 깎인 모습을 확인할 수 있도록 1초 더 기다림
        yield return new WaitForSeconds(1.0f);

        // 대기 시간이 모두 끝나면 비로소 다음 턴으로 넘김
        EndTurn();
    }

    public void OnAction(string action)
    {
       
        switch (action)
        {
            case "attack":
                float finalDamage = _currentEnemy.TakeDamage(_currentUnit.Attack);

                battleUI.SetEnemyUI(_currentEnemy.Name, _currentEnemy.CurrentHp, _currentEnemy.MaxHp);

                battleUI.SpawnDamageText(finalDamage, true);

                battleUI.SetTurnText($"{_currentUnit.Name}의 공격! {finalDamage}의 데미지!");

                battleUI.SetActionPanel(false);

                break;
            case "skill":
                // 현재 유닛의 스킬 목록 가져오기
                List<SkillData> skills = new List<SkillData>();

                if (_currentUnit is PlayerUnit playerUnit)
                {
                    // 플레이어는 모든 스킬 사용 가능 (나중에 장착 시스템으로 변경)
                    skills.Add(DataManager.Instance.GetSkill("skill_01"));
                    skills.Add(DataManager.Instance.GetSkill("skill_02"));
                    skills.Add(DataManager.Instance.GetSkill("skill_03"));
                }
                else if (_currentUnit is CompanionUnit companionUnit)
                {
                    // 동료는 본인 스킬만 사용
                    skills.Add(DataManager.Instance.GetSkill(companionUnit.SkillId));
                }

                battleUI.ShowSkillActions(skills);
                return; // EndTurn 호출 안 함!

            case "item":
                Debug.Log("아이템!");
                battleUI.SetActionPanel(false);
                break;

            case "flee":
                // 도망 성공률 계산
                float playerSpeed = _currentUnit.Speed;
                float enemySpeed = _currentEnemy.Speed;

                // 기본 성공률 50%, 속도 차이에 따라 가중치 적용
                float fleeChance = 0.5f + (playerSpeed - enemySpeed) * 0.05f;
                fleeChance = Mathf.Clamp(fleeChance, 0.1f, 0.9f); // 최소 10%, 최대 90%

                if (Random.value < fleeChance)
                {
                    // 도망 성공
                    battleUI.SetTurnText("도망쳤다!");
                    _turnBattleCanvas.SetActive(false);
                    IsInBattle = false;
                    return; // EndTurn 호출 안 함
                }
                else
                {
                    // 도망 실패
                    battleUI.SetTurnText("도망에 실패했다!");
                    battleUI.SetActionPanel(false);
                }
                break;
        }

        EndTurn();
    }

    public void UseSkill(SkillData skill)
    {
        // MP 체크
        if (_currentUnit.CurrentMp < skill.MpCost)
        {
            Debug.Log("MP가 부족합니다!");
            battleUI.ShowDefaultActions();
            return;
        }

        // MP 소모
        _currentUnit.UseMp(skill.MpCost);

        switch (skill.Type)
        {
            case "Damage":

                float finalDamage = _currentEnemy.TakeDamage(skill.Damage);
                battleUI.SetEnemyUI(_currentEnemy.Name, _currentEnemy.CurrentHp, _currentEnemy.MaxHp);
                battleUI.SpawnDamageText(finalDamage, true);
                battleUI.SetTurnText($"{_currentUnit.Name}의 공격! {finalDamage}의 데미지!");
                battleUI.SetActionPanel(false);
                break;

            case "Heal":
                _currentUnit.Heal(skill.Damage);
                battleUI.SetPartyUI(_players);
                battleUI.SetTurnText($"{_currentUnit.Name}의 {skill.Name}! {skill.Damage} 회복!");
                break;
        }

        battleUI.SetPartyUI(_players);
        battleUI.ShowDefaultActions();
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
            IsInBattle = false;
            return true;
        }
        if (allPlayersDead)
        {
            _turnBattleCanvas.SetActive(false);
            Debug.Log("패배!");
            IsInBattle = false;
            return true;
        }
        return false;
    }

}