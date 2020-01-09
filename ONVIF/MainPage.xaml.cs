using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using OnvifDiscovery;
using LibVLCSharp.Shared;
using LibVLCSharp.Forms.Shared;
using System.Diagnostics;

namespace ONVIF
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {       
        readonly LibVLC _libvlc;



        public MainPage()
        {
            InitializeComponent();

            Title = "ONVIF Live Cam";

            ToolbarItem itemSearch = new ToolbarItem() { Text = "Search" };
            ToolbarItems.Add(itemSearch);
            itemSearch.Clicked += OnSearchTap;

            // this will load the native libvlc library (if needed, depending on the platform). 
            Core.Initialize();

            // instanciate the main libvlc object
            _libvlc = new LibVLC();            
        }

        void OnSearchTap(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ListDevices());
        }

        
        protected override void OnAppearing()
        {
            base.OnAppearing();

            
            


            // create mediaplayer objects,
            // attach them to their respective VideoViews
            // create media objects and start playback

            VideoView.MediaPlayer = new MediaPlayer(_libvlc);
            //VideoView.MediaPlayer.Play(new Media(_libvlc, VIDEO_URL, FromType.FromLocation));

            CameraLabel.Text = "Camera Selected: " + (String.IsNullOrEmpty(Camera.IpAddress) ? "None" : Camera.Name);
        }


        void OnShowTap(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Camera.IpAddress)) { 
                DisplayAlert("Alert", " Select a camera first", "OK");
                return ;
            }
            string videoUrl = Camera.GetRTSPAddress(Login.Text, Pass.Text);
            Debug.WriteLine(videoUrl);
            if (VideoView.MediaPlayer.IsPlaying)
                VideoView.MediaPlayer.Stop();
            VideoView.MediaPlayer.Play(new Media(_libvlc, videoUrl, FromType.FromLocation));
        }

    }
}

