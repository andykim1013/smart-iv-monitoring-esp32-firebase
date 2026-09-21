# Reconstruction Changes (3단계)

이 문서는 신규 코드와 과거 원본 코드를 절대 혼동하지 않도록, 3단계에서 이루어진 모든 변경/생성 작업을 6가지로 분리해 기록합니다.

---

## 1. 원본 그대로 복사
- `firmware/esp32_iv_monitor/esp32_iv_monitor.ino` — 2단계에서 이미 완료(변경 없음, SHA256 동일 유지)

## 2. 문서 원문 그대로 복원
- `mobile-app/recovered-source/com/example/a02/MainActivity.java`, `MainActivity01.java` — 2단계에서 이미 완료, **이번 3단계에서 한 글자도 수정하지 않았습니다.**

## 3. 신규 Android 재구성 (`mobile-app/reconstructed-app/`)
모두 **[신규 재구성]**이며 당시 원본이 아닙니다.

| 파일 | 근거 |
|---|---|
| `settings.gradle`, `build.gradle`, `gradle.properties`, `app/build.gradle` | 원본 Gradle 파일 없음. 이 PC의 기존 Android SDK(platform 34/35, build-tools 34.0.0/35.0.0)와 로컬 Gradle 캐시(AGP 8.6.0/8.7.2, Gradle 8.9)에 맞춰 새로 작성 |
| `app/src/main/AndroidManifest.xml` | 원본 없음. recovered-source의 실제 Activity 2개(MainActivity, MainActivity01)만 선언, INTERNET permission만 추가(Firebase 통신 근거) |
| `app/src/main/java/com/example/a02/MainActivity.java`, `MainActivity01.java` | recovered-source를 그대로 복사한 뒤, **컴파일 오류 1건만** 최소 수정(아래 "Android 원문 수정 내역" 참조) |
| `app/src/main/res/layout/activity_main.xml`, `activity_main01.xml` | 원본 XML 없음. 최종보고서 ver1.5.0 "코드 설명" 원문 텍스트(ConstraintLayout/CardView 속성값, 텍스트 문구, 색상 hex)를 근거로 재구성. recovered Java가 참조하는 View ID(`button2`, `textViewend1`, `textViewstart1`, `button05`)만 정확히 반영 |
| `app/src/main/res/values/colors.xml` | `#ECF1E6`, `#EAF1E1`은 문서 원문 hex 값 그대로. `green`/`green2`는 이름만 언급되고 실제 hex는 없어 **reconstructed approximation**으로 표시 |
| `app/src/main/res/values/strings.xml`, `themes.xml` | 신규 작성(Material3 미도입, AppCompat 최소 테마만 사용) |
| `app/src/main/res/drawable/border_shape.xml` | 원문 텍스트("네모난 빈 틀") 근거로 재구성, 색상/두께는 근사치 |
| `app/src/main/res/drawable/card_background.xml` | 아래 "빌드 호환 대체" 참조 |

### Android 원문 수정 내역 (recovered-source → reconstructed-app 사본, 이 사본에서만 적용)
| 파일 | 수정 내용 | 사유 |
|---|---|---|
| `MainActivity.java` | `import androidx.activity.EdgeToEdge;` 삭제 | 원문에서도 실제로 사용되지 않는 import였고, 빌드에 사용한 `androidx.activity:activity:1.7.0`에는 이 클래스가 없어 컴파일 에러(`cannot find symbol`) 발생. **명백한 compile error에 대한 최소 수정** |
| `MainActivity01.java` | 동일하게 `import androidx.activity.EdgeToEdge;` 삭제 | 위와 동일 사유 |

그 외 원문의 특이사항(중복 import, 미사용 필드 `textView03`, `onCancelled`에서 `textViewend1`을 잘못 갱신하는 부분)은 **빌드를 막지 않으므로 수정하지 않고 그대로 두었습니다.**

