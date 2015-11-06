using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class create : Page
    {

        public class UnameRadius
        {
            public String username { get; set; }
            public Int32 rad { get; set; }
            public double lat { get; set; }
            public double lon { get; set; }
            public double accurate { get; set; }
        }

        public create()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public string URI = "http://1.186.5.164:8080/creategroup";
        string json;

        private async void submit_Click(object sender, RoutedEventArgs e)
        {
            string uname = username.Text;
            string rad = radius.Text;

            var js = new UnameRadius { username = uname, rad = Int32.Parse(rad), lat = 13.351320074, lon = 74.79314491152, accurate = 10 };
            json = JsonConvert.SerializeObject(js);

            this.Frame.Navigate(typeof(waiting));

            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await hc.PostAsync(URI, new StringContent(json));
            }
        }
    }
}
