// [신규 재구현] 당시 원본 Window2.xaml.cs 코드 본문은 존재하지 않습니다.
// 원문: "btn1_Click : 입력받은 정보들을 저장한다. 받은 정보들은 MainWindow에서 바로
// 사용할 수 있도록 변환을 마쳐서 저장한다." (투여 시간은 분 -> TimeSpan "00:00:00" 형태로 변환)
using System;
using System.Windows;

namespace IvMonitorDesktop
{
    public partial class Window2 : Window
    {
        public string EnteredName { get; private set; }
        public TimeSpan EnteredDosageTime { get; private set; }
        public double EnteredDosageVolume { get; private set; }
        public string EnteredFluidType { get; private set; }

        public Window2()
        {
            InitializeComponent();
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            EnteredName = NameBox.Text;

            if (int.TryParse(DosageTimeBox.Text, out int minutes))
            {
                EnteredDosageTime = TimeSpan.FromMinutes(minutes); // 원문: 분 -> "00:00:00" 형태로 변환
            }

            if (double.TryParse(DosageVolumeBox.Text, out double volume))
            {
                EnteredDosageVolume = volume;
            }

            EnteredFluidType = FluidTypeBox.Text;

            DialogResult = true;
            Close();
        }
    }
}
