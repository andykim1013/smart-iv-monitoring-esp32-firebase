# Reconstruction Status

컴포넌트별 복원 상태를 기록합니다. "복원"은 근거가 명확한 것만 포함하며, 근거 없는 추측은 포함하지 않습니다.

---

## ESP32 Firmware
**상태: 실제 원본 확보 (100%)**
- `esp32_iv_monitor.ino` — 원본 파일 SHA256 해시 일치, 완전 동일 사본
- 센서 핀 설정, HX711 무게 처리, TCRT5000 물방울 카운트, LCD 출력, 시간 계산/예상 종료시간 계산, Firebase 데이터 경로, NTP, loop 주기 — **모두 원본 그대로이며 어떤 것도 리팩터링하지 않았습니다.**
- WiFi SSID/비밀번호, Firebase API Key/URL/이메일/비밀번호는 원본부터 "OOO 작성"류 플레이스홀더였고, 이번 단계에서는 `secrets.h` 구조로도 바꾸지 않았습니다(다음 공개 정리 단계에서 별도 판단).

## Android (mobile-app)
**상태(3단계 갱신): Original source partially recovered (2 classes) + reconstructed build scaffolding added — BUILD SUCCESSFUL**

- `recovered-source/`: 아래 "부분 소스 확보" 내용 그대로, **3단계에서 한 글자도 수정하지 않음**
- `reconstructed-app/`: recovered-source를 복사해 최소 수정(컴파일 에러 1건)한 뒤, Gradle/Manifest/리소스를 신규 재구성해 실제 `assembleDebug` 빌드에 **성공**(APK 산출 확인, 이후 `gradle clean`으로 빌드 산출물은 정리함)
- 상세 검증 내용은 이 문서 하단 "3단계: 실제 빌드 검증 결과" 참조

**상태(2단계): 부분 소스 확보 — 완전한 클래스 2개만 복원**
- 문서(최종보고서 ver1.5.0 본문+부록, 사용자메뉴얼)에서 다음 2개 클래스가 **package~닫는 중괄호까지 완결된 형태**로 확인되어 `.java`로 복원했습니다.
  - `MainActivity.java` — package, imports, class 선언, `onCreate`, 버튼 클릭 리스너, `Intent` 이동, 닫는 중괄호까지 전부 존재
  - `MainActivity01.java` — package, imports, class 선언, 필드(`textView03`, `textViewend1`, `textViewstart1`, `Starttime`, `Endtime`), `onCreate`, Firebase `ValueEventListener` 2개(`onDataChange`/`onCancelled`), 버튼 클릭 리스너, 닫는 중괄호까지 전부 존재
- **원문 그대로 보존한 특이사항(임의 수정하지 않음)**:
  - `MainActivity.java`: `import android.os.Bundle;`, `import androidx.appcompat.app.AppCompatActivity;`가 각각 2회 중복 선언됨(원본 그대로)
  - `MainActivity01.java`: 필드 `textView03`이 선언만 되고 본문에서 사용되지 않음(원본 그대로)
  - `MainActivity01.java`: `Starttime.addValueEventListener(...)`의 `onCancelled` 콜백 내부에서 `textViewstart1`이 아니라 `textViewend1.setText(...)`을 호출함 — **원본 문서에 실제로 이렇게 적혀 있는 버그로 보이는 부분**이며, 임의로 고치지 않고 그대로 옮겼습니다.
  - 두 클래스 모두 최종보고서 텍스트에는 코드 라인에 들여쓰기가 없는 상태로 남아 있었습니다(HWP 원본의 문단 서식 때문인지, 학생이 원래 들여쓰기 없이 작성했는지는 확인 불가). 이번 복원에서는 **원문 그대로 들여쓰기를 추가하지 않고** 발췌된 형태를 유지했습니다.
- **복원하지 않은 것**: 그 외 언급되는 XML 리소스, Manifest, Gradle 설정 — 텍스트 원문이 없어 `docs/missing-source.md`에 별도 기록

## WPF (desktop-app)
**상태(3단계 갱신): Original source unavailable — portfolio reimplementation added (build UNVERIFIED, no .NET SDK)**

