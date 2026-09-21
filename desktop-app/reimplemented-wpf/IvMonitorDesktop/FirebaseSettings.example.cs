// [신규 재구현 - config example/template]
// 실제 Firebase AuthSecret/BasePath는 어떤 원본 자료에도 실값으로 남아있지 않습니다
// (ESP32 펌웨어와 최종보고서 모두 "OOO 작성" 플레이스홀더였음, docs/source-map.md 참조).
// 이 파일은 example 템플릿이며, 실제 값은 절대 커밋하지 말고 이 파일을 복사한 뒤
// (예: FirebaseSettings.cs) 값을 채워 사용하십시오. FirebaseSettings.cs는 .gitignore 대상입니다.
namespace IvMonitorDesktop
{
    public static class FirebaseSettings
    {
        // Firebase Realtime Database Auth Secret (레거시 DB Secret 또는 서비스 계정 토큰)
        public const string AuthSecret = "YOUR_FIREBASE_AUTH_SECRET";

        // 예: "https://your-project-id.firebaseio.com/"
        public const string BasePath = "https://YOUR_PROJECT_ID.firebaseio.com/";
    }
}
