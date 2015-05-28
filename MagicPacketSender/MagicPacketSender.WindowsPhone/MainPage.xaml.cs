using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MagicPacketSender
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        
        private ObservableCollection<RequestInfo> recentRequests = new ObservableCollection<RequestInfo>();
        public  ObservableCollection<RequestInfo> RecentRequests
        {
            get { return recentRequests; }
            set
            {
                recentRequests = value;
                OnPropertyChanged("RecentRequests");
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
           if (RecentRequests.Count == 0)
           {
               var requests = await FileUtils.GetRequestInfo();
               if (requests != null)
               {
                   foreach (var request in requests)
                   {
                       RecentRequests.Add(request);
                   }
               }
           }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            HostName targetHost = new HostName(HostnameBox.Text);
            uint port = Convert.ToUInt32(PortNumberBox.Text);
            string macAddress = MacAddressBox.Text;            

            MagicPacketSender magicSender = new MagicPacketSender();            
            await magicSender.SendMagicPacket(targetHost, port, macAddress);

            RequestInfo info = new RequestInfo
            {
                RequestHostName = HostnameBox.Text,
                MacAddress = MacAddressBox.Text,
                Port = Convert.ToUInt32(PortNumberBox.Text)
            };
            RecentRequests.Add(info);
            await FileUtils.SaveRequestInfo(RecentRequests.ToList());
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RequestInfo info = e.AddedItems[0] as RequestInfo;
            if(info != null)
            {
                HostnameBox.Text = info.RequestHostName;
                PortNumberBox.Text = info.Port.ToString();
                MacAddressBox.Text = info.MacAddress;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
