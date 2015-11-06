using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
    public sealed partial class membermap : Page
    {
        public string uname, gid;
        jointomap obj;
        private Geolocator locator = null;
        bool flag = false;
        public double radius;

        public class update
        {
            public string username { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public double accuracy { get; set; }
        }

        public class leaderdata
        {
            public string uname { get; set; }
            public double lon { get; set; }
            public double lat { get; set; }
            public double accuracy { get; set; }
        }

        public string URI = "http://1.186.5.164:8080";
        private CoreDispatcher dispatcher;

        public membermap()
        {
            this.InitializeComponent();
            dispatcher = Window.Current.CoreWindow.Dispatcher;
            func2();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        int k = 0;
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            k++;
            obj = e.Parameter as jointomap;
            uname = obj.username;
            gid = obj.gid;
            /*MessageDialog m = new MessageDialog(uname + " " + gid);
            m.ShowAsync();*/
            funcparse();

            HttpClient httpClient = new HttpClient();
            string rad = await httpClient.GetStringAsync(URI + "/getradius?" + "gid=" + gid + "&k=" + k);

            radius = Convert.ToDouble(rad);
        }
        int i = 0;
        int j = 0;
        async public void funcparse()
        {
            i++;
            HttpClient httpClient = new HttpClient();
            string ResponseString = await httpClient.GetStringAsync(URI + "/getleaderlocation?" + "gid=" + gid + "&i="+i);

            if (ResponseString != "")
            {
                JObject j = JsonConvert.DeserializeObject<JObject>(ResponseString);
                JArray ja = (JArray)j["data"];
                //Debug.WriteLine(ja.Count);

                foreach (JObject jj in ja)
                {
                    leaderdata mm = new leaderdata()
                    {
                        accuracy = (double)jj["accuracy"],
                        uname = (string)jj["username"],
                        lat = (double)jj["lat"],
                        lon = (double)jj["lon"]
                    };
                    //Debug.WriteLine(mm.uname);
                }

            }
            else
            {
                MessageDialog m1 = new MessageDialog("NHP");
                await m1.ShowAsync();
            }
        }
        

        private void func2()
        {
            if (locator == null)
            {
                locator = new Geolocator();
            }
            if (locator != null)
            {
                locator.DesiredAccuracy = PositionAccuracy.High;
                locator.DesiredAccuracyInMeters = 5;
                locator.MovementThreshold = 1;

                locator.PositionChanged +=
                    new TypedEventHandler<Geolocator,
                        PositionChangedEventArgs>(locator_PositionChanged);

            }
        }

        async private void locator_PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {

                Geoposition geoPosition = e.Position;

                double lat = geoPosition.Coordinate.Point.Position.Latitude;
                double longi = geoPosition.Coordinate.Point.Position.Longitude;
                double accuracy = geoPosition.Coordinate.Accuracy;

                var js = new update { username = uname, latitude = lat, longitude = longi, accuracy = accuracy };
                string json = JsonConvert.SerializeObject(js);

                using (HttpClient hc = new HttpClient())
                {
                    hc.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await hc.PostAsync(URI + "/updatemember?gid="+gid, new StringContent(json));


                }

                funcmap();
            });
        }

        private async void funcmap()
        {
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

            map1.Center = geoposition.Coordinate.Point;

            /*string text = "";
            text += "Latitude: ";
            text += geoposition.Coordinate.Latitude;
            text += "   Longitude: ";
            text += geoposition.Coordinate.Longitude;

            MessageDialog m = new MessageDialog(text);
            await m.ShowAsync();*/

            if (!flag)
            {
                map1.ZoomLevel = 15;
                flag = true;
            }

            map1.MapElements.Clear();
            MapIcon mapIcon = new MapIcon();
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/bluedot.png"));
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
            mapIcon.Location = geoposition.Coordinate.Point;
            mapIcon.Title = "You are here";
            map1.MapElements.Add(mapIcon);
            displayleader(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);

            /*Windows.UI.Xaml.Shapes.Ellipse fence = new Windows.UI.Xaml.Shapes.Ellipse();
            fence.Width = 30;
            fence.Height = 30;
            fence.Stroke = new SolidColorBrush(Colors.DarkOrange);
            fence.StrokeThickness = 2;
            MapControl.SetLocation(fence, geoposition.Coordinate.Point);
            MapControl.SetNormalizedAnchorPoint(fence, new Point(0.5, 0.5));
            map1.Children.Add(fence);*/

        }

        public async void displayleader(double para1, double para2)
        {
            i++;
            HttpClient httpClient = new HttpClient();
            string ResponseString = await httpClient.GetStringAsync(URI + "/getleaderlocation?gid="+gid+"&j="+i);

            if (ResponseString != "")
            {
                JObject j = JsonConvert.DeserializeObject<JObject>(ResponseString);
                JArray ja = (JArray)j["data"];
                //Debug.WriteLine(ja.Count);

                foreach (JObject jj in ja)
                {
                    leaderdata mm = new leaderdata()
                    {
                        accuracy = (double)jj["accuracy"],
                        uname = (string)jj["username"],
                        lat = (double)jj["lat"],
                        lon = (double)jj["lon"]
                    };
                    //Debug.WriteLine(mm.uname);

                    var jayway = new Geopoint(new BasicGeoposition() { Latitude = mm.lat, Longitude = mm.lon });

                    MapIcon mapIcon = new MapIcon();
                    mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/reddot.png"));
                    mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                    mapIcon.Location = jayway;
                    mapIcon.Title = mm.uname;
                    map1.MapElements.Add(mapIcon);

                    if (DistanceTo(para1, para2, jayway.Position.Latitude, jayway.Position.Longitude) > radius)
                    {
                        MessageDialog m = new MessageDialog("You have gone out of range");
                        await m.ShowAsync();
                    }
                }

            }

            else
            {
                MessageDialog m1 = new MessageDialog("NHP");
                await m1.ShowAsync();
            }

        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            HttpClient httpClient = new HttpClient();
            string response = await httpClient.GetStringAsync(URI + "/endmembersession?username=" + uname + "&j=" + j);

            this.Frame.Navigate(typeof(splash));
        }

        public double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }
    }
}
