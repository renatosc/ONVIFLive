using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using OnvifDiscovery;
using System.Collections.ObjectModel;

namespace ONVIF
{
    public class ListDevices : ContentPage
    {

        private ListView lstView;
        private ObservableCollection<Device> items = new ObservableCollection<Device>();
        ActivityIndicator loadingIndicator;

        public ListDevices()
        {
            this.Title = "ONVIF Discovery";

            ToolbarItem itemRefresh = new ToolbarItem() { Text = "Refresh" };
            ToolbarItems.Add(itemRefresh);
            itemRefresh.Clicked += OnRefreshTap;

            lstView = new ListView();
            lstView.ItemsSource = items;
            lstView.ItemTemplate = new DataTemplate(typeof(TextCell));
            lstView.ItemTemplate.SetBinding(TextCell.TextProperty, "Name");
            lstView.ItemTemplate.SetBinding(TextCell.DetailProperty, "IpAddress");
            
            lstView.ItemTapped += Handle_ItemTapped;
            lstView.IsVisible = false;
            

            var overlay = new AbsoluteLayout();
            var content = lstView;
            loadingIndicator = new ActivityIndicator();
            AbsoluteLayout.SetLayoutFlags(content, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(content, new Rectangle(0f, 0f, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            AbsoluteLayout.SetLayoutFlags(loadingIndicator, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(loadingIndicator, new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            overlay.Children.Add(content);
            overlay.Children.Add(loadingIndicator);

            Content = overlay;

            OnvifDiscovery();

        }

        public async Task OnvifDiscovery()
        {
            loadingIndicator.IsRunning = true;
            lstView.IsVisible = false;


            //Create a Discovery instance
            var onvifDiscovery = new Discovery();

            // Call the asynchronous method Discover with a timeout of 4 second
            var onvifDevices = await onvifDiscovery.Discover(30);

            foreach (var device in onvifDevices)
            {
                Console.WriteLine($"Device model {device.Model} from manufacturer {device.Mfr} has address {device.Address}");
                Console.Write($"Urls to device: ");
                foreach (var address in device.XAdresses)
                {
                    Console.Write($"{address}, ");
                }
                Console.WriteLine(device + "\n");
                items.Add(new Device { Manufacturer = device.Mfr, Model = device.Model, IpAddress = device.Address });
            }
            Console.WriteLine("ONVIF Discovery finished");

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                loadingIndicator.IsRunning = false;
                lstView.IsVisible = true;
            });            
        }

        void OnRefreshTap(object sender, EventArgs e)
        {
            _ = OnvifDiscovery();
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            Device deviceSelected = (Device)e.Item;
            Camera.Manufacturer = deviceSelected.Manufacturer;
            Camera.Model = deviceSelected.Model;
            Camera.IpAddress = deviceSelected.IpAddress;


            //Deselect Item
            ((ListView)sender).SelectedItem = null;

            Navigation.PopAsync();
        }
       

    }

    public class Device {
        public string Name {
            get {
                return string.Format("{0} - {1}", Manufacturer, Model);
            }
        }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string IpAddress { get; set; }

    }

}
