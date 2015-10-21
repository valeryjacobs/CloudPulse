using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace CloudPulse
{
   
    public class ViewModel1
    {
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private App viewModel;
        private IBandClient bandClient;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.DataContext = this.viewModel = App.Current;

            App.Current.MainWindow = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private async void stop_Click(object sender, RoutedEventArgs e)
        {
            await bandClient.SensorManager.HeartRate.StopReadingsAsync();
            this.viewModel.StatusMessage ="Stopped sensing.";
        }

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.viewModel.StatusMessage = "Starting...";

                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);

                if (pairedBands.Length < 1)
                {
                    this.viewModel.StatusMessage = "Connect your band and click the start button again.";
                    return;
                }

                bool heartRateConsentGranted;

                // Check whether the user has granted access to the HeartRate sensor.
                if (bandClient.SensorManager.HeartRate.GetCurrentUserConsent() == UserConsent.Granted)
                {
                    heartRateConsentGranted = true;
                }
                else
                {
                    heartRateConsentGranted = await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
                }

                if (!heartRateConsentGranted)
                {
                    this.viewModel.StatusMessage = "Access to the heart rate sensor is denied.";
                }
                else
                {
                    // Subscribe to HeartRate data.
                    bandClient.SensorManager.HeartRate.ReadingChanged += (s, args) => 
                    {
                        this.viewModel.StatusMessage = args.SensorReading.HeartRate.ToString();
                    };
                    await bandClient.SensorManager.HeartRate.StartReadingsAsync();
                }
            }
            catch (Exception ex)
            {
                this.viewModel.StatusMessage = ex.ToString();
            }
        }
    }
}
