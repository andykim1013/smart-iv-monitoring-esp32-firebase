# Source Map

이 문서는 `smart-iv-monitoring-esp32-firebase/` 안의 각 공개용 파일이 원본 자료의 어디에서, 어떤 근거 수준으로 왔는지 추적합니다.

## Source of Truth 우선순위 (고정)
```
1순위: 실제 원본 소스파일 (.ino 등)
2순위: 최종보고서 ver1.5.0
3순위: 해당 기능 담당자의 개인보고서
4순위: 최종발표 V1.0.0
5순위: 구버전 보고서/발표자료
```
※ 상위 우선순위 자료가 있으면 그것을 그대로 사용하고, 여러 자료의 내용이 서로 다를 경우 "최신이니까"라는 이유만으로 임의로 합치지 않고 차이를 그대로 기록합니다.

## 파일별 매핑

| 공개용 파일 | 원본 출처 | 근거 수준 | 상태 | 비고 |
|---|---|---|---|---|
| `firmware/esp32_iv_monitor/esp32_iv_monitor.ino` | 원본 폴더 `4조_디지털 수액 모니터링 시스템 개발/` 내 하드웨어 담당 팀원의 `.ino` 파일 (1순위, 원본 파일명에 학번+실명 포함되어 이 문서에서는 생략) | 원본 파일 직접 복사 | **[실제 원본]** | SHA256 해시 일치, 파일명만 변경, 내용 100% 동일(검증 완료, 아래 참조) |
| `mobile-app/recovered-source/com/example/a02/MainActivity.java` | 최종보고서 ver1.5.0 부록#5 "어플리케이션 코드"(2순위), 본문 3-1 App 섹션과 문자 단위 동일, 원본 폴더 루트의 사용자메뉴얼 hwp(팀장 작성, 파일명에 학번+실명 포함되어 생략)에도 동일 텍스트 존재 | 문서 원문 텍스트 전체 발췌(package~닫는 중괄호까지 완결) | **[문서 원문 복원]** | 원문 그대로, 변수명/스타일 미수정. import 중복(`android.os.Bundle`, `androidx.appcompat.app.AppCompatActivity` 각 2회) 등 원문의 오탈자성 특징도 그대로 보존 |
| `mobile-app/recovered-source/com/example/a02/MainActivity01.java` | 최종보고서 ver1.5.0 부록#5(2순위), 본문 3-1 App 섹션과 문자 단위 동일(줄바꿈 차이만) | 문서 원문 텍스트 전체 발췌(package~닫는 중괄호까지 완결) | **[문서 원문 복원]** | 미사용 필드 `textView03`, `onCancelled`에서 `Starttime` 콜백인데 `textViewend1`을 갱신하는 원본 버그 등 그대로 보존 (임의 수정 없음) |
| `desktop-app/README.md` | 최종보고서 ver1.5.0 "3-2 프로그램"(2순위), WPF 담당 팀원 개인보고서(3순위) | 클래스/메서드명·역할 설명 텍스트만(코드 본문은 스크린샷이라 텍스트 없음) | **[설명만 존재]** | 실제 `.xaml`/`.cs` 코드는 어디에도 텍스트로 존재하지 않음. FireSharp 패키지명은 3순위 자료에서 텍스트로 직접 확인됨 |
| App Inventor 관련 자산 | 역설계 단계 회의록·PPT | 사용 사실만 텍스트로 확인, `.aia` 파일 없음 | **[폐기 설계]** | 최종 구현은 Android Studio로 전환되어 App Inventor 산출물은 최종 구현에 포함하지 않음 |
| Arduino UNO / ESP8266 / Bluetooth / MySQL 직접연동 | 1~3차 회의록 | 텍스트로 명시적 확인 | **[폐기 설계]** | 최종 아키텍처(ESP32+WiFi+Firebase)로 대체되어 반영하지 않음 |
| 팔 높게 들기 방지 알람 / 혈액 역류 방지 시스템 | `수정목록.txt` | 아이디어 수준 텍스트만 | **[미구현]** | 코드/회로 근거 전혀 없음 |

### 3단계 신규 추가분

