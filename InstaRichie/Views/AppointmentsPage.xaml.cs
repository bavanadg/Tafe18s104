using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using StartFinance.Models;
using SQLite.Net;

namespace StartFinance.Views
{
	public sealed partial class AppointmentsPage : Page
	{
		SQLiteConnection conn;
		string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

		public AppointmentsPage()
		{
			this.InitializeComponent();
			NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

			conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

			Results();
		}

		public void Results()
		{
			conn.CreateTable<Appointments>();
			var query = conn.Table<Appointments>();
			AppointmentsList.ItemsSource = query.ToList();
		}

		public void Page_Loaded(object sender, RoutedEventArgs e)
		{
			Results();
		}

		public async void AppBarButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (ApptName.Text.ToString() == "")
				{
					MessageDialog dialog = new MessageDialog("Appointment name not entered", "Oops..!");
					await dialog.ShowAsync();
				}
				else if (ApptLoc.Text.ToString() == "")
				{
					MessageDialog dialog = new MessageDialog("Appointment location not entered", "Oops..!");
					await dialog.ShowAsync();
				}
				else
				{
					conn.Insert(new Appointments()
					{
						eventName = ApptName.Text,
						location = ApptLoc.Text,
						eventDate = Date(),
						startTime = Time(ApptStart.Time),
						endTime = Time(ApptFinish.Time)
					});
					Results();
				}
			}
			catch (Exception ex)
			{
				if (ex is FormatException)
				{
					MessageDialog dialog = new MessageDialog("You entered some invalid data!", "Oops..!");
					await dialog.ShowAsync();
				}
				else
				{
					MessageDialog dialog = new MessageDialog("An unexpected error occured!", "Oops..!");
					await dialog.ShowAsync();
				}
			}
		}

		public async void DeleteItem_Click(object sender, RoutedEventArgs e)
		{
			MessageDialog showConf = new MessageDialog("Are you sure you want to delete this appointment?", "Important!");
			showConf.Commands.Add(new UICommand("Yes, delete")
			{
				Id = 0
			});
			showConf.Commands.Add(new UICommand("Cancel")
			{
				Id = 1
			});
			showConf.DefaultCommandIndex = 0;
			showConf.CancelCommandIndex = 1;

			var result = await showConf.ShowAsync();
			if ((int)result.Id == 0)
			{
				try
				{
					int AppointmentsLabel = ((Appointments)AppointmentsList.SelectedItem).appointmentID;
					var queryDel = conn.Query<Appointments>("DELETE FROM Appointments WHERE appointmentID=" + AppointmentsLabel);
					Results();
				}
				catch (NullReferenceException)
				{
					MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
					await ClearDialog.ShowAsync();
				}
			}
		}

		public async void EditItem_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (ApptName.Text.ToString() == "")
				{
					MessageDialog dialog = new MessageDialog("Appointment name not entered", "Oops..!");
					await dialog.ShowAsync();
				}
				else if (ApptLoc.Text.ToString() == "")
				{
					MessageDialog dialog = new MessageDialog("Appointment location not entered", "Oops..!");
					await dialog.ShowAsync();
				}
				else
				{
					int AppointmentsLabel = ((Appointments)AppointmentsList.SelectedItem).appointmentID;
					
					
					var queryEdit = conn.Query<Appointments>(
						"UPDATE Appointments SET eventName = '" +
						ApptName.Text + "', location = '" +
						ApptLoc.Text + "', eventDate = '" +
						Date() + "', startTime = '" + 
						Time(ApptStart.Time) + "', endTime = '" +
						Time(ApptFinish.Time) + "' WHERE appointmentID = " + AppointmentsLabel);
					Results();
				}
			}
			catch (NullReferenceException)
			{
				MessageDialog ClearDialog = new MessageDialog("Please select the item to edit!", "Oops..!");
				await ClearDialog.ShowAsync();
			}
		}

		public string Date()
		{
			DateTimeOffset date = (DateTimeOffset)ApptDate.Date;
			return date.Date.ToString("dd/MM/yyyy");
		}

		public string Time(TimeSpan time)
		{
			return time.ToString(@"h\:mm\:ss");
		}

		private void AppointmentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (AppointmentsList.SelectedIndex != -1)
			{
				ApptName.Text = ((Appointments)AppointmentsList.SelectedItem).eventName;
				ApptLoc.Text = ((Appointments)AppointmentsList.SelectedItem).location;
				ApptDate.Date = Convert.ToDateTime(((Appointments)AppointmentsList.SelectedItem).eventDate);
				ApptStart.Time = TimeSpan.Parse(((Appointments)AppointmentsList.SelectedItem).startTime);
				ApptFinish.Time = TimeSpan.Parse(((Appointments)AppointmentsList.SelectedItem).endTime);
			}
		}
	}
}