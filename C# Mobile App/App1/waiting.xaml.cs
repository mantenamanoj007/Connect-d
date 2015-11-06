using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
//using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class waiting : Page
    {
        public string URI = "http://1.186.5.164:8080";
        string gid;
        public object Thread { get; set; }

        public waiting()
        {
            this.InitializeComponent();
        }
        int i=0;
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            GetaString(i);

            /*
            while (true)
            {
                //if (gid != "Fetching Group ID")
                {
                    HttpClient httpClient = new HttpClient();
                    string ResponseString = await httpClient.GetStringAsync(URI + "/getmembers?" + "groupid=" + gid);

                    members.Text = ResponseString;
                }
            }*/
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //GetaString();
            this.Frame.Navigate(typeof(map),gid);
            
        }

        public async void GetaString(int i)
        {
            try
            {
                //Create HttpClient
                HttpClient httpClient = new HttpClient();

                //Define Http Headers
                //httpClient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

                //ur += "?";
                //ur+= i.ToString();

                //Call
                string ResponseString = await httpClient.GetStringAsync(URI+"/getgroupid");
                //Replace current URL with your URL
                gid = ResponseString;
                giddisplay.Text = ResponseString;

                //MessageDialog m = new MessageDialog(ResponseString);
                //await m.ShowAsync();
            }

            catch (Exception ex)
            {
                //....
            }
        }
    }

}
