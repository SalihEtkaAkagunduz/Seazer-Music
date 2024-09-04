using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Net.Http;
using System.Threading.Tasks;
using Laerdal;
using Laerdal.FFmpeg;
using Microsoft.Maui.Controls;
using VideoLibrary;
using Plugin;

using NReco.VideoConverter;
using System.Diagnostics;

using Plugin.Maui.Audio;
using FirebaseMedium;
 
using NAudio.Wave;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace seazermusic5;

public partial class listeekrani : ContentPage
{
    Crud crud;
    Listt dsdd;
    Dictionary<string, Song> ff;
    
 
    public listeekrani(Listt sdss)
    {
        InitializeComponent(); crud = new Crud();
        dsdd = sdss;
        songs = sdss.Songs.Values.ToList();
        MessagingCenter.Send<listeekrani, List<Song>>(this, "Songmessage", songs);
        LoadSongsAsync();
        
      
    }
    String aadf = "";
    public listeekrani(String aadf)
    {
        aadf = aadf;
         InitializeComponent();
          _ = InitializeAsync();

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
       
    }
    public async Task InitializeAsync()
    {
       await init();
    }
    public async Task init()
    { aadf="";
        crud = new Crud();
        List<Listt> allLists = crud.LoadAllListNames(aadf);
        dsdd = allLists[0];
        songs = dsdd.Songs.Values.ToList();
        MessagingCenter.Send<listeekrani, List<Song>>(this, "Songmessage", songs);
        LoadSongsAsync();
    }
        public async Task LoadSongsAsync()
    { 
        var songs = await GetSongsAsync();
#if ANDROID

        LoadingIndicator1.IsRunning = true;
        LoadingIndicator1.IsVisible = true;
        SongsCollectionView1.ItemsSource = songs;
        LoadingIndicator1.IsRunning = false;
        LoadingIndicator1.IsVisible = false;

#elif WINDOWS
 LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

      
        SongsCollectionView.ItemsSource = songs;

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
#endif




    }
    private async Task<List<Song2>> GetSongsAsync()
    {
        //String gg = await Resimindir.DownloadAndSaveImageAsync(dsdd.ImageUrl, $"{dsdd.Name.Replace(' ', '-')}.jpg");
        String gg =dsdd.ImageUrl;
#if ANDROID
 imgg1.Source= gg;add.Text = dsdd.Name;
#elif WINDOWS
  imgg.Source= gg;
        lbll.Text = dsdd.Name;
#endif
        
        int s�re=0;
        int ��e=0;
        foreach(Song sss in dsdd.Songs.Values)
        {
            bool containsColon = sss.Length.Contains(':');

            if (containsColon)
            { 
                string[] parts = sss.Length.Split(':');
               
                if (parts.Length == 2)
                {
                    s�re += Convert.ToInt32(parts[0]) * 60 + Convert.ToInt32(parts[1]);
                }
                if (parts.Length == 3)
                {
                    s�re += Convert.ToInt32(parts[0]) * 3600 + Convert.ToInt32(parts[1])*60 + Convert.ToInt32(parts[2]) ;
                }
                 
                ��e += 1;
            }
            else
            {
 s�re +=Convert.ToInt32( sss.Length);
            ��e += 1;
            }
               
	 	        
		 
	 
	   
           
        }
        double aar = s�re / 3600;
        int aaas =Convert.ToInt32( Math.Round(aar));

#if ANDROID
        derder1.Text  = $"{��e} ��e � {aaas} saat {Convert.ToInt32(Math.Round((double)(s�re % 3600) / 60))} dakika";
#elif WINDOWS
 derder.Text= �gg.Text = $"{��e} ��e � {aaas} saat {Convert.ToInt32(Math.Round((double)(s�re % 3600)/60)) } dakika";
#endif
        List<Song> songss = new List<Song>();
        string directoryPath = "C:\\ProgramData\\Seazer Software\\Seazer Music\\";
        for (int i = 0; i < songs.Count; i++)
        {
             
                songss.Add(songs[i]);
            //  songs[i].Tag =await Resimindir.DownloadAndSaveImageAsync(songs[i].ImageUrl, $"{songs[i].Title.Replace(' ', '-')}.jpg");
            songs[i].Tag =  songs[i].ImageUrl ;


        }
        List<Song2> df = new List<Song2>();
        for (int i = 0; i < songs.Count; i++)
        {
           
             Song2 gh;
            if (File.Exists(directoryPath + songs[i].Title.Replace(' ', '-') + ".mp3"))
            {
               gh= song1tosong2.convert(songss[i], true);
            }
            else
            {
               gh= song1tosong2.convert(songss[i], false);
            }

            df.Add(gh);
        }
        return df;
    }
   
    List<Song> songs;
    private async void OnMoreOptionsClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var action = await DisplayActionSheet("Se�enekler", "�ptal", null, "Dosya Bilgileri", "Cihazdan Sil");

        if (action == "Dosya Bilgileri")
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
    private async Task DownloadVideoAsync(string videoUrl)
    {

        try
        {
            var youtube = YouTube.Default;
            var video = youtube.GetVideo(videoUrl);
            videon = video.FullName.Replace(' ', '-');
            var client = new HttpClient();
            long? totalByte = 0;

            string filePath;
            var platform = Microsoft.Maui.Devices.DeviceInfo.Platform; // Platform bilgisini g�ncellenmi� API ile al�n
            if (platform == Microsoft.Maui.Devices.DevicePlatform.Android)
            {
                // Android i�in dosya yolu
                filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), videon);
            }
            else
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                filePath = Path.Combine(directoryPath, videon);
            }

            using (var output = File.OpenWrite(filePath))
            {
                using (var request = new HttpRequestMessage(HttpMethod.Head, video.Uri))
                {
                    var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    totalByte = response.Content.Headers.ContentLength;
                }

                using (var input = await client.GetStreamAsync(video.Uri))
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

            // Platform kontrol� ve dosya d�n��t�rme i�lemleri
            if (platform == Microsoft.Maui.Devices.DevicePlatform.Android)
            {
                // await ConvertVideoToAudioAsync(filePath); // await ekleyerek asenkron �a�r�y� bekleyin
            }
            else
            {
                var convertVideo = new NReco.VideoConverter.FFMpegConverter();
                convertVideo.ConvertMedia(filePath, filePath.Replace(".mp4", ".mp3"), "mp3");
            }

            File.Delete(filePath); // Orijinal video dosyas�n� d�n��t�rme sonras� silin
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
    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedSong = e.CurrentSelection.FirstOrDefault() as Song2;
        if (selectedSong != null)
        {
            Song ff = song1tosong2.Convert3(selectedSong);
            int selectedIndex = FindSongIndex(ff, songs); // �rnek bir int de�er
                      MessagingCenter.Send(this, "integerMessagxe", selectedIndex);


            // Se�ili �ark�n�n indeksini bul


            // Bir sonraki �ark�y� al
            Song nextSong = null;
            if (selectedIndex < songs.Count - 1) // Son �ark� de�ilse
            {
                nextSong = songs[selectedIndex + 1];
            }

            // Bir �nceki �ark�y� al
            Song previousSong = null;
            if (selectedIndex > 0) // �lk �ark� de�ilse
            {
                previousSong = songs[selectedIndex - 1];
            }

            // Bir sonraki ve bir �nceki �ark� ile ilgili i�lemler
            // �rne�in, bilgilerini yazd�rabilirsiniz
            if (nextSong != null)
            {
                Debug.WriteLine($"Bir sonraki �ark�: {nextSong.Title}");
            }
            if (previousSong != null)
            {
                Debug.WriteLine($"Bir �nceki �ark�: {previousSong.Title}");
            }
        }


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
    Listt agns;
    private void MenuFlyoutItem_Clicked(object sender, EventArgs e)
    {
        sdaa();
    }
    async Task sarkiekle(Song s)
    {
        crud.SetData(s);
        
    }
        async Task sdaa()
    {
        var youtubeS = new YoutubeClient();
        var updatedSongs = new Dictionary<string, Song>();

        foreach (var s in songs)
        {
            // Video bilgilerini al
            var videoS = await youtubeS.Videos.GetAsync(s.YouTubeLink);
            var streamManifest = await youtubeS.Videos.Streams.GetManifestAsync(videoS.Id);
            var audioStreamInfow = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            s.audioStreamInfo = audioStreamInfow.Url;

            // G�ncellenmi� �ark�y� yeni s�zl��e ekle
            updatedSongs[s.Title] = s;
            sarkiekle(s);
        }
        agns = dsdd;
        // G�ncellenmi� �ark�lar� Listt i�indeki Songs'a ata
        agns.Songs = updatedSongs;
        crud.AddList(agns);
    }
}