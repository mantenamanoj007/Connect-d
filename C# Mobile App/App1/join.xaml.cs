using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
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
    public sealed partial class join : Page
    {
        public class memberdata
        {
            public String username { get; set; }
            public Int32 gid { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public double accuracy { get; set; }
        }

        public join()
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

        public string URI = "http://1.186.5.164:8080/joingroup";
        string json;

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            string uname = username.Text;
            string gid = groupid.Text;

            Geolocator geolocator = new Geolocator();
            Geoposition geoposition = null;
            try
            {
                geoposition = await geolocator.GetGeopositionAsync();
            }
            catch (Exception ex)
            {
                // Handle errors like unauthorized access to location
                // services or no Internet access.
            }

            double latitude = geoposition.Coordinate.Latitude;
            double longitude = geoposition.Coordinate.Longitude;
            double accuracy = geoposition.Coordinate.Accuracy; ;

            var js = new memberdata { username = uname, gid = Int32.Parse(gid), latitude = latitude, longitude = longitude, accuracy = accuracy };
            json = JsonConvert.SerializeObject(js);

            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await hc.PostAsync(URI, new StringContent(json));
            }

            jointomap obj = new jointomap();

            obj.username = uname;
            obj.gid = gid;
            
            this.Frame.Navigate(typeof(membermap), obj);

        }

    }
}
