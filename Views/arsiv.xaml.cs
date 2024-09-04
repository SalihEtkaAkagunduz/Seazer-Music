using System.Collections.ObjectModel;
using FirebaseMedium;
using System.Threading.Tasks;

namespace seazermusic5;

public partial class arsiv : ContentPage
{
    public ObservableCollection<ArsivItem> ArsivItems
    {
        get; set;
    }
    public ObservableCollection<AlbumItem> RecentAlbums
    {
        get; set;
    }
    Crud crud;
    Dictionary<string, Song> ff;
    List<Song> songs;

    public arsiv()
    {
        InitializeComponent();
#if ANDROID
        Shell.SetNavBarIsVisible(this, false);
#endif
        crud = new Crud();
        LoadDataAsync();

        ArsivItems = new ObservableCollection<ArsivItem>
        {
            new ArsivItem { Icon = "bl11.png", Title = "Listeler" },
            new ArsivItem { Icon = "bl12.png", Title = "Sanat��lar" },
            new ArsivItem { Icon = "bl9.png", Title = "Alb�mler" },
            new ArsivItem { Icon = "bl8.png", Title = "Par�alar" },
            new ArsivItem { Icon = "bl7.png", Title = "Size �zel", EditText = "D�zenle", IsEditVisible = true },
            new ArsivItem { Icon = "bl1.png", Title = "�ndirilenler" },
            new ArsivItem { Icon = "bl3.png", Title = "TV ve Filmler" },
            new ArsivItem { Icon = "bl4.png", Title = "Video Listeleri" },
            new ArsivItem { Icon = "bl2.png", Title = "Podcast" },
        };

        // Recent Albums (Son Eklenenler)
        

        ArsivCollectionView.ItemsSource = ArsivItems;
        RecentAlbumsCollectionView.ItemsSource = RecentAlbums;
    }
    private async void OnArsivItemSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault();
        if (selectedItem != null)
        { String ass=((ArsivItem)selectedItem).Title;
            // Se�ilen ��e ile ilgili i�lemleri burada yapabilirsiniz
            if (ass=="Listeler") {

                var listeEkraniPage = new listeler();
                await Navigation.PushAsync(listeEkraniPage);
            }
            if (ass == "Sanat��lar")
            {

                DisplayAlert("Hata","Hen�z Tasar�m Ayarlanmad�", "Tamam");
                



            } if (ass=="Alb�mler") {

                DisplayAlert("Hata", "Hen�z Tasar�m Ayarlanmad�", "Tamam");
            }
            if (ass == "Par�alar")
            {

                var listeEkraniPage = new kay�tl�sarki();
                await Navigation.PushAsync(listeEkraniPage);
            }
            if (ass == "Size �zel")
            {

                DisplayAlert("Hata", "Hen�z Tasar�m Ayarlanmad�", "Tamam");
            }
            if (ass == "�ndirilenler")
            {

                var listeEkraniPage = new indirilenler();
                await Navigation.PushAsync(listeEkraniPage);
            }
            if (ass == "TV ve Filmler")
            {

                DisplayAlert("Hata", "Hen�z Tasar�m Ayarlanmad�", "Tamam");
            }
            if (ass == "Video Listesi")
            {

                DisplayAlert("Hata", "Hen�z Tasar�m Ayarlanmad�", "Tamam");
            }
            if (ass == "Podcast")
            {

                var listeEkraniPage = new podcastlist();
                await Navigation.PushAsync(listeEkraniPage);
            }
        }
    }
    private async Task LoadDataAsync()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        ff = await Task.Run(() => crud.LoadDataa());
        if (songs == null)
        {
            songs = new List<Song>();
        }
        songs = ff.Values.ToList();
        RecentAlbumsCollectionView.ItemsSource = songs;

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
    }

    public class ArsivItem
    {
        public string Icon
        {
            get; set;
        }
        public string Title
        {
            get; set;
        }
        public string EditText
        {
            get; set;
        }
        public bool IsEditVisible
        {
            get; set;
        }
    }

    public class AlbumItem
    {
        public string AlbumCover
        {
            get; set;
        }
        public string AlbumTitle
        {
            get; set;
        }
        public string ArtistName
        {
            get; set;
        }
    }
}