### 빌드 호환 대체 (라이브러리 → 표준 위젯)
최종보고서 원문은 메인화면에 `ConstraintLayout`+`CardView` 사용을 명시하고 있어 처음에는 `androidx.constraintlayout:constraintlayout:2.1.4`, `androidx.cardview:cardview:1.0.0`를 그대로 포함했습니다. 그러나 이 빌드 환경(Gradle 8.9 + AGP 8.6.0 + JDK 21)에서 두 라이브러리의 커스텀 XML 속성(`app:layout_constraintTop_toTopOf`, `app:cardCornerRadius` 등)이 `processDebugResources`(AAPT2 리소스 링크) 단계에서 **"attribute res-auto:... not found"** 오류로 전부 실패하는 것을 확인했습니다.

진단 과정(모두 재현됨):
1. 프로젝트 경로에 한글이 포함되어 발생하는 문제인지 의심 → ASCII 전용 임시 경로로 복사해 재현 → **동일하게 실패**(한글 경로 문제 아님)
2. Gradle 캐시가 오염된 것인지 의심 → `gradle clean --rerun-tasks`로 캐시 무효화 후 재시도 → **동일하게 실패**
3. ConstraintLayout/CardView가 실제로 `debugCompileClasspath`에 해석되는지 확인 → 정상 해석됨, `ExtractAarTransform`도 정상 수행됨(`--info` 로그로 확인)
4. CardView 없이 ConstraintLayout 속성만 단독으로 테스트 → **동일하게 실패**(두 라이브러리 각각의 문제가 아니라 공통적인 환경 이슈로 판단)
5. 표준 프레임워크 위젯(`LinearLayout`, `Button`, `TextView`)만으로 별도 레이아웃을 만들어 테스트 → **정상 링크 성공**

원인을 완전히 특정하지는 못했으나(이 환경의 AAPT2/AGP 조합에 한정된 문제로 추정), "단순하게 재현"이라는 3단계 원칙에 따라 **ConstraintLayout/CardView를 걷어내고 표준 LinearLayout + shape drawable(`card_background.xml`, 문서 원문의 `cardCornerRadius=20dp`, `cardBackgroundColor=#ECF1E6` 값을 그대로 반영)로 동일한 배치/색상을 재현**했습니다. `app/build.gradle`에서 두 의존성도 제거했습니다. 라이브러리를 임의로 "다른 라이브러리"로 바꾼 것이 아니라, 문서에 없는 라이브러리 자체를 걷어내고 프레임워크 기본 기능으로 대체한 것입니다.

## 4. 신규 WPF 재구현 (`desktop-app/reimplemented-wpf/`)
모두 **[신규 재구현]**이며 당시 원본이 아닙니다. `desktop-app/README.md`에 정리된 확인된 클래스명/메서드명/DataGrid 컬럼/Firebase 경로/5초 주기 갱신/환자정보 입력 흐름만 근거로 새로 작성했습니다.

| 파일 | 내용 |
|---|---|
| `IvMonitorDesktop.sln`, `IvMonitorDesktop.csproj` | 신규 작성. Target Framework는 "16. WPF Target Framework" 참조 |
| `App.xaml`, `App.xaml.cs` | 표준 WPF 진입점(최소) |
| `MainWindow.xaml`, `MainWindow.xaml.cs` | DataGrid 6컬럼(호실/이름/시작시간/종료시간/예상종료시간/진행도) + 확인된 메서드명(UpdateInfo, Info, Setting, OnTimeEvent, UpdateInfoForItem, LoadData, Read_Value, BtnName_Click, SetNowEnd) 전부 반영 |
| `Window2.xaml`, `Window2.xaml.cs` | 환자정보 입력창(이름/투여시간(분→TimeSpan)/투여용량/수액종류) + `btn1_Click` |
| `PatientRow.cs` | DataGrid 바인딩용 모델(원문에 클래스명 언급 없음, 신규 설계) |
| `FirebaseSettings.example.cs` | config example/template. 실제 AuthSecret/BasePath 없음 |

