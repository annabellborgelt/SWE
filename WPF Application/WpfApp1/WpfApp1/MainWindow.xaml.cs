using WpfApp1.GazeSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static readonly DependencyProperty CurrentWordProperty = DependencyProperty.Register("CurrentWord", typeof(string), typeof(MainWindow));
		private readonly TimeSpan GazeTimeMilliseconds = TimeSpan.FromSeconds(0.9);
		private GazeSource GazeSource;
		private DispatcherTimer Timer;
		private Action TimerAction;
		private Button HighlightedButton;

		public MainWindow()
		{
			InitializeComponent();
			GazeSource = GazeSourceFactory.Create();
			GazeSource.UseCalibration = true;
			GazeSource.UseSmoothing = true;
			GazeSource.DataAvailable += GazeSource_DataAvailable;
			Timer = new DispatcherTimer();
			Timer.Interval = GazeTimeMilliseconds;
			Timer.IsEnabled = false;
			Timer.Tick += Timer_Tick;
		}

		private void Calibrate()
		{
			var calibrationWindow = new CalibrationWindow();
			calibrationWindow.ShowDialog();
		}

		private void TimeOut(Action action)
		{
			Timer.IsEnabled = false;
			TimerAction = action;
			Timer.IsEnabled = true;
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (DateTime.UtcNow - GazeSource.DataTimeStamp < GazeTimeMilliseconds)
				TimerAction?.Invoke();
		}


		private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.C)
				Calibrate();
			if (e.Key == System.Windows.Input.Key.Escape)
				Close();
		}

	}

	private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
	{
		Timer.IsEnabled = false;
		TimerAction = null;
		HighlightedButton = null;
	}

	private void GazeSource_DataAvailable(object sender, EventArgs e)
	{
		System.Windows.Forms.Cursor.Position = GazeSource.DataPoint;
	}


}
}
