# Firebase Setup

이 문서는 이 저장소의 각 컴포넌트가 Firebase Realtime Database(RTDB)를 실제로 어떻게 사용하는지, 그리고 이 저장소를 직접 실행해보려는 사람이 무엇을 준비해야 하는지를 설명합니다. 아래 내용은 실제 코드(`firmware/`, `mobile-app/`, `desktop-app/`)를 다시 읽고 확인한 결과이며, 추측은 [확인 불가]로 표시했습니다.

## 1. Firebase Realtime Database의 역할

이 프로젝트에서 Firebase RTDB는 ESP32와 두 모니터링 클라이언트(Android, WPF) 사이의 유일한 데이터 통로입니다. 아두이노-앱-데이터베이스가 직접 연결되던 초기 구조(블루투스/MySQL, [docs/development-history.md](development-history.md) 참조)가 재설계 과정에서 이 구조로 바뀌었습니다.

```
ESP32 (HX711 + TCRT5000 + LCD)
   │  Firebase.setFloat / Firebase.setString  (쓰기)
   ▼
Firebase Realtime Database  ──────────────┐
   │  FirebaseDatabase.getInstance()...    │  IFirebaseClient.GetAsync(...)
   │  (읽기)                                │  (읽기)
   ▼                                        ▼
Android App (MainActivity01)         WPF Desktop (MainWindow)
```

