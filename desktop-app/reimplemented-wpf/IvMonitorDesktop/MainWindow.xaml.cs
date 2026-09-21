// [신규 재구현] 당시 원본 MainWindow.xaml.cs 코드 본문은 존재하지 않습니다(클래스/메서드명과
// 역할 설명만 최종보고서 ver1.5.0 텍스트로 확인됨, desktop-app/README.md 참조).
// 아래 메서드 이름(UpdateInfo, Info, Setting, OnTimeEvent, UpdateInfoForItem, LoadData, Read_Value,
// BtnName_Click, SetNowEnd)은 원문에서 확인된 이름을 그대로 유지했고, 실제 구현 로직은 원문 설명
// (역할 설명 + Firebase 경로 + DataGrid 컬럼 + 5초 주기 갱신)을 근거로 새로 작성했습니다.
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace IvMonitorDesktop
{
    public partial class MainWindow : Window
    {
        // 원문 확인: "안드로이드 스튜디오, 프로그램과 파이어베이스 연동" — 경로는 최종보고서에서
        // 확인된 /802/StartTime, /802/EndTime, /802/speed, /802/StartWeight 만 사용합니다.
        private readonly IFirebaseClient _firebaseClient;
        private readonly ObservableCollection<PatientRow> _patients = new ObservableCollection<PatientRow>();
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        // 원문: "각 층의 호실을 한눈에 확인" — 그러나 문서/코드에 실제로 등장하는 호실은 "802" 하나뿐이라
        // 다른 호실 목록은 근거가 없어 임의로 만들지 않았습니다(802만 시연용으로 포함).
        private static readonly string[] KnownRooms = { "802" };

        public MainWindow()
        {
            InitializeComponent();

            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = FirebaseSettings.AuthSecret,
                BasePath = FirebaseSettings.BasePath
            };
            _firebaseClient = new FireSharp.FirebaseClient(config);

            Info();

            // 원문: "실시간 업데이트를 위한 타이머 또한 실행된다" (5초 주기)
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += OnTimeEvent;
            _timer.Start();
        }

        // 원문: "MainWindow 실행 시 실행되며, DataGrid의 행을 생성한다"
        private void Info()
        {
            _patients.Clear();
            foreach (var room in KnownRooms)
            {
                _patients.Add(new PatientRow { Room = room });
            }
            LoadData();
        }

        // 원문: "프로그램 실행시 작동하며, DataGird를 생성하는 역할을 한다.
        // 호실을 제외한 셀들은 공백이며..."
        private void LoadData()
        {
            PatientGrid.ItemsSource = _patients;
        }

        // 원문: "행마다 데이터가 존재하는지 확인하며 존재하면 현재 값을 계산하여 업데이트한다."
        private async void OnTimeEvent(object sender, EventArgs e)
        {
            foreach (var row in _patients)
            {
                if (row.HasData)
                {
                    await UpdateInfoForItem(row);
                }
            }
        }

        // 원문: "파이어베이스에서 가져온 데이터 변환"
        private async Task UpdateInfoForItem(PatientRow row)
        {
            FirebaseResponse endTimeResponse = await _firebaseClient.GetAsync($"{row.Room}/EndTime");
            FirebaseResponse speedResponse = await _firebaseClient.GetAsync($"{row.Room}/speed");

            string endTime = endTimeResponse.Body != null ? endTimeResponse.ResultAs<string>() : null;
            float? speed = speedResponse.Body != null ? speedResponse.ResultAs<float?>() : null;

            Setting(row, endTime, speed);
        }

        // 원문: "파이어베이스에서 데이터를 가져왔을 때 가져온 데이터를 이용하여 필요한 데이터를 만든다."
        private void Setting(PatientRow row, string endTime, float? speed)
        {
            if (!string.IsNullOrEmpty(endTime))
            {
                row.EndTime = endTime;
                row.ExpectedEndTime = endTime;
            }

            if (row.StartDateTime.HasValue && row.StartWeight > 0 && speed.HasValue && speed.Value != 0)
            {
                double elapsedMinutes = (DateTime.Now - row.StartDateTime.Value).TotalMinutes;
                double estimatedTotalMinutes = row.StartWeight / (Math.Abs(speed.Value) * 60.0);
                int progressPercent = estimatedTotalMinutes > 0
                    ? Math.Min(100, Math.Max(0, (int)(elapsedMinutes / estimatedTotalMinutes * 100)))
                    : 0;
                row.Progress = $"{progressPercent}%";
            }
        }

        // 원문: "[입력] 버튼을 누르면 실행되며, Window2 창을 실행하며 창이 종료되면
        // 입력된 정보들을 저장한다."
        private void BtnName_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var row = (PatientRow)button.Tag;

            var window2 = new Window2();
            bool? result = window2.ShowDialog();

            if (result == true)
            {
                UpdateInfo(row, window2.EnteredName, window2.EnteredDosageTime,
                    window2.EnteredDosageVolume, window2.EnteredFluidType);
            }
        }

        // 원문: "Window2 창을 닫을 때 실행되며 입력한 정보들을 DataGrid에 사용할 수 있게 한다."
        private async void UpdateInfo(PatientRow row, string name, TimeSpan dosageTime, double dosageVolume, string fluidType)
        {
            row.Name = name;
            row.StartTime = DateTime.Now.ToString("HH:mm:ss");
            row.StartDateTime = DateTime.Now;
            row.StartWeight = dosageVolume;

            SetNowEnd(row);
            await Read_Value(row);
        }

        // 원문: "예상종료시간과 진행도 계산에 필요한 시작무게, 시작시간을 저장하며, DataGrid를 업데이트한다."
        private void SetNowEnd(PatientRow row)
        {
            row.Progress = "0%";
            LoadData();
        }

        // 원문: "파이어베이스 데이터 일부를 가져온다. Window2 종료시 작동하며 speed와 EndTime을 가져온다."
        private async Task Read_Value(PatientRow row)
        {
            FirebaseResponse endTimeResponse = await _firebaseClient.GetAsync($"{row.Room}/EndTime");
            FirebaseResponse speedResponse = await _firebaseClient.GetAsync($"{row.Room}/speed");

            string endTime = endTimeResponse.Body != null ? endTimeResponse.ResultAs<string>() : null;
            float? speed = speedResponse.Body != null ? speedResponse.ResultAs<float?>() : null;

            Setting(row, endTime, speed);
        }
    }
}
