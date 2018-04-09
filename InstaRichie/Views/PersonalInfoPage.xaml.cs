// **************************************************************************
//Start Finance - An to manage your personal finances.

//Start Finance is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//Start Finance is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Start Finance.If not, see<http://www.gnu.org/licenses/>.
// ***************************************************************************

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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfoPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public PersonalInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<PersonalInfo>();
            var query = conn.Table<PersonalInfo>();
            InfoList.ItemsSource = query.ToList();
        }



        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstNameTxtBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("First Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (LastNameTxtBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Last Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (EmailTxtBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Email not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (PhoneTxtBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Phone not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {   // Inserts the data
                    conn.Insert(new PersonalInfo()
                    {
                        FirstName = FirstNameTxtBox.Text,
                        LastName = LastNameTxtBox.Text,
                        DOB = Date(),
                        Gender = GenderComboBox.SelectedItem.ToString(),
                        Email = EmailTxtBox.Text,
                        MobilePhone = PhoneTxtBox.Text
                    });
                    Results();
                }
            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Entry already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }



        // Date conversion
        public string Date()
        {
            DateTimeOffset date = (DateTimeOffset)DOBPicker.Date;
            return date.Date.ToString("dd/MM/yyyy");
        }


        // Clears the fields
        private async void ClearFileds_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ClearDialog = new MessageDialog("Cleared", "information");
            await ClearDialog.ShowAsync();
        }

        // Displays the data when navigation between pages
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Deleting this Info will delete all info of this account", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                // checks if data is null else inserts
                try
                {
                    int PersonalInfoLabel = ((PersonalInfo)InfoList.SelectedItem).PersonalID;
                    var querydel = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE PersonalID ='" + PersonalInfoLabel + "'");
                    Results();
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }

        private async void EditItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstNameTxtBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("First Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (LastNameTxtBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Last Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (EmailTxtBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Email not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (PhoneTxtBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Phone not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {   // Inserts the data

                    int InfoLabel = ((PersonalInfo)InfoList.SelectedItem).PersonalID;

                    var queryEdit = conn.Query<PersonalInfo>(
                         "UPDATE PersonalInfo SET FirstName = '" + FirstNameTxtBox.Text +
                        "', LastName = '" + LastNameTxtBox.Text +
                        "', DOB = '" + Date() +
                        "', Gender = '" + GenderComboBox.SelectedItem.ToString() +
                        "', Email = '" + EmailTxtBox.Text.ToString() +
                         "', MobilePhone = '" + PhoneTxtBox.Text +
                        "' WHERE PersonalID = " + InfoLabel);
                    Results();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog ClearDialog = new MessageDialog("Please select the item to edit!", "Oops..!");
                await ClearDialog.ShowAsync();
            }
        }

    }
}