## 5. CAD 선별/파일명 변경
- 최종본으로 선정: `케이스 외형 출력 디자인.3dm`(2024-11-18) + `케이스 외형 출력 디자인.stl`(2024-11-19) → 이 둘은 파일명이 동일한 쌍이고 STL이 3dm 하루 뒤에 생성되어, 이 3dm이 실제로 STL로 출력(내보내기)된 버전이라는 근거가 명확합니다.
- `hardware/case-design/iv_monitor_case.3dm`, `iv_monitor_case.stl`로 이름만 일반화해 복사. **내용은 SHA256 해시로 원본과 100% 동일함을 확인**했습니다(자세한 근거는 21~24번 항목 참조).
- **복사하지 않은 CAD 후보와 사유**는 아래 "CAD 최종본 조사" 절 참조.

## 6. 보안을 위해 추가한 template/config 처리
- `desktop-app/reimplemented-wpf/IvMonitorDesktop/FirebaseSettings.example.cs` — 실제 값 없이 `YOUR_FIREBASE_AUTH_SECRET` / `YOUR_PROJECT_ID` 플레이스홀더만 포함. 파일명에 `.example`을 붙여 실제 설정 파일(`FirebaseSettings.cs`, 커밋 금지 대상)과 구분되도록 함.
- Android 쪽은 이미 2단계의 `.ino` 원본이 플레이스홀더 상태였고, 이번 3단계에서 별도 config 파일을 추가하지 않았습니다(원본 그대로 유지 원칙).
- `mobile-app/reconstructed-app/local.properties`는 이 PC의 로컬 Android SDK 경로만 담고 있으며(개인정보 아님), Android 프로젝트 관례상 항상 `.gitignore` 대상입니다 — 다음 단계 `.gitignore` 작성 시 반드시 포함되어야 합니다.

---

## CAD 최종본 조사 (21~24번 항목 상세)

### 파일별 조사 결과
| 파일 | 수정일 | 크기 | 판단 |
|---|---|---|---|
| 케이스모델 디자인.3dm | 2024-11-09 | 85.8 KB | 초기 컨셉 스케치로 판단(가장 이른 날짜, 가장 작은 크기) |
| 4조_외형도안.3dm / .3dmbak | 2024-11-12 | 985.3 KB | 4차 회의(2024-11-12, 케이스 디자인 논의)와 날짜 일치. 전체 부품 통합 도면으로 추정(용량이 가장 큼) |
| **케이스 외형 출력 디자인.3dm** | 2024-11-18 | 262.2 KB | **STL과 페어링된 유일한 3dm** → 최종본으로 채택 |
| **케이스 외형 출력 디자인.stl** | 2024-11-19 | 274.1 KB | **폴더 내 유일한 STL 파일** → 실제 출력에 사용된 파일로 채택 |
| 케이스 외형 출력 디자인1.3dm / .3dmbak | 2024-11-25 / 26 | 231.8 / 231.1 KB | 6차 회의(2024-11-26, "수액 케이스 조립") 직전 최신 수정본이지만, **대응하는 STL이 없어 이 버전이 실제로 출력되었다는 근거가 없음** |
| *.3dmbak (전체) | — | — | Rhino 자동 백업 파일 — 원칙대로 전부 제외 |

### 판단 및 한계
"케이스 외형 출력 디자인1.3dm"이 날짜상으로는 가장 최신이지만, **STL 출력물과 직접 대응되는 파일이 아니므로** 이번 3단계에서는 `hardware/case-design/`에 복사하지 않았습니다. 이 파일이 실제 최종 조립에 쓰인 근본 디자인인지, 아니면 출력 이후의 미세 조정(출력과 무관한 내부 배치 변경 등)인지는 **문서만으로 확정할 수 없어 [판단 불가]**로 남깁니다. 필요 시 다음 단계에서 이 파일도 참고용으로 별도 포함할 수 있습니다(사용자 확인 필요).

