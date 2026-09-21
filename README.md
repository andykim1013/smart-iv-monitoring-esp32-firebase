# Smart IV Monitoring System

**ESP32 · Firebase · Android · WPF 기반 디지털 수액 모니터링 시스템**

---

## 이 프로젝트는 무엇인가

- 2024년 대학교 학기설계(설계 및 프로젝트 기본II) 과목의 **팀 프로젝트**를 바탕으로 정리한 포트폴리오 저장소입니다.
- **교육 및 연구 목적의 프로토타입**이며, 실제 임상 현장에서 검증된 의료기기가 **아닙니다.**
- **실제 환자 모니터링이나 의료적 판단 목적으로 사용해서는 안 됩니다.**
- 이 저장소의 코드 중 일부는 2024년 당시 실제 소스이고, 일부는 2026년에 원본이 유실된 부분을 문서 근거로 재구성/재구현한 것입니다. 아래 "구현 상태" 표에서 정확히 구분하고 있으니 반드시 확인하십시오.

## 프로젝트 목적

당시 최종보고서를 근거로, 이 프로젝트는 다음을 목표로 했습니다.
- 로드셀(무게 센서)로 수액 팩의 무게 변화를 측정
- 적외선 센서로 수액 챔버 내 물방울 낙하를 감지
- 위 두 값으로 투여 속도와 잔여 시간을 추정
- Firebase Realtime Database로 측정값을 실시간 전송
- Android 앱과 WPF 데스크톱 프로그램으로 간호사가 원격에서 여러 환자의 수액 상태를 한눈에 모니터링

실제 구현을 넘어서는 효과(예: 임상적 안전성 향상, 의료사고 예방 등)를 이 저장소는 주장하지 않습니다. 그런 표현은 당시 보고서의 "기대효과" 서술일 뿐, 실측/임상시험으로 검증된 사실이 아닙니다.

## 시스템 구조

```
HX711 Load Cell ─┐
TCRT5000 (물방울 감지) ─┼→ ESP32 ──(WiFi)──→ Firebase Realtime Database
LCD (16x2 I2C) ───┘                                   │
                                                       ├──(읽기)──→ Android App
                                                       └──(읽기)──→ WPF Desktop App
```

- ESP32만 Firebase에 **값을 씁니다**(무게/시간/속도 측정 결과).
- Android, WPF는 모두 Firebase 값을 **읽기만** 합니다.
- 자세한 경로(path)와 근거는 [docs/firebase-setup.md](docs/firebase-setup.md) 참조.

## 구현 상태 (반드시 확인)

| 구성요소 | 현재 Repository 상태 | 출처 | 검증 |
|---|---|---|---|
| **ESP32 Firmware** | [실제 2024 원본] 그대로 보존 | `4조_디지털 수액 모니터링 시스템 개발/` 폴더의 `.ino` 원본 | 하드웨어 compile/run은 별도 미검증(소스 보존만 확인) |
| **Android — recovered-source** | [문서 원문 복원] 2024년 문서(최종보고서)에 실제 텍스트로 남아있던 코드를 그대로 옮김 | 최종보고서 ver1.5.0 부록 | 원문과 문자 단위 대조 완료 |
| **Android — reconstructed-app** | [신규 재구성] 2026년 공개 아카이브 정리 과정에서 Gradle/Manifest/리소스를 새로 작성 | recovered-source + 문서의 화면 설명 | `assembleDebug` **BUILD SUCCESSFUL**(컴파일/빌드만 검증, Firebase 런타임 연결은 별도 미검증) |
| **WPF** | [신규 재구현] 당시 전체 소스는 유실, 확인된 클래스/메서드/화면 명세만 근거로 새로 작성 | `desktop-app/README.md`의 기능 명세 | 정적 구조 검증만 완료, **실제 build 미검증**(.NET SDK 없음) |
| **CAD (.3dm/.stl)** | [실제 2024 원본] 파일명만 일반화, 내용은 SHA256 해시로 원본과 동일 확인 | 원본 폴더의 3D 케이스 설계 파일 | 해시 동일성 확인, `.3dm` 내부 문서 속성은 수동 확인 권장(아래 "알려진 한계" 참조) |

포트폴리오를 보시는 분이 **신규 재구현 코드를 2024년 원본으로 오해하지 않도록**, 이 표의 분류를 항상 기준으로 삼아주십시오.

## 실제 구현된 기능

- HX711 로드셀 기반 무게 측정
- TCRT5000 적외선 센서 기반 물방울 낙하 감지
- LCD(16x2, I2C) 출력
- NTP 서버를 이용한 시간 동기화
- 물방울 수/무게 변화 기반 투여 속도·잔여 시간·예상 종료시간 계산
- Firebase Realtime Database로 측정값 전송
- Android 앱의 층별 화면 이동 및 실시간 시작/종료 시간 표시(Firebase 리스너)
- WPF DataGrid 기반 환자 목록 모니터링(호실/이름/시작시간/종료시간/예상종료시간/진행도), 5초 주기 갱신, 환자정보 입력창

## 미구현 / 계획 (문서에만 존재)

다음은 팀 내부 메모(수정 계획)에 아이디어로만 있었고, 코드/회로 근거가 없어 이 저장소에 구현되어 있지 않습니다.
- 팔을 높게 들었을 때의 방지 알람 시스템
- 혈액 역류 방지 시스템

## 개발 과정 (초기 설계 → 최종 구현)

```
초기: Arduino UNO + Bluetooth + App Inventor(블록코딩)
   ↓
중간: ESP8266 + MySQL(구상)
   ↓
중간: ESP8266 + Firebase  ("아두이노에서 MySQL 직접 진입 불가" 판단에 따른 교체)
   ↓
최종: ESP32 + WiFi + Firebase Realtime Database + Android Studio + WPF
```

초기/중간 단계의 구성 요소(Arduino UNO, ESP8266, Bluetooth, MySQL, App Inventor)는 **모두 폐기된 설계이며 이 저장소의 최종 기술 스택에 포함되어 있지 않습니다.** 자세한 내용은 [docs/development-history.md](docs/development-history.md) 참조.

## Repository 구조

```
smart-iv-monitoring-esp32-firebase/
├── firmware/esp32_iv_monitor/          # [실제 원본] ESP32 펌웨어
├── mobile-app/
│   ├── recovered-source/               # [문서 원문 복원] Java 2개 클래스
│   └── reconstructed-app/              # [신규 재구성] 전체 Gradle 프로젝트 (빌드 성공)
├── desktop-app/
│   ├── README.md                       # WPF 기능 명세(2024년 문서 근거)
│   └── reimplemented-wpf/              # [신규 재구현] WPF 프로젝트 (build 미검증)
├── hardware/case-design/               # [실제 원본] 3D 케이스 설계(.3dm/.stl)
├── firebase/sample-data.json           # [신규 공개용 예시 데이터] synthetic
└── docs/                                # 복원/재구성 근거 문서 일체
```

## Android

- `mobile-app/recovered-source/`: **2024년 문서에서 복원**한 `MainActivity.java`, `MainActivity01.java`. 최종보고서에 package~닫는 중괄호까지 완결된 형태로 실려 있던 코드를 그대로 옮겼습니다(변수명/스타일 미수정, 원문의 버그·중복 import도 그대로 보존).
- `mobile-app/reconstructed-app/`: **2026년 공개 아카이브 정리 과정에서 재구성**한 Gradle 프로젝트 전체(Manifest, 리소스 XML, 빌드 설정)입니다. 당시 원본이 아닙니다.
- `gradle assembleDebug`가 **BUILD SUCCESSFUL**로 완료되어 `app-debug.apk`가 정상 산출되는 것을 확인했습니다. 이는 **소스/빌드 설정이 구조적으로 유효하다는 것**을 의미할 뿐이며, **Firebase 실시간 연결이 성공했다는 뜻이 아닙니다**(초기화 코드 관련 한계는 [docs/firebase-setup.md](docs/firebase-setup.md) 참조).

## WPF

- **2024년 당시 실제 WPF 전체 소스는 현재 자료에 남아있지 않습니다**(코드 본문은 스크린샷으로만 존재).
- `desktop-app/reimplemented-wpf/`는 최종보고서에서 확인된 클래스명(`MainWindow`, `Window2`), 메서드명, DataGrid 컬럼, Firebase 경로, 화면 흐름만 근거로 **공개용 포트폴리오 목적의 신규 재구현**입니다.
- 이 PC에 .NET SDK가 없어 **실제 build는 미검증**입니다(XAML/구문 정적검증만 완료).
- Firebase 연동에 사용한 **FireSharp 패키지의 최신 .NET(net7.0-windows)과의 호환성도 미검증** 상태입니다.

## Firebase

- 이 저장소에는 **2024년 당시 실제 Firebase 자격증명(프로젝트 ID, API Key, 이메일/비밀번호, DB Secret)이 전혀 포함되어 있지 않습니다.**
- 이 저장소를 재현하려면 **본인의 Firebase 프로젝트(Realtime Database)를 별도로 준비**해야 합니다.
- 확인된 RTDB 경로는 `/802/StartWeight`, `/802/StartTime`, `/802/EndTime`, `/802/speed` 4개뿐입니다.
- ESP32/Android/WPF 각각의 설정 방법, 초기화 관련 한계, sample data는 [docs/firebase-setup.md](docs/firebase-setup.md)에 상세히 정리했습니다.

