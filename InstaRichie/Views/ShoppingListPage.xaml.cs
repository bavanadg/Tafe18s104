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
    public sealed partial class ShoppingListPage : Page
    {
        SQLiteConnection conn;
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ShoppingListPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            conn.CreateTable<ShoppingList>();
            Results();
        }


        public void Results()
        {
            conn.CreateTable<ShoppingList>();
            var query = conn.Table<ShoppingList>();
            ItemList.ItemsSource = query.ToList();
        }

        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        public async void ShopingListItemAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ShopName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Shop Name not entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (NameOfItem.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Name of Item is not entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Insert(new ShoppingList()
                    {

                        shopName = ShopName.Text,
                        nameOfItem = NameOfItem.Text,
                        shoppingDate = Date(),
                        priceQuoted = Convert.ToDouble(PriceQuoted.Text),

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
            MessageDialog showConf = new MessageDialog("Are you sure you want to delete this shopping list?", "Important!");
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
                    int ItemID = ((ShoppingList)ItemList.SelectedItem).shoppingItemID;
                    var queryDel = conn.Query<ShoppingList>("DELETE FROM SHOPPINGLIST WHERE shoppingItemID=" + ItemID);
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
                if (ShopName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Shop name not entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (NameOfItem.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Name of Item not entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    int ItemID = ((ShoppingList)ItemList.SelectedItem).shoppingItemID;


                    var queryEdit = conn.Query<ShoppingList>(
                    "UPDATE ShoppingList SET shopName = '" +
                    ShopName.Text + "', nameofItem = '" +
                    NameOfItem.Text + "', shoppingDate = '" +
                    Date() + "', priceQuoted = '" +
                    Convert.ToDouble(PriceQuoted.Text) + "' WHERE shoppingItemID = " + ItemID);

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
            DateTimeOffset date = (DateTimeOffset)ShoppingDate.Date;
            return date.Date.ToString("dd/MM/yyyy");
        }


        private void ShopList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemList.SelectedIndex != -1)
            {
                ShopName.Text = ((ShoppingList)ItemList.SelectedItem).shopName;
                NameOfItem.Text = ((ShoppingList)ItemList.SelectedItem).nameOfItem;
                ShoppingDate.Date = Convert.ToDateTime(((ShoppingList)ItemList.SelectedItem).shoppingDate);
                PriceQuoted.Text = ((ShoppingList)ItemList.SelectedItem).priceQuoted.ToString();
            }

        }
    }
}