### CAD 파일명 일반화 및 해시 검증
```
원본: 케이스 외형 출력 디자인.3dm
사본: hardware/case-design/iv_monitor_case.3dm
SHA256(원본) = 6ee5b9e4aca50f48c0c6bf2c79445ff80fb311299e47739b4f525db3524f2d4b
SHA256(사본) = 6ee5b9e4aca50f48c0c6bf2c79445ff80fb311299e47739b4f525db3524f2d4b  (동일)

원본: 케이스 외형 출력 디자인.stl
사본: hardware/case-design/iv_monitor_case.stl
SHA256(원본) = 3599b7c6182944d6a2c27390522f9529293366e06db966672c309b8dbddf050c
SHA256(사본) = 3599b7c6182944d6a2c27390522f9529293366e06db966672c309b8dbddf050c  (동일)
```
파일명만 변경, 내용은 100% 동일합니다.

### CAD 내부 metadata 검사
- STL 헤더(첫 80바이트): `"Rhinoceros Binary STL ( Nov 12 2024 )"` — 이는 Rhino 소프트웨어 자체의 빌드/버전 문자열로 보이며, 파일의 실제 내보내기 날짜(2024-11-19, 파일시스템 mtime)와는 무관합니다. 개인정보는 아닙니다.
- 3dm 파일들에 대해 팀원 실명 패턴, 이메일 패턴, `C:\Users\...` 절대경로 패턴을 바이너리 레벨로 스캔한 결과 **검출되지 않았습니다.**
- 다만 Rhino의 OpenNURBS 바이너리 포맷 내부에는 "문서 속성(작성자, 회사명 등)" 필드가 별도로 존재할 수 있으며, 전용 SDK/Rhino 프로그램 없이는 이 필드를 완전히 파싱해 확인할 수 없습니다 → **[확인 불가]**로 정직하게 남깁니다. 공개 전 Rhino에서 직접 열어 "문서 속성"을 확인/삭제할 것을 권장합니다.

---

## WPF Target Framework 선택 근거 (16번 항목)
- 문서(최종보고서, WPF 담당 팀원 개인보고서)에는 정확한 .NET Target Framework 버전이 전혀 언급되어 있지 않습니다 → 원본 근거 없음.
- 이 PC 확인 결과: `dotnet --info` 기준 **.NET SDK는 설치되어 있지 않고**, .NET 7 런타임(`Microsoft.WindowsDesktop.App 7.0.0`, `Microsoft.NETCore.App 7.0.0`)만 설치되어 있습니다.
- SDK가 없어 실제 빌드는 애초에 불가능하지만, "설치되면 가장 호환 가능성이 높은" 선택으로 **net7.0-windows**를 지정했습니다. 이는 **[신규 재구현용 기술 선택]**이며 과거 원본 환경이 아닙니다.

## FireSharp 처리 결과 (17번 항목)
- `IvMonitorDesktop.csproj`에 `PackageReference Include="FireSharp" Version="2.0.4"`를 그대로 포함했습니다(다른 라이브러리로 교체하지 않음).
- **알려진 위험**: FireSharp은 2016년경 마지막으로 배포되었고 .NET Framework 4.5를 대상으로 하는 것으로 알려져 있어, `net7.0-windows`(SDK 스타일 프로젝트)와의 실제 호환성은 **검증되지 않았습니다.**
- 이 PC에 .NET SDK가 없어 `dotnet restore`/`dotnet build`를 실제로 실행할 수 없었고, 따라서 "빌드된다" 또는 "빌드되지 않는다"를 확정할 수 없습니다 → **[확인 필요/미검증]**으로 보고합니다. 임의로 다른 Firebase 클라이언트 라이브러리로 바꾸지 않았습니다.
