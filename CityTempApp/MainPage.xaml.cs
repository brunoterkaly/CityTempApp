﻿using Newtonsoft.Json;
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

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8124");
            request.BeginGetResponse(MyCallBack, request);



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
                    List<CityTemp> cityTemp = (List<CityTemp>)serializer.Deserialize(reader, typeof(List<CityTemp>));
                    List<NameValueItem> items = new List<NameValueItem>();
                    string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                    for (int i = 11; i >= 0; i-- )
                    {
                       items.Add(new NameValueItem { Name = months[i], Value = cityTemp[0].Temperatures[i] });
                    }
                    
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.BarChart.Title = cityTemp[0].City + ", 2014";
                        ((BarSeries)this.BarChart.Series[0]).ItemsSource = items;
                    }); 


                }
                catch (WebException e)
                {
                    return;
                }
            }
        }

    }
    public class CityTemp
    {
        public string City { get; set;  }
        public List<double> Temperatures { get; set; }
    }
    public class NameValueItem
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }
}
