using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

using System.Diagnostics;
using System.Text.Json;
using System.Net.Http;

namespace spotprice2
{

    public delegate void DataChangedHandler();

    public class API
    {
        public class MyObj
        {
            public string? TimeStamp { get; set; }
            public string? TimeStampDay { get; set; }
            public string? TimeStampHour { get; set; }
            public float Value { get; set; }
            public string? PriceArea { get; set; }
            public string? Unit { get; set; }
        }

        //https://www.vattenfall.se/api/price/spot/pricearea/2022-12-02/2022-12-02/SN2
        private const string BASE_URL = "https://www.vattenfall.se/api/price/spot/pricearea/";
        private int timeInterval = 100;
        public List<MyObj>? data { get; private set; }
        private System.Windows.Threading.DispatcherTimer? updateTimer;

        private DateTime lastUpdate = DateTime.Now;

        private PowerArea _area;
        public PowerArea Area
        {
            get { return this._area; }
            set
            {
                if (value == _area) return;
                this._area = value;
                GetData();
            }
        }

        public event DataChangedHandler? DataChanged;
        private void OnDataChange() => DataChanged?.Invoke();
        private async void GetData()
        {
            //CODE
            Debug.WriteLine("Data fetched");
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string tomorrow = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            string url = String.Format("{0}/{1}/{2}/{3}", BASE_URL, date, tomorrow, _area.ToString());
            Debug.WriteLine(url);

            using (var httpClient = new HttpClient())
            {
                // Always catch network exceptions for async methods
                try
                {
                    string result = await httpClient.GetStringAsync(url);

                    data = JsonSerializer.Deserialize<List<MyObj>>(result);

                    lastUpdate = DateTime.Now;
                    OnDataChange();
                }
                catch (Exception ex)
                {
                    // Details in ex.Message and ex.HResult.
                    Debug.WriteLine(ex);
                }
            }

        }
        private void SetTimer(int timeInMiliSeconds)
        {
            updateTimer = new System.Windows.Threading.DispatcherTimer();

            updateTimer.Tick += OnTimedEvent;
            updateTimer.Interval = TimeSpan.FromMilliseconds(timeInMiliSeconds);
            updateTimer.Start();
        }
        private void OnTimedEvent(object? sender, EventArgs e)
        {
            DateTime today = DateTime.Now;
            if (today.Hour == lastUpdate.Hour && today.Day == lastUpdate.Day) return;

            GetData();
            //OnDataChange();

        }

        public API() : this(PowerArea.SN1) { }
        public API(PowerArea area)
        {
            _area = area;
            GetData();
            SetTimer(timeInterval);
        }


        public API(PowerArea area, int timeIntervalInMilliseconds) : this(area)
        {
            timeInterval = timeIntervalInMilliseconds;
        }
        ~API()
        {
            updateTimer?.Stop();
        }
    }
}
