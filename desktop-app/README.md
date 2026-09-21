# Desktop App (WPF / C#) — 사양 문서 (소스 미확보)

> 이 문서는 **소스 코드가 아니라 복원 근거 문서**입니다.
> 원본 자료(최종보고서 ver1.5.0, WPF 담당자 개인보고서, 최종발표 V1.0.0)를 전수 조사한 결과,
> 이 프로그램의 **실제 텍스트 형태 전체 소스코드(.xaml, .xaml.cs, .csproj, .sln)는 현재 자료에 존재하지 않습니다.**
> 존재하는 것은 클래스/메서드 이름, 역할 설명, 화면 스크린샷(이미지)뿐이며, 이번 2단계에서는 이 정보만 문서화하고
> `.xaml` / `.xaml.cs` / `.csproj` / `.sln` 파일은 **생성하지 않았습니다.**

## 근거 자료
| 자료 | 근거 수준 |
|---|---|
| 최종보고서 ver1.5.0 "3-2 프로그램" 섹션 | 클래스/메서드 이름 + 역할 설명 텍스트, 코드 본문은 스크린샷(이미지) |
| WPF 담당 팀원 개인보고서 (원본 폴더 내 hwpx 파일 — 파일명에 학번+실명 포함되어 이 문서에서는 생략) | FireSharp 패키지 설치 절차 설명 + import 문 일부(`using FireSharp;` 등 패키지명만) 텍스트, 코드 본문은 스크린샷 |
| 최종발표 V1.0.0 PPT | 코드/클래스 관련 텍스트 없음 (해당 없음) |

## 확인된 기술 스택
- **UI 프레임워크**: WPF (Windows Presentation Foundation)
- **언어**: C#
- **Firebase 연동 패키지**: **FireSharp** (NuGet 패키지, WPF에서 Firebase Realtime Database 연동용으로 명시적으로 언급됨)
- **연동 대상**: Firebase Realtime Database (ESP32 펌웨어와 동일한 DB)

## 확인된 화면(클래스) 구성
### MainWindow
- 역할: 간호사 데스크가 여러 환자의 수액 상태를 한 화면에서 확인하는 메인 화면
- 구성 요소: `DataGrid` — 컬럼: **호실, 이름, 시작시간, 종료시간, 예상종료시간, 진행도**
- 동작: 프로그램 시작 시 호실을 제외한 셀은 공백으로 표시, 이름 열에 `Window2`를 여는 버튼 존재
- 실시간 업데이트: 타이머로 **5초 주기** 갱신, Firebase 값 변경 시에도 갱신

### Window2
- 역할: 환자 정보 입력 창 (이름, 투여 시간, 투여 용량, 수액 종류 입력)
- 동작: 입력 후 [확인] 버튼 클릭 시 창이 닫히고 `MainWindow`의 `DataGrid` 해당 행에 정보 반영
- 투여 시간은 분 단위 입력을 `TimeSpan`("00:00:00" 형식)으로 변환해 저장

## 확인된 메서드 이름과 역할 (본문 텍스트 근거, 구현 코드 본문은 없음)
| 메서드 | 소속(추정) | 역할 설명(원문 근거) |
|---|---|---|
| `UpdateInfo` | MainWindow | `Window2` 종료 시 실행, 입력값을 `DataGrid`에 반영 |
| `Info` | MainWindow | `MainWindow` 실행 시 `DataGrid` 행 생성 |
| `Setting` | MainWindow | Firebase에서 가져온 데이터를 가공 |
| `OnTimeEvent` | MainWindow | 행마다 데이터 존재 여부 확인 후 현재 값 계산·갱신 |
| `UpdateInfoForItem` | MainWindow | Firebase에서 가져온 데이터 변환 |
| `LoadData` | MainWindow | 프로그램 실행 시 `DataGrid` 생성(호실 제외 공백 처리) |
| `Read_Value` | MainWindow | Firebase에서 `speed`, `EndTime` 일부 데이터 조회 (`Window2` 종료 시 동작) |
| `BtnName_Click` | MainWindow | `[입력]` 버튼 클릭 시 `Window2` 실행 및 종료 후 값 저장 |
| `SetNowEnd` | MainWindow | 예상종료시간·진행도 계산에 필요한 시작무게/시작시간 저장, `DataGrid` 갱신 |
| `btn1_Click` | Window2 | 입력값 저장, `MainWindow`에서 사용 가능한 형식으로 변환 |

## 확인 가능한 Firebase 데이터 구조 (ESP32 펌웨어와 공유)
```
/802/StartTime
/802/EndTime
/802/speed
/802/StartWeight
```
(802는 예시 병실 호실 번호로, 문서 내 시연 예시입니다.)

## 환자정보 입력 흐름 (문서 근거)
```
프로그램 실행 → MainWindow 로드 → DataGrid에 호실만 표시(나머지 공백)
   ↓ [이름 열의 입력 버튼 클릭] (BtnName_Click)
Window2 표시 → 이름 / 투여시간(분→TimeSpan 변환) / 투여용량 / 수액종류 입력
   ↓ [확인] 버튼 (btn1_Click)
입력값 저장 → Window2 종료 → UpdateInfo 실행 → DataGrid 해당 행 갱신
   ↓ (백그라운드, 5초 주기)
Firebase에서 speed/EndTime 등 재조회(Read_Value) → 진행도·예상종료시간 재계산 → DataGrid 갱신
```

## 명시적으로 하지 않은 것 (2단계 기준)
- `.xaml` / `.xaml.cs` / `.csproj` / `.sln` 신규 작성 — 2단계에서는 하지 않음
- 위 메서드 이름을 기반으로 한 코드 본문 추측/작성 — 2단계에서는 하지 않음
- FireSharp 연동 코드, DataGrid 바인딩 코드 등 실제 구현부 재현 — 2단계에서는 하지 않음

## 3단계 갱신: 재구현 완료
위 명세를 근거로 3단계에서 [reimplemented-wpf/](reimplemented-wpf/)에 **[신규 재구현]** 코드를 작성했습니다(당시 원본이 아님). `.NET SDK`가 이 PC에 없어 실제 build는 검증하지 못했고(정적검증만 완료), FireSharp의 net7.0-windows 호환성도 미검증입니다. 상세 내용은 [../docs/reconstruction-changes.md](../docs/reconstruction-changes.md)와 [../docs/reconstruction-status.md](../docs/reconstruction-status.md)를 참조하십시오.

## 결론
WPF 데스크톱 프로그램은 **당시 실제 소스가 유실**된 상태였고, 이 문서는 재구현의 근거가 된 **기능 명세서**입니다. 실제 재구현 코드는 `reimplemented-wpf/`에 있습니다.