ESP32만 쓰기(write)를 수행하고, Android와 WPF는 모두 읽기(read) 전용으로 동작합니다. 이는 recovered-source(Java)와 reimplemented-wpf(C#) 코드 어디에도 Firebase에 값을 **쓰는(set) 호출이 없다는 사실**로 확인했습니다.

## 2. 확인된 RTDB path / schema

세 컴포넌트(펌웨어, Android, WPF)의 실제 코드를 대조한 결과, 아래 4개 경로만 확인되었습니다. `802`는 최종보고서·코드 양쪽에서 예시로 등장하는 병실/침대 번호이며, 실제로는 임의의 호실 번호가 들어갈 수 있는 자리입니다.

| Path | 타입 | 쓰는 주체 | 읽는 주체 |
|---|---|---|---|
| `/802/StartWeight` | float | ESP32 | (읽는 코드 없음, 확인 안 됨) |
| `/802/StartTime` | string | ESP32 | Android |
| `/802/EndTime` | string | ESP32 | Android, WPF |
| `/802/speed` | float | ESP32 | WPF |

이 4개 외에 코드/문서로 확인되는 필드는 없습니다. 문서 본문에는 "환자 이름, 투여 용량, 수액 종류"도 언급되지만, 이 값들이 실제로 Firebase에 쓰였다는 코드 근거는 없어(WPF Window2 입력값은 화면에만 반영되는 것으로 확인) **schema에 포함하지 않았습니다.**

## 3. ESP32가 사용하는 설정

`firmware/esp32_iv_monitor/esp32_iv_monitor.ino`(2024년 실제 원본, 이번 4단계에서도 **수정하지 않음**)는 다음 상수를 코드에 직접 선언합니다.

```cpp
const char* ssid = "와이파이 이름 작성";
const char* password = "와이파이 비밀번호 작성";
#define API_KEY "파이어베이스 API 키 작성"
#define DATABASE_URL "파이어 베이스 URL 주소 작성"
#define USER_EMAIL "사용자 이메일"
#define USER_PASSWORD "비밀번호 작성"
```

원본부터 실제 값이 아니라 사람이 채워 넣어야 할 자리표시자였습니다. 이 저장소에서는 이 구조를 `secrets.h` 같은 별도 헤더 파일로 리팩터링하지 않았습니다 — **원본 소스를 그대로 보존하는 것이 우선**이기 때문입니다. 이 파일을 직접 사용하려면 위 6개 값을 본인의 WiFi/Firebase 정보로 직접 채워 넣으십시오(파일을 열어 편집).

## 4. Android가 사용하는 설정

`mobile-app/reconstructed-app/app/build.gradle`을 확인한 결과:
- `com.google.gms:google-services` 플러그인이 적용되어 있지 **않습니다.**
- `google-services.json`을 참조하는 코드/설정이 어디에도 없습니다.
- `recovered-source`/`reconstructed-app`의 `MainActivity.java`, `MainActivity01.java`에도 `FirebaseApp.initializeApp(...)`이나 `FirebaseOptions`를 이용한 수동 초기화 코드가 없습니다. `FirebaseDatabase.getInstance()`를 곧바로 호출할 뿐입니다.

**결론**: 현재 Gradle 구조는 `google-services.json`을 필요로 하지 않지만, 그 이유는 "설정이 필요 없어서"가 아니라 **Firebase 초기화 코드 자체가 원본 문서에 없어서**입니다. 즉 이 상태로는 앱을 실제 기기에서 실행해도 `FirebaseDatabase.getInstance()` 호출 시점에 "Default FirebaseApp이 초기화되지 않았다"는 런타임 오류가 발생할 가능성이 높습니다. 이는 **알려진 한계**이며, 이번 4단계(문서화 단계)에서는 코드를 추가로 개발하지 않았으므로 `google-services.example.json` 같은 템플릿도 만들지 않았습니다. 실제로 앱을 동작시키려면 다음 단계 중 하나가 **추가로 필요**합니다(둘 다 이번 저장소 범위 밖의 작업):
1. `com.google.gms:google-services` 플러그인 적용 + 본인의 `google-services.json` 추가, 또는
2. `Application` 클래스 등에서 `FirebaseOptions.Builder()`로 API Key/DB URL을 직접 넣어 수동 초기화하는 코드 추가

## 5. WPF가 사용하는 설정

`desktop-app/reimplemented-wpf/IvMonitorDesktop/FirebaseSettings.example.cs`에 두 필드만 존재합니다.

```csharp
public const string AuthSecret = "YOUR_FIREBASE_AUTH_SECRET";
public const string BasePath = "https://YOUR_PROJECT_ID.firebaseio.com/";
```

`MainWindow.xaml.cs`는 이 두 필드(`FirebaseSettings.AuthSecret`, `FirebaseSettings.BasePath`)만 참조하며 다른 필드는 없습니다. 실제로 사용하려면 `FirebaseSettings.example.cs`를 복사해 `FirebaseSettings.cs`(같은 폴더, 같은 네임스페이스)로 만들고 두 값을 채우십시오. `FirebaseSettings.cs`는 `.gitignore`에 등록되어 있어 실수로 커밋되지 않습니다.

## 6. Credential을 Repository에 포함하지 않는 정책

이 저장소에는 다음이 **전혀 포함되어 있지 않습니다.**
- 2024년 당시 실제 Firebase 프로젝트 ID
- 실제 Firebase API Key / DB URL
- 실제 Firebase 로그인 이메일/비밀번호 또는 Auth Secret
- 실제 WiFi SSID/비밀번호

이 저장소를 재현하려면 **사용자가 자신의 Firebase 프로젝트(Realtime Database 활성화)를 별도로 준비**해야 하며, 위 3~5절의 설정 자리에 자신의 값을 채워야 합니다.

## 7. Android build 성공 ≠ Firebase runtime 연결 검증

`mobile-app/reconstructed-app`는 `gradle assembleDebug`로 **컴파일/빌드 성공**을 확인했습니다(`docs/reconstruction-status.md` 참조). 이것은 **소스 코드 구조와 의존성 해석이 정상**임을 의미할 뿐이며, 실제 기기/에뮬레이터에서 앱을 실행해 Firebase에 실시간으로 연결되는지는 **검증하지 않았습니다**(4절에서 설명한 초기화 누락 문제도 있어 현재 상태로는 런타임 연결이 되지 않을 가능성이 높습니다).

## 8. WPF Firebase 연동은 build/runtime 모두 미검증

`desktop-app/reimplemented-wpf`는 이 PC에 .NET SDK가 설치되어 있지 않아 `dotnet build` 자체를 실행하지 못했습니다(XAML/구문 정적검증만 완료). 따라서 컴파일 성공 여부, FireSharp 패키지의 실제 호환성, Firebase 런타임 연결 모두 **미검증** 상태입니다.

## 9. Sample data

`firebase/sample-data.json`에 위 2절의 schema(경로 4개)만 사용한 **신규 공개용 예시 데이터**를 두었습니다. 실제 2024년 DB 덤프가 아니며, 실제 환자정보·의료정보를 포함하지 않습니다.
