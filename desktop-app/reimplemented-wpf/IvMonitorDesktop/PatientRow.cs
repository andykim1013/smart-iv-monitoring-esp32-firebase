// [신규 재구현] DataGrid 한 행을 표현하는 모델 클래스입니다.
// 컬럼 6종(호실/이름/시작시간/종료시간/예상종료시간/진행도)은 desktop-app/README.md에 정리된
// 최종보고서 ver1.5.0 원문 설명을 근거로 합니다. 클래스 자체는 원문에 존재하지 않던 신규 설계입니다.
using System.ComponentModel;

namespace IvMonitorDesktop
{
    public class PatientRow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _room;
        public string Room
        {
            get => _room;
            set { _room = value; OnPropertyChanged(nameof(Room)); }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private string _startTime;
        public string StartTime
        {
            get => _startTime;
            set { _startTime = value; OnPropertyChanged(nameof(StartTime)); }
        }

        private string _endTime;
        public string EndTime
        {
            get => _endTime;
            set { _endTime = value; OnPropertyChanged(nameof(EndTime)); }
        }

        private string _expectedEndTime;
        public string ExpectedEndTime
        {
            get => _expectedEndTime;
            set { _expectedEndTime = value; OnPropertyChanged(nameof(ExpectedEndTime)); }
        }

        private string _progress;
        public string Progress
        {
            get => _progress;
            set { _progress = value; OnPropertyChanged(nameof(Progress)); }
        }

        // SetNowEnd에서 예상종료시간/진행도 계산에 사용하는 값들 (원문: "예상종료시간과 진행도 계산에
        // 필요한 시작무게, 시작시간을 저장")
        public double StartWeight { get; set; }
        public System.DateTime? StartDateTime { get; set; }
        public bool HasData => !string.IsNullOrEmpty(StartTime);

        public override string ToString() => Room;
    }
}