- `desktop-app/README.md`(기능 명세)는 2단계 그대로 유지
- `reimplemented-wpf/`: README의 명세(클래스명 10개 메서드, DataGrid 6컬럼, Firebase 경로, 5초 주기, FireSharp)를 근거로 **전체 신규 재구현**(MainWindow, Window2, PatientRow, FirebaseSettings.example)
- **build는 검증하지 못했습니다**: 이 PC에 .NET SDK가 설치되어 있지 않아(`dotnet --list-sdks` 결과 없음, 런타임만 존재) `dotnet restore`/`build` 자체를 실행할 수 없었습니다. XAML well-formed 여부와 C# 중괄호/괄호 균형 등 **정적검증만 완료**했습니다.
- FireSharp(2.0.4)은 .NET Framework 4.5 세대 라이브러리로 알려져 있어 net7.0-windows와의 실제 호환성이 미검증 상태입니다(다른 라이브러리로 임의 교체하지 않음). 상세는 `docs/reconstruction-changes.md` 참조.

## CAD
**상태(3단계 갱신): 실제 원본 보존 + 공개용 최종본 1쌍 복사 완료(해시 검증됨)**
- 최종본으로 선정: `케이스 외형 출력 디자인.3dm`(2024-11-18, STL과 페어링됨) + `케이스 외형 출력 디자인.stl`(2024-11-19, 폴더 내 유일한 STL)
- `hardware/case-design/iv_monitor_case.3dm`, `iv_monitor_case.stl`로 파일명만 일반화해 복사, SHA256 해시로 원본과 완전 동일함을 확인
- `.3dmbak`(3개)와 "케이스 외형 출력 디자인1.3dm"(날짜상 더 최신이나 대응 STL 없음)은 복사하지 않음 — 근거는 `docs/reconstruction-changes.md` "CAD 최종본 조사" 참조
- 3dm 내부 OpenNURBS "문서 속성"(작성자 등)은 전용 도구 없이 완전히 확인 불가 — [확인 불가]로 명시, 텍스트 패턴 스캔으로는 PII 미검출

---

## 3단계: 실제 빌드 검증 결과

### Android
```
환경: Gradle 8.9(로컬 캐시), AGP 8.6.0, JDK 21, Android SDK platform 34/35 (모두 이 PC에 기존 설치되어 있던 것 사용, 신규 설치 없음)
명령: gradle assembleDebug
결과: BUILD SUCCESSFUL — app-debug.apk 산출 확인 후 gradle clean으로 산출물 정리
```
과정에서 발견/조치한 문제 2건(모두 `docs/reconstruction-changes.md`에 상세 기록):
1. `firebase-database`가 끌어오는 구버전 `kotlin-stdlib-jdk7/jdk8`이 최신 `kotlin-stdlib`과 클래스 중복 → 버전을 올리지 않고 구버전 transitive 모듈만 `exclude`
2. `androidx.constraintlayout`/`androidx.cardview`의 커스텀 XML 속성이 이 환경의 AAPT2 리소스 링크 단계에서 인식되지 않음(원인 미특정, 한글 경로·캐시 오염은 배제됨) → 표준 프레임워크 위젯(`LinearLayout`+shape drawable)으로 대체

### WPF
```
환경: .NET 7 런타임만 설치(SDK 없음)
결과: 정적검증(XAML well-formed, C# 중괄호/괄호 균형)만 완료 / 실제 build 미검증
```

## App Inventor
**상태: 최종 구현에서 제외**
- 역설계 단계(2024.09)에서 사용된 사실은 회의록/PPT로 확인되나, `.aia` 원본 파일이 자료에 없고 최종 구현은 Android Studio로 전면 교체됨
- 새 App Inventor 프로젝트를 생성하지 않았으며, 전환 과정만 `development-history.md`에 서술

## 폐기 아키텍처 (Arduino UNO / ESP8266 / Bluetooth / MySQL 직접연동)
**상태: 최종 구현에서 완전히 제외**
- 회의록(2·3차)에 폐기 결정이 명시적으로 기록되어 있어, 최종 구현물에 포함하지 않고 `development-history.md`에 "폐기 설계"로만 기록
