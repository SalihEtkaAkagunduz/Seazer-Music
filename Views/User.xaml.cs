using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace seazermusic5;

public partial class User : ContentPage
{
    private IFirebaseClient? _client;
    private string? _userIdToken;

    public User()
    {
        InitializeComponent();
        InitializeFirebase();
        CheckUserAuthentication();
    }

    private void InitializeFirebase()
    {
        IFirebaseConfig fc = new FirebaseConfig
        {
            AuthSecret = "sy3XV4C4MoNK62SVdm2r9yWwxwl4DeYOfNfOxJdR",
            BasePath = "https://seazer-music-default-rtdb.firebaseio.com/"
        };
        try
        {
            _client = new FireSharp.FirebaseClient(fc);
        }
        catch (Exception)
        {
            Console.WriteLine("sunucuya ba�lan�lamad�");
        }
    }

    private async void CheckUserAuthentication()
    {
        // Eski verileri temizle
         

        // Kullan�c� oturum a�m�� m� kontrol et
        var userToken = await SecureStorage.GetAsync("user_token");
        if (!string.IsNullOrEmpty(userToken))
        {
            _userIdToken = userToken;
            ShowUserInfo();
        }
        else
        {
            ShowLoginForm();
        }
    }

    private void ShowLoginForm()
    {
        LoginFrame.IsVisible = true;
        UserInfoFrame.IsVisible = false;
    }

    private void ShowUserInfo()
    {
        LoginFrame.IsVisible = false;
        UserInfoFrame.IsVisible = true;

        // Kullan�c� bilgilerini g�ster
        var userEmail = SecureStorage.GetAsync("user_email").Result;
        var userName = SecureStorage.GetAsync("user_name").Result;

        UserEmailLabel.Text = $"E-posta: {userEmail}";
        UserNameLabel.Text = $"Kullan�c� Ad�: {userName}";
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Hata", "L�tfen e-posta ve �ifre girin.", "Tamam");
            return;
        }

        try
        {
            var authData = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(authData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyAImSleR3TQofh4U0xj9pZHJpaS84fN01k" +
                    "", content);
                var result = await response.Content.ReadAsStringAsync();
                var resultObj = JObject.Parse(result);

                if (resultObj["idToken"] != null)
                {
                    _userIdToken = resultObj["idToken"].ToString();
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(_userIdToken) as JwtSecurityToken;
                    var userId = jsonToken?.Claims.First(claim => claim.Type == "user_id").Value;
                    await SecureStorage.SetAsync("user_token", userId);
                    await SecureStorage.SetAsync("user_email", email);
                    await SecureStorage.SetAsync("user_name", "user123"); // Kullan�c� ad�n� buradan alabilirsiniz

                    // Kullan�c� verilerini Firebase Realtime Database'e kaydet
                    var userData = new
                    {
                        email = email,
                        userName = "user123" // Kullan�c� ad�n� buradan alabilirsiniz
                    };

                    var setResponse = await _client.SetAsync($"users/{resultObj["localId"]}", userData);

                    await DisplayAlert("Ba�ar�l�", "Giri� ba�ar�l�.", "Tamam");
                    ShowUserInfo();
                }
                else
                {
                    await DisplayAlert("Hata", "Giri� ba�ar�s�z.", "Tamam");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Giri� s�ras�nda bir hata olu�tu: {ex.Message}", "Tamam");
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Hata", "L�tfen e-posta ve �ifre girin.", "Tamam");
            return;
        }

        try
        {
            var authData = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(authData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyAImSleR3TQofh4U0xj9pZHJpaS84fN01k", content);
                var result = await response.Content.ReadAsStringAsync();
                var resultObj = JObject.Parse(result);

                if (resultObj["idToken"] != null)
                {
                    _userIdToken = resultObj["idToken"].ToString();
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(_userIdToken) as JwtSecurityToken;
                    var userId = jsonToken?.Claims.First(claim => claim.Type == "user_id").Value;
                    await SecureStorage.SetAsync("user_token", userId);
                    await SecureStorage.SetAsync("user_email", email);
                    await SecureStorage.SetAsync("user_name", "user123");  // Kullan�c� ad�n� buradan alabilirsiniz

                    // Kullan�c� verilerini Firebase Realtime Database'e kaydet
                    var userData = new
                    {
                        email = email,
                        userName = "user123" // Kullan�c� ad�n� buradan alabilirsiniz
                    };

                    var setResponse = await _client.SetAsync($"users/{resultObj["localId"]}", userData);

                    await DisplayAlert("Ba�ar�l�", "Kay�t ba�ar�l�.", "Tamam");
                    ShowUserInfo();
                }
                else
                {
                    await DisplayAlert("Hata", "Kay�t ba�ar�s�z.", "Tamam");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Kay�t s�ras�nda bir hata olu�tu: {ex.Message}", "Tamam");
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        SecureStorage.Remove("user_token");
        SecureStorage.Remove("user_email");
        SecureStorage.Remove("user_name");
    }
}
