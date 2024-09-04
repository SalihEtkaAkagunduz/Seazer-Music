using System.Collections.ObjectModel;
using YoutubeExplode;
using System.Collections.ObjectModel;
using System.Linq;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using VideoLibrary;
using YoutubeExplode.Videos;
using FirebaseMedium;
using FireSharp;
using FireSharp.Response;

using System.Diagnostics;
using YoutubeExplode.Common;
using iTunesPodcastFinder.Models;
using iTunesPodcastFinder;

namespace seazermusic5;

public partial class podcastbul : ContentPage
{
   
    connection cc; PodcastFinder finder = new PodcastFinder();
    private readonly YoutubeClient youtubeClient;
    public ObservableCollection<VideoItem> VideoItems { get; set; } = new ObservableCollection<VideoItem>();
    public podcastbul()
    {
        InitializeComponent(); cc = new connection(); 
        youtubeClient = new YoutubeClient();
#if WINDOWS
 songsCollectionView.ItemsSource = VideoItems;
#elif ANDROID
 songsListView1.ItemsSource = VideoItems;
#endif


    }
    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue)) return;
#if WINDOWS
loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;
#elif ANDROID
loadingIndicator1.IsRunning = true;
            loadingIndicator1.IsVisible = true;
#endif
            

            await Task.Run(async () =>
            {
                var results = await finder.SearchPodcastsAsync(e.NewTextValue, 10);

                Device.BeginInvokeOnMainThread(() =>
                {
                    VideoItems.Clear();
                    foreach (var podcast in results)
                    {
                        // Thumbnail URL'sini almak i�in bir y�ntem se�in
                        var thumbnailUrl = podcast.ArtWork; // Do�ru �zelli�i kullan�n
                        
                        // Podcast URL'sini olu�turun
                        var podcastUrl = podcast.FeedUrl;
                   
                        VideoItems.Add(new VideoItem
                        {
                            Title = podcast.Name,
                            Author = podcast.Editor,
                            Thumbnail = thumbnailUrl, // Burada se�ilen thumbnail URL'sini kullan�n
                            Url = podcastUrl // Podcast URL'sini burada ayarlay�n
                        });
                    }
 
 
                   
                });
            });
        }
        
        catch
        {

        }
    }
      
    public async Task<bool> IsTitleExists(string title)
    {
        // "songs" d���m� alt�nda "Title" �zelli�ine g�re sorgulama yap�n
        // Bu �rnekte, "songs" verilerinizi saklad���n�z d���m�n ad�d�r
        FirebaseResponse response = await cc.client.GetAsync("users/SalihDeneme/Lists/List1/Songs");
        if (response.Body != "null")
        {
            Dictionary<string, Song> songs = response.ResultAs<Dictionary<string, Song>>();
            foreach (var song in songs)
            {
                if (song.Value.Title == title)
                {
                    // E�er Title zaten varsa, true d�n
                    return true;
                }
            }
        }
        // E�er Title bulunamazsa, false d�n
        return false;
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedPodcast = e.CurrentSelection.FirstOrDefault() as VideoItem;
        if (selectedPodcast == null) return;

        var rssUrl = selectedPodcast.Url;

        // podcast.xaml.cs dosyas�n� a�
        await Navigation.PushAsync(new podcast(rssUrl));
    }
    private async void OnPlayClicked(object sender, EventArgs e)
    {
        // 'sender' nesnesini Button olarak kabul edin
        var button = sender as Button;
        if (button == null) return;

        // Button'un ba�l� oldu�u VideoItem nesnesini al�n
        var videoItem = button.BindingContext as VideoItem;
        if (videoItem == null) return;

        // VideoItem'dan YouTube video URL'sini al�n
        var videoUrl = videoItem.Url;
        Song s = new Song();
        var youtube = YouTube.Default;
        var video = youtube.GetVideo(videoUrl);
        s.Title = video.Title;
        s.Artist = video.Info.Author;



















        var youtubeS = new YoutubeClient();

        // Video bilgilerini al
        var videoS = await youtubeS.Videos.GetAsync(videoUrl);

        // Ses ak���n� al
        var streamManifest = await youtubeS.Videos.Streams.GetManifestAsync(videoS.Id);
        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        s.audioStreamInfo = audioStreamInfo.Url;

        s.YouTubeLink = videoUrl;
        var videor = await youtubeClient.Videos.GetAsync(videoUrl);
        var aad = videor.Thumbnails.GetWithHighestResolution()?.Url;
        s.ImageUrl = aad;
        s.Length = video.Info.LengthSeconds.ToString();
        s.Single = "Single";

        if (await IsTitleExists(s.Title))
        {
            DisplayAlert("Uyar�", "Bu video zaten listenize kay�tl�", "Tamam");
        }
        else
        {
            // E�er Title yoksa, veriyi ekleyin
           
        }

        MessagingCenter.Send<podcastbul, string>(this, "strm", videoUrl);
        // URL'yi kullanarak istedi�iniz i�lemi yap�n
        // �rne�in, bir mesaj g�sterin veya videoyu oynat�n


        // �ste�e ba�l� olarak, MessagingCenter arac�l���yla URL'yi g�nderin


    }

    private void OnDownloadClicked(object sender, EventArgs e)
    {
        DisplayAlert("Alarm", "Bu bir alarm mesaj�d�r!", "Tamam");

    }
} 