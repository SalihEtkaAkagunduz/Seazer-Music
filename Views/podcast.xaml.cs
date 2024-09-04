using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using FirebaseMedium;
using Laerdal.FFmpeg;
using NAudio.Wave;
using VideoLibrary;
using static seazermusic5.podcastclass;

namespace seazermusic5;

public partial class podcast : ContentPage
{
    Crud crud; PodcastChannel podcastChannel;
    Listt dsdd;
    Dictionary<string, Song> ff; XDocument rssDoc; string rssUrl;
    public podcast( String re)
    {
        InitializeComponent(); crud = new Crud(); rssUrl = re; 
        podcastChannel = LoadPodcastChannel(rssUrl);
        if (podcastChannel != null)
        {LoadSongsAsync();

        }
       
         
    }
    private async Task LoadSongsAsync()
    {       var cc = await GetSongsAsync();
#if WINDOWS
  LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;SongsCollectionView.ItemsSource = cc;

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
#elif ANDROID
  LoadingIndicator1.IsRunning = true;
        LoadingIndicator1.IsVisible = true;SongsCollectionView1.ItemsSource = cc;

        LoadingIndicator1.IsRunning = false;
        LoadingIndicator1.IsVisible = false;
#endif
      

 
        
    }
    String gg = "";
    private async Task GetInfo()
    {
      
          gg = podcastChannel.Image ;
        // Burada RSS feed  
#if WINDOWS
imggg.Source = gg;
        lblll.Text = podcastChannel.Title;
        �gg.Text = podcastChannel.Description;
#elif ANDROID
imgg1.Source = gg;
add1.Text = podcastChannel.Title;
#endif

    }
    PodcastChannel channel;
        private PodcastChannel LoadPodcastChannel(string rssUrl)
    {
        try
        {
            XDocument rssDoc = XDocument.Load(rssUrl);
            var channelElement = rssDoc.Descendants("channel").FirstOrDefault();
            if (channelElement != null)
            {
                channel = new PodcastChannel
                {
                    RssUrl = rssUrl,
                    Title = channelElement.Element("title")?.Value,
                    Description = channelElement.Element("description")?.Value,
                    Link = channelElement.Element("link")?.Value,
                    Image = channelElement.Element("image").Element("url")?.Value
                };
                var itemElements = channelElement.Descendants("item").ToList();
                int itemCount = Math.Min(itemElements.Count, 21); // 21 veya daha az say�da ��e varsa, d�ng� say�s�n� ayarla
                for (int i = 0; i < itemCount; i++)
                {
                    var itemElement = itemElements[i];

                    string dateString = itemElement.Element("pubDate")?.Value;
                    DateTime dateTime;
                    string[] formats = { "ddd, dd MMM yyyy HH:mm:ss 'GMT'", "ddd, dd MMM yyyy HH:mm:ss +0000" };
                    if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    {
                        string formattedDate = dateTime.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);

                        var item = new PodcastItem
                        {
                            Title = itemElement.Element("title")?.Value,
                            Description = itemElement.Element("description")?.Value,
                            EnclosureUrl = itemElement.Element("enclosure").Attribute("url").Value,
                            Link = itemElement.Element("link")?.Value,
                            PubDate = formattedDate,
                            ChannelTitle = channelElement.Element("title")?.Value,
                            Image = channelElement.Element("image").Element("url")?.Value,
                        };
                        channel.Items.Add(item);
                    }
                    else
                    {
                        Debug.WriteLine($"Tarih format� ��z�mlenemedi: {dateString}");
                    }
                }

                return channel;
            }

         
        }
        catch (Exception ex)
        {
          
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Hata", "Bu Podcast�n RSS bilgisi veritaban�na hatal� girilmi� yada �lkemizde desteklenmiyor bu hata bir veri taban� hatas� olup uygulama hatas� de�ildir l�tfen ba�ka bir podcast se�in", "Tamam");
                await Navigation.PopAsync();
            });
            return null; // Hata durumunda metodu sonland�r
         
    }

        return null;
    }
     
    
    private async Task<List<PodcastItem>> GetSongsAsync()
    {
        gg =  podcastChannel.Image ;
        // Burada RSS feed  
#if WINDOWS
imggg.Source = gg;
        lblll.Text = podcastChannel.Title;
        derder.Text = podcastChannel.Link;
        �gg.Text = podcastChannel.Description;
#elif ANDROID
 imgg1.Source = gg;
        add1.Text = podcastChannel.Title;
         
        
#endif
        
        podcastChannel = LoadPodcastChannel(rssUrl);
        List<PodcastItem> df = podcastChannel.Items;
       
        
        return df;
    }

    List<Song> songs;
    private async void OnMoreOptionsClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var action = await DisplayActionSheet("Se�enekler", "�ptal", null, "�ndir", "Dosya Bilgileri", "Cihazdan Sil");
        if (action == "�ndir")
        {
            // await DownloadVideoAsync(songs[sender.ta]);
            var buttonm = (Button)sender;
            PodcastItem item = (PodcastItem)buttonm.BindingContext;

            videon = item.Title.Replace(' ', '-') + ".mp3";
            if (File.Exists(directoryPath + videon))
            {

                await DisplayAlert("Uyar�", "Bu m�zik zaten indirili", "Tamam");
            }
            else
            {
                await DownloadMp3Async(item.EnclosureUrl);

            }

        }
        else if (action == "Dosya Bilgileri")
        {
            var buttonm = (Button)sender;
            Song item = (Song)buttonm.BindingContext;

            // await DownloadVideoAsync(songs[sender.ta]);
            using (MediaFoundationReader mf = new MediaFoundationReader(item.audioStreamInfo))
            using (WasapiOut audioPlayer2 = new WasapiOut())
            {

                audioPlayer2.Init(mf);
                audioPlayer2.Play();



                while (audioPlayer2.PlaybackState == PlaybackState.Playing)
                {
                    await Task.Delay(1000);
                }

                // Timer'� durdur

            }



        }

        else if (action == "Cihazdan Sil")
        {
            // await DownloadVideoAsync(songs[sender.ta]);
            var buttonm = (Button)sender;
            Song item = (Song)buttonm.BindingContext;
            var youtube = YouTube.Default;
            var video = youtube.GetVideo(item.YouTubeLink);
            videon = video.Title.Replace(' ', '-') + ".mp3";
            if (File.Exists(directoryPath + videon))
            {



                File.Delete(directoryPath + videon); await DisplayAlert("Uyar�", "Cihazdan silme i�lemi ba�ar�l�", "Tamam");
            }
            else
            {
                await DisplayAlert("Uyar�", "Bu m�zik zaten indirili de�il", "Tamam");
            }



        }

    }
    string directoryPath = "C:\\ProgramData\\Seazer Software\\Seazer Music\\";

    string videon;
    private async Task DownloadMp3Async(string mp3Url)
    {
        try
        {
            var client = new HttpClient();
            long? totalByte = 0;

            string filePath;
            var platform = Microsoft.Maui.Devices.DeviceInfo.Platform; // Platform bilgisini g�ncellenmi� API ile al�n
            string fileName = Path.GetFileName(mp3Url);
            if (platform == Microsoft.Maui.Devices.DevicePlatform.Android)
            {
                // Android i�in dosya yolu
                filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);
            }
            else
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                filePath = Path.Combine(directoryPath, fileName);
            }

            using (var output = File.OpenWrite(filePath))
            {
                using (var request = new HttpRequestMessage(HttpMethod.Head, mp3Url))
                {
                    var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    totalByte = response.Content.Headers.ContentLength;
                }

                using (var input = await client.GetStreamAsync(mp3Url))
                {
                    byte[] buffer = new byte[16 * 1024]; // 16KB buffer
                    int read;
                    long totalRead = 0;
                    while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                        totalRead += read;
                        var progress = totalRead * 100 / totalByte;
                        // UI g�ncellemeleri i�in Dispatcher kullan�n
                        Dispatcher.Dispatch(() => Debug.WriteLine($"Downloading {progress}% ..."));
                    }
                    Dispatcher.Dispatch(() => Debug.WriteLine("Download Complete"));
                }
            }
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(() => Debug.WriteLine($"Download error: {ex.Message}"));
        }
    }
    private int FindSongIndex(Song songToFind, List<Song> songList)
    {
        for (int i = 0; i < songList.Count; i++)
        {
            if (songList[i].Title == songToFind.Title && songList[i].Tag == songToFind.Tag)
            {
                return i;
            }
        }
        return -1; // E�le�me bulunamad�
    }
    private  void OnSelectionChanged1(object sender, SelectionChangedEventArgs e)
    {
        
        


    }private void AddButton(object sender, EventArgs e)
    {
        Listt2 bb = new Listt2();
        bb.Rss = rssUrl;
        bb.ImageUrl = channel.Image;
        bb.Name = channel.Title;
        crud.AddPodcast(bb);
#if WINDOWS
 addbutton.Text = "Takipten ��k";
#elif ANDROID
 addbutton1.Text = "Takipten ��k";
#endif
       
    }
    private void Button_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Uyar�", "L�tfen bu men�y� a�mak i�in ssa� t�k kullan�n", "Tamam");
         
        
 
    }
    private async Task ConvertVideoToAudioAsync(string filePath)
    {
        try
        {
            string a = filePath.Substring(0, filePath.Length - 4) + ".mp4";
            string b = filePath.Substring(0, filePath.Length - 4) + ".mp3";
            int status = await Task.Run(() => FFmpeg.Execute($"-i {a} {b}"));
            if (status == 0)
            {
                Dispatcher.Dispatch(() => Debug.WriteLine("Success"));
            }
            else
            {
                Dispatcher.Dispatch(() => Debug.WriteLine($"FFmpeg failed with status code {status}"));
            }
        }
        catch (Exception e)
        {
            Dispatcher.Dispatch(() => Debug.WriteLine(e.Message.ToString()));
        }
    }
}