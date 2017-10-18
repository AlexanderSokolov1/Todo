using System;
using System.Collections.Generic;
using System.Diagnostics;
using Todo.Models;
using Xamarin.Forms;

namespace Todo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtTodoId = -1;
            // listView.ItemsSource = await App.Service.GetAsync();
            listView.ItemsSource = new List<TodoItem>
            {
                new TodoItem
                {
                    Id = 1,
                    Done = false,
                    Name = "Item 1",
                    Notes = "Some Notes"
                },
                new TodoItem
                {
                    Id = 2,
                    Done = false,
                    Name = "Item 2",
                    Notes = "Some Notes"
                },
                new TodoItem
                {
                    Id = 3,
                    Done = true,
                    Name = "Item 3",
                    Notes = "Some Notes"
                }
            };
        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TodoItemPage
            {
                BindingContext = new TodoItem()
            });
        }

        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as TodoItem;
            ((App)App.Current).ResumeAtTodoId = item.Id;
            Debug.WriteLine("setting ResumeAtTodoId = " + item.Id);
            try
            {
                await Navigation.PushAsync(new TodoItemPage
                {
                    BindingContext = item
                });
            } catch (Exception ex)
            {

            }
        }
    }
}
