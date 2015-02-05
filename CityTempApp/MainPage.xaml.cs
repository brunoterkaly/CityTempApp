using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace CityTempApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Random _random = new Random();

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:47581/api/values");
            //while (true)
            //{
               //request.BeginGetResponse(MyCallBack, request);
               //Task.Delay(TimeSpan.FromSeconds(3));
                
            //}
            //while (true)
            //{
                //using (EventWaitHandle tmpEvent = new ManualResetEvent(false))
                //{
                    request.BeginGetResponse(MyCallBack, request);
                    //tmpEvent.WaitOne(TimeSpan.FromSeconds(3));
                //}
            //}



        }
        async void MyCallBack(IAsyncResult result)
        {

            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            if (request != null)
            {
                try
                {
                    WebResponse response = request.EndGetResponse(result);
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    JsonSerializer serializer = new JsonSerializer();
                    List<TimeTemp> cityTemp = (List<TimeTemp>)serializer.Deserialize(reader, typeof(List<TimeTemp>));
                    List<NameValueItem> items = new List<NameValueItem>();
                    for (int i = 0; i < cityTemp.Count; i++)
                    {
                       items.Add(new NameValueItem { Name = cityTemp[i].time.ToString(), 
                            Value = Convert.ToDouble(cityTemp[i].temperature) });
                    }
                    
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.BarChart.Title = "Temperatures";
                        ((BarSeries)this.BarChart.Series[0]).ItemsSource = null;
                        ((BarSeries)this.BarChart.Series[0]).ItemsSource = items;
                    }); 


                }
                catch (WebException e)
                {
                    return;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:47581/api/values");
            request.BeginGetResponse(MyCallBack, request);

        }

    }
    public class TimeTemp
    {
        public string time { get; set; }
        public string temperature { get; set; }
    }

    public class NameValueItem
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }
}
