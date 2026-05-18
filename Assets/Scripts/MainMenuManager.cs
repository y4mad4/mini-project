using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 반드시 필요한 네임스페이스

public class MainMenuManager : MonoBehaviour
{
    // [게임 시작] 버튼과 연결할 함수
    public void StartGame()
    {
        // "GameScene" 자리에 강아지가 돌아다니는 실제 인게임 씬 이름을 적어주세요.
        SceneManager.LoadScene("Main");
    }

    // [게임 종료] 버튼과 연결할 함수
    public void ExitGame()
    {
        // 유니티 에디터에서는 실제로 꺼지지 않으므로 작동 확인용 로그를 찍습니다.
        Debug.Log("게임이 종료되었습니다.");

        // 실제 빌드된 PC 게임(기기)에서 게임을 끄는 명령어
        Application.Quit();
    }
}