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
namespace seazermusic5;

public partial class sarkibul : ContentPage
{
    Crud ccc;
    connection cc;
    private readonly YoutubeClient youtubeClient;
    public ObservableCollection<VideoItem> VideoItems { get; set; } = new ObservableCollection<VideoItem>();	public sarkibul()
	{
		InitializeComponent(); cc = new connection(); ccc = new Crud();
        youtubeClient = new YoutubeClient();
#if WINDOWS
 songsCollectionView.ItemsSource = VideoItems;
#elif ANDROID
 songsListView1.ItemsSource = VideoItems;
#endif

    }
    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue)) return;

        var videos = await youtubeClient.Search.GetVideosAsync(e.NewTextValue).Take(10).ToListAsync();

        VideoItems.Clear();
        var tasks = videos.Select(async video =>
        {
            // Thumbnail URL'sini almak i�in bir y�ntem se�in
            var thumbnailUrl = video.Thumbnails.OrderByDescending(t => t.Resolution.Area).FirstOrDefault()?.Url;

            // Video URL'sini olu�turun
            var videoUrl = $"https://www.youtube.com/watch?v={video.Id}";

            return new VideoItem
            {
                Title = video.Title,
                Author = video.Author.ChannelTitle,
                Thumbnail = thumbnailUrl, // Burada se�ilen thumbnail URL'sini kullan�n
                Url = videoUrl // Video URL'sini burada ayarlay�n
            };
        });

        var videoItems = await Task.WhenAll(tasks);
        foreach (var item in videoItems)
        {
             VideoItems.Add(item);
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
        s.Title= video.Title;
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
        var aad =  videor.Thumbnails.GetWithHighestResolution()?.Url;
        s.ImageUrl = aad;
        s.Length = video.Info.LengthSeconds.ToString();
        s.Single = "Single";
        
        if (await IsTitleExists(s.Title))
        {
            DisplayAlert("Uyar�","Bu video zaten listenize kay�tl�", "Tamam");
        }
        else
        {
            // E�er Title yoksa, veriyi ekleyin
            DisplayAlert("��lem tamamland�", "�ark� listenize kaydedildi", "Tamam");
            ccc.SetData(s);
        }
         
        MessagingCenter.Send<sarkibul, string>(this, "strm", videoUrl);
        // URL'yi kullanarak istedi�iniz i�lemi yap�n
        // �rne�in, bir mesaj g�sterin veya videoyu oynat�n


        // �ste�e ba�l� olarak, MessagingCenter arac�l���yla URL'yi g�nderin


    }
    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedPodcast = e.CurrentSelection.FirstOrDefault() as VideoItem;
        if (selectedPodcast == null) return;

        var rssUrl = selectedPodcast.Url;

        // podcast.xaml.cs dosyas�n� a�
        await Navigation.PushAsync(new podcast(rssUrl));
    }

    private void OnDownloadClicked(object sender, EventArgs e)
    {
          DisplayAlert("Alarm", "Bu bir alarm mesaj�d�r!", "Tamam");

    }

}

public class VideoItem
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Thumbnail { get; set; }
    public string Url { get; set; } // YouTube video URL'si
}

//using System.Collections.ObjectModel;
//using System.Linq;
//using YoutubeExplode;
//using YoutubeExplode.Videos.Streams;

//public partial class sarkibul : ContentPage
//{
//    private readonly YoutubeClient youtubeClient;
//    public ObservableCollection<VideoItem> VideoItems { get; set; } = new ObservableCollection<VideoItem>();

//    public sarkibul()
//    {
//        InitializeComponent();
//        youtubeClient = new YoutubeClient();
//        songsCollectionView.ItemsSource = VideoItems;
//    }

//    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
//    {
//        if (string.IsNullOrWhiteSpace(e.NewTextValue)) return;

//        var videos = await youtubeClient.Search.GetVideosAsync(e.NewTextValue).Take(10).ToListAsync();

//        VideoItems.Clear();
//        foreach (var video in videos)
//        {
//            // Thumbnail URL'sini almak i�in bir y�ntem se�in
//            var thumbnailUrl = video.Thumbnails.OrderByDescending(t => t.Resolution.Area).FirstOrDefault()?.Url;

//            VideoItems.Add(new VideoItem
//            {
//                Title = video.Title,
//                Author = video.Author.ChannelTitle,
//                Thumbnail = thumbnailUrl // Burada se�ilen thumbnail URL'sini kullan�n
//            });
//        }
//    }


//    private void OnPlayClicked(object sender, EventArgs e)
//    {
//        // Oynatma i�levselli�i burada
//    }

//    private void OnDownloadClicked(object sender, EventArgs e)
//    {
//        // �ndirme i�levselli�i burada
//    }
//}

//public class VideoItem
//{
//    public string Title { get; set; }
//    public string Author { get; set; }
//    public string Thumbnail { get; set; }
//}