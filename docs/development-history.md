# Development History

회의록·발표자료에서 확인된 기술적 의사결정 변화만 정리했습니다(개인정보 제외). 최종 구현과 폐기안을 명확히 구분합니다.

## 아키텍처 변천사

```
[역설계 단계 (2024.09)]
Arduino UNO + Bluetooth 통신 + App Inventor(블록코딩) + MySQL(구상)
데이터 흐름: 아두이노 → 블루투스 통신 → 앱인벤터 → 데이터베이스
        │
        │  1차 회의(재설계 착수): ESP8266 채택, MySQL 유지
        ▼
[재설계 1차 (2024.10 초)]
ESP8266 + MySQL(구상) + 적외선 트래킹 센서
        │
        │  2차 회의: "아두이노에서 MySQL로 직접 진입 불가" 판단
        │  → 데이터베이스를 MySQL에서 Firebase로 교체
        ▼
[재설계 2차]
ESP8266 + Firebase
데이터 흐름: 아두이노 → 데이터베이스(Firebase) → 모니터링 프로그램
        │
        │  3차 회의: 광센서 각도 조정, 센서 2개 구성으로 회로 수정
        │  → 보드를 ESP8266에서 ESP32로 교체
        ▼
[최종 구현 (2024.12 완료)]
ESP32 + WiFi(내장) + Firebase Realtime Database
+ Android Studio 앱 (App Inventor 대체)
+ WPF(C#) 데스크톱 프로그램 (FireSharp로 Firebase 연동)
```

## 최종 구현에 포함된 것
- ESP32 보드, WiFi(내장 모듈), HX711 로드셀, TCRT5000 광센서(2개 구성), LCD(I2C)
- Firebase Realtime Database
- Android Studio 기반 모바일 앱 (Java)
- WPF(C#) 기반 데스크톱 모니터링 프로그램 (FireSharp)
- Rhino 8 기반 3D 프린팅 케이스

## 폐기된 설계 (최종 구현에 포함되지 않음)
| 폐기 항목 | 대체된 것 | 폐기 시점(근거) |
|---|---|---|
| Arduino UNO | ESP8266 → ESP32 | 재설계 착수 시점 |
| Bluetooth 통신 | WiFi(ESP32 내장) | 재설계 초기 |
| App Inventor(블록코딩) | Android Studio(Java) | 재설계 진행 중 |
| MySQL 직접연동 | Firebase Realtime Database | 2차 회의("아두이노에서 MySQL 직접 진입 불가") |
| ESP8266 | ESP32 | 3차 회의(회로 수정과 함께 교체) |

## 미구현/계획 수준 아이디어 (코드화하지 않음)
다음 항목은 `수정목록.txt`(팀 내부 메모)에 아이디어로만 존재하며, 회로도·코드·시연 근거가 전혀 없어 **[미구현/계획]**으로만 표시합니다.
- 팔 높게 들기 방지 알람 시스템
- 혈액 역류 방지 시스템

## App Inventor 처리 방침
초기 역설계 단계에서 실제로 사용된 사실은 확인되지만, `.aia` 프로젝트 파일 원본이 자료에 존재하지 않고 최종 시스템은 Android Studio로 완전히 대체되었습니다. 따라서 이번 복원 작업에서는:
- App Inventor용 파일을 새로 만들지 않았습니다.
- 최종 모바일 앱 코드(`mobile-app/`)에는 App Inventor 관련 산출물을 포함하지 않았습니다.
- 이 문서에만 전환 과정으로 기록합니다.

## 제외된 내용에 대한 안내
원본 최종보고서에는 3D 모델링 소프트웨어(Rhino 8)의 **비정상적인 설치 방법에 관한 설명**이 일부 포함되어 있었습니다. 해당 내용은 공개용 저장소 정책상 부적절하다고 판단하여 이 작업폴더의 어떤 문서에도 옮기지 않았습니다(구체적 방법·파일명·절차는 기록하지 않음).
