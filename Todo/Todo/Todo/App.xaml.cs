using Todo.Services;
using Xamarin.Forms;

namespace Todo
{
    public partial class App : Application
    {
        public int ResumeAtTodoId { get; set; }

        static TodoItemService service;

        public static TodoItemService Service
        {
            get
            {
                if (service == null)
                {
                    // this is localhost for android network
                    service = new TodoItemService("http://10.0.2.2:1476/api");
                }

                return service;
            }
        }

        public App()
        {
            Resources = new ResourceDictionary();
            Resources.Add("primaryGreen", Color.FromHex("91CA47"));
            Resources.Add("primaryDarkGreen", Color.FromHex("6FA22E"));

            InitializeComponent();
            var mainPage = new NavigationPage(new MainPage());
            mainPage.BarBackgroundColor = (Color)App.Current.Resources["primaryGreen"];
            mainPage.BarTextColor = Color.White;
            MainPage = mainPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