## Hardware / CAD

- 보드: ESP32 (초기에는 Arduino UNO → ESP8266을 거쳐 최종 교체)
- 센서: HX711 + 5kg 로드셀, TCRT5000 적외선 반사 센서(2개 구성)
- 출력: 16x2 I2C LCD(PCF8574)
- `hardware/case-design/iv_monitor_case.3dm`, `iv_monitor_case.stl`: 실제 3D 프린팅 출력에 사용된 설계 원본이며, 파일명만 공개용으로 일반화했습니다(내용은 SHA256 해시로 원본과 동일함을 확인).
- `.3dm` 파일 내부의 Rhino "문서 속성"(작성자 등 메타데이터)은 전용 도구 없이 완전히 검증하지 못했습니다 — **공개 전 Rhino에서 직접 열어 수동으로 확인하는 것을 권장합니다(manual review recommended).**

## Build / 검증 상태

| 컴포넌트 | 상태 |
|---|---|
| ESP32 Firmware | 원본 소스 보존. 실제 하드웨어 컴파일/실행은 이번 문서화 단계에서 별도로 검증하지 않았습니다. |
| Android reconstructed-app | `assembleDebug` 성공. Firebase 런타임 연결은 별도 미검증. |
| WPF reimplementation | 정적 구조(XAML well-formed, C# 구문) 검증 완료. .NET SDK 부재로 실제 build 미검증. |

## 보안 / 개인정보

- 실제 WiFi 자격증명 없음 (원본부터 자리표시자만 존재)
- 실제 Firebase 자격증명 없음
- 실제 환자 데이터 없음, 실제 의료정보 없음
- 팀원 개인정보(실명, 학번, 전화번호 등) 제거됨
- `firebase/sample-data.json`은 **신규 공개용 합성 예시 데이터**이며 실제 데이터가 아닙니다.

## 알려진 한계

- Android 코드 중 리소스(XML)·Manifest·Gradle 설정은 문서 기반으로 재구성한 것이며 당시 원본이 아닙니다.
- WPF는 전체가 기능 명세 기반의 신규 재구현입니다.
- WPF의 실제 build는 미검증입니다.
- FireSharp 패키지의 최신 .NET과의 호환성이 미검증입니다.
- 실제 Firebase 런타임 연결은 Android/WPF 모두 검증되지 않았습니다(Android는 초기화 코드 자체가 원본에 없었음).
- 실제 임상 검증이나 의료기기 인증을 받은 적이 없습니다.
- `.3dm` 파일 내부 메타데이터(작성자 등)는 일부 수동 확인이 필요할 수 있습니다.

## 원본 자료를 공개하지 않은 이유

원본 폴더에는 HWP/HWPX 최종보고서, PPTX 발표자료, 수업 평가표, 회의록 등이 있었지만 이 저장소에는 포함하지 않았습니다. 이유는 다음과 같습니다.
- 팀원 개인정보(실명, 학번, 연락처)가 포함되어 있음
- 일부 자료(평가표)에는 우리 팀 외 다른 학생들의 개인정보도 포함되어 있음
- 여러 버전이 중복되어 있어 공개 저장소에 그대로 둘 필요가 없음
- 수업 제출용 문서로, 공개 배포를 전제로 작성되지 않음

또한 원본 문서 중 일부에는 3D 모델링 소프트웨어의 비정상적인 설치 방법에 관한 설명이 포함되어 있었는데, 이 내용은 공개본에서 완전히 제외했습니다(구체적인 방법은 이 저장소 어디에도 기재하지 않았습니다).

## 관련 문서

- [docs/source-map.md](docs/source-map.md) — 모든 공개 파일의 출처와 근거 수준
- [docs/reconstruction-status.md](docs/reconstruction-status.md) — 컴포넌트별 복원/재구성 상태
- [docs/reconstruction-changes.md](docs/reconstruction-changes.md) — 3단계에서 이루어진 모든 변경의 상세 기록
- [docs/missing-source.md](docs/missing-source.md) — 여전히 원본이 없는 파일 목록과 근거
- [docs/development-history.md](docs/development-history.md) — 아키텍처 변천사(폐기 설계 포함)
- [docs/firebase-setup.md](docs/firebase-setup.md) — Firebase 연동 구조와 설정 방법

## 라이선스

이 저장소는 팀 프로젝트 결과물과 신규 재구성/재구현 코드가 혼재되어 있어, 라이선스는 아직 지정되지 않았습니다.
