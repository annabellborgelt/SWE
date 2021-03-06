﻿using iGaze.GazeSources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace iGaze
{
	/// <summary>
	/// Interaction logic for CalibrationWindow.xaml
	/// </summary>
	public partial class CalibrationWindow : Window
	{
		private GazeSource GazeSource;
        private DispatcherTimer TimeoutTimer;
		private DispatcherTimer GazeTimer;
		private List<System.Drawing.Point> DataPoints;
		private DateTime LastSampleTime;
		private PointF[] AverageDataPoints = new PointF[4];
		private int DataPointIndex = 0;

		public CalibrationWindow()
		{
			InitializeComponent();
		}

		private void StartCalibration()
		{
			System.Windows.Forms.Cursor.Hide();
			DataPointIndex = 0;
			DataPoints = new List<System.Drawing.Point>();
			GazeSource = GazeSourceFactory.Create();
			GazeSource.UseCalibration = false;
			GazeSource.UseSmoothing = true;
			GazeTimer = new DispatcherTimer();
			GazeTimer.Interval = TimeSpan.FromMilliseconds(1000 / 50);
			GazeTimer.Tick += GazeTimerTick;
			GazeTimer.Start();
            TimeoutTimer = new DispatcherTimer();
            TimeoutTimer.Interval = TimeSpan.FromSeconds(8);
            TimeoutTimer.Tick += TimeoutTimer_Tick;
            TimeoutTimer.Start();
			ClearFocusPointLocation();
			Canvas.SetLeft(FocusPoint, 0);
			Canvas.SetTop(FocusPoint, 0);
		}

        private void TimeoutTimer_Tick(object sender, EventArgs e)
        {
            MessageBox.Show("I'm not getting any eye data from you");
            Close();
        }

        private void GazeTimerTick(object sender, EventArgs e)
		{
			if (GazeSource.DataTimeStamp == LastSampleTime)
				return;

			System.Windows.Point controlPointOnScreen = FocusPoint.PointToScreen(new System.Windows.Point(FocusPoint.Width / 2, FocusPoint.Height / 2));
			var gazeRectangle = new Rectangle((int)controlPointOnScreen.X, (int)controlPointOnScreen.Y, 0, 0);
			gazeRectangle.Inflate(300, 300);

			if (!gazeRectangle.Contains(GazeSource.DataPoint))
				return;

			if (GazeSource.DataTimeStamp - LastSampleTime > TimeSpan.FromSeconds(1))
				DataPoints.Clear();

			DataPoints.Add(GazeSource.DataPoint);
			LastSampleTime = GazeSource.DataTimeStamp;
			if (DataPoints.Count == 100)
			{
				double x = DataPoints.Skip(DataPoints.Count / 2).Average(p => p.X);
				double y = DataPoints.Skip(DataPoints.Count / 2).Average(p => p.Y);
				DataPoints.Clear();
				AverageDataPoints[DataPointIndex] = new PointF((float)x, (float)y);
				DataPointIndex++;

                TimeoutTimer.Stop();
                TimeoutTimer.Start();

				switch (DataPointIndex)
				{
					case 1:
						ClearFocusPointLocation();
						Canvas.SetRight(FocusPoint, 0);
						Canvas.SetTop(FocusPoint, 0);
						break;

					case 2:
						ClearFocusPointLocation();
						Canvas.SetRight(FocusPoint, 0);
						Canvas.SetBottom(FocusPoint, 0);
						break;

					case 3:
						ClearFocusPointLocation();
						Canvas.SetLeft(FocusPoint, 0);
						Canvas.SetBottom(FocusPoint, 0);
						break;

					case 4:
						GazeSource.SetCalibrationPoints(AverageDataPoints[0], AverageDataPoints[1], AverageDataPoints[2], AverageDataPoints[3]);
						Close();
						break;

				}
			}
		}

		private void ClearFocusPointLocation()
		{
			FocusPoint.ClearValue(Canvas.TopProperty);
			FocusPoint.ClearValue(Canvas.BottomProperty);
			FocusPoint.ClearValue(Canvas.LeftProperty);
			FocusPoint.ClearValue(Canvas.RightProperty);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			GazeTimer.Stop();
			GazeSource.Dispose();
			System.Windows.Forms.Cursor.Show();
		}

		private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
				StartCalibration();
		}
	}
}