| 공개용 파일 | 원본 출처 | 근거 수준 | 상태 | 비고 |
|---|---|---|---|---|
| `mobile-app/reconstructed-app/app/src/main/java/com/example/a02/MainActivity.java`, `MainActivity01.java` | recovered-source 동일 파일 | recovered-source 복사 + 컴파일 에러 1건 최소 수정(EdgeToEdge import 제거) | **[문서 원문 복원 + 최소 빌드 수정]** | 수정 내역은 `docs/reconstruction-changes.md` "Android 원문 수정 내역" 참조. recovered-source 자체는 무수정 |
| `mobile-app/reconstructed-app/app/src/main/res/layout/activity_main.xml`, `activity_main01.xml` | 최종보고서 ver1.5.0 "코드 설명" 텍스트(색상 hex, 텍스트 문구) + recovered Java의 View ID | 텍스트 근거 기반 근사 재현 (ConstraintLayout/CardView → 표준 위젯으로 대체, 사유는 reconstruction-changes.md) | **[신규 재구성]** | 원본 XML 아님. `card_background_main`(#ECF1E6), `room_background`(#EAF1E1)만 문서 원문 hex, 나머지 색상은 근사치 |
| `mobile-app/reconstructed-app/app/src/main/AndroidManifest.xml`, `*.gradle`, `colors.xml`, `strings.xml`, `themes.xml`, `border_shape.xml`, `card_background.xml` | 근거 없음(Manifest/Gradle), 또는 텍스트 근거 기반 근사(리소스) | 신규 작성 | **[신규 재구성]** | `docs/reconstruction-changes.md` 참조 |
| `desktop-app/reimplemented-wpf/**` (전체: .sln, .csproj, App/MainWindow/Window2 .xaml/.cs, PatientRow.cs, FirebaseSettings.example.cs) | `desktop-app/README.md`의 확인된 클래스/메서드/Firebase 경로/DataGrid 컬럼 명세 | 명세 기반 신규 구현(코드 본문 원문 없음) | **[신규 재구현]** | 당시 원본 WPF 소스가 아님. FireSharp 호환성 미검증(SDK 없음), 자세한 내용은 `docs/reconstruction-changes.md` |
| `hardware/case-design/iv_monitor_case.3dm`, `iv_monitor_case.stl` | 원본 `케이스 외형 출력 디자인.3dm`(2024-11-18) / `.stl`(2024-11-19) | 원본 파일 직접 복사 | **[실제 원본]** | SHA256 해시 일치, 파일명만 일반화. 선정 근거는 `docs/reconstruction-changes.md` "CAD 최종본 조사" 참조 |
| `케이스 외형 출력 디자인1.3dm` 등 나머지 CAD 후보 | 원본 폴더 | STL과 미대응 | **[판단 불가]**(공개용 미포함) | 최신 수정일이지만 대응 STL 없음 — 다음 단계에서 사용자 확인 후 포함 여부 결정 |

## ESP32 펌웨어 원본 동일성 검증 상세
```
원본: 2학년 2학기 설계/4조_디지털 수액 모니터링 시스템 개발/ 폴더 내 하드웨어 담당 팀원의 .ino 파일 (파일명에 학번+실명 포함되어 이 문서에서는 생략)
사본: firmware/esp32_iv_monitor/esp32_iv_monitor.ino

SHA256(원본) = 61faf2f7c8b4062545676607af73021d955608b7ad703fb4b8de788f67593ac7
SHA256(사본) = 61faf2f7c8b4062545676607af73021d955608b7ad703fb4b8de788f67593ac7
diff 결과   = 차이 없음 (0 lines)
파일 크기    = 13,711 bytes (동일)

결론: 파일명만 공개용으로 일반화되었을 뿐, 내용은 100% 동일한 원본입니다.
```

## 참고: 최종보고서 부록4(펌웨어 코드)와 .ino 원본의 관계
최종보고서 ver1.5.0 부록#4 "소스 코드"에 실린 ESP32 코드는 `.ino` 원본과 실질적으로 동일한 내용(변수/로직/주석 일치, 코드 종료부 문자열까지 정확히 일치 확인)입니다. 다만 이번 2단계에서는 **1순위 자료인 `.ino` 파일 자체**를 그대로 사용했으므로, 부록4 텍스트는 교차검증 용도로만 참고했습니다.

## Android 코드 본문·부록 간 동일성
최종보고서 본문(3-1 App 섹션)과 부록#5(어플리케이션 코드)에 실린 `MainActivity`/`MainActivity01` 코드는 줄바꿈 유무를 제외하면 **완전히 동일**합니다(문자 단위 diff 결과 실질 차이 없음). 서로 다른 버전이 아니라 같은 코드가 본문과 부록에 중복 수록된 것으로 판단했습니다.
