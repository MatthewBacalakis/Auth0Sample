using Auth0.OidcClient;
using IdentityModel.OidcClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPSample
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string AccessToken { get; set; }
        private static HttpClient httpClient = new HttpClient();
        private const string itemsEndpoint = "https://auth0sampleapi.azurewebsites.net/api/items";

        readonly string[] _connectionNames = new string[]
        {
            "Username-Password-Authentication",
            "google-oauth2",
            "twitter",
            "facebook",
            "github",
            "windowslive"
        };

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            Auth0Client client = GetAuth0Client();

            var extraParameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(connectionNameAutoSuggestBox.Text))
                extraParameters.Add("connection", connectionNameAutoSuggestBox.Text);

            if (!string.IsNullOrEmpty(audienceTextBox.Text))
                extraParameters.Add("audience", audienceTextBox.Text);

            DisplayResult(await client.LoginAsync(extraParameters: extraParameters));
        }

        private void DisplayResult(LoginResult loginResult)
        {
            // Display error
            if (loginResult.IsError)
            {
                resultTextBox.Text = loginResult.Error;
                return;
            }

            AccessToken = loginResult.AccessToken;

            SaveRefreshToken(loginResult.RefreshToken);


            //Display result
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Tokens");
            sb.AppendLine("------");
            sb.AppendLine($"id_token: {loginResult.IdentityToken}");
            sb.AppendLine($"access_token: {loginResult.AccessToken}");
            sb.AppendLine($"refresh_token: {loginResult.RefreshToken}");
            sb.AppendLine();

            sb.AppendLine("Claims");
            sb.AppendLine("------");
            foreach (var claim in loginResult.User.Claims)
            {
                sb.AppendLine($"{claim.Type}: {claim.Value}");
            }

            resultTextBox.Text = sb.ToString();
        }

        private static Auth0Client GetAuth0Client()
        {
            string domain = "mattbtest.auth0.com";
            string clientId = "QiCcgvG2oMooLm5yKJYYpHzy8gdsIKrU";

            var client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = domain,
                ClientId = clientId,
                Scope = "openid offline_access"
            });
            return client;
        }

        private void SaveRefreshToken(string refreshToken)
        {
            PasswordVault vault = new PasswordVault();

            vault.Add(new PasswordCredential(
                            "My App", "testUser", refreshToken));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            connectionNameAutoSuggestBox.ItemsSource = _connectionNames;
        }

        private async void readButton_Click(object sender, RoutedEventArgs e)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var result = await httpClient.GetAsync(itemsEndpoint);
            resultTextBox.Text = await result.Content.ReadAsStringAsync();
        }

        private async void writeButton_Click(object sender, RoutedEventArgs e)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            using (HttpContent httpContent = new StringContent(string.Empty))
            {
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await httpClient.PutAsync(itemsEndpoint, httpContent);
                resultTextBox.Text = await result.Content.ReadAsStringAsync();
            }
        }

        private async void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var result = await httpClient.DeleteAsync(itemsEndpoint);
            resultTextBox.Text = await result.Content.ReadAsStringAsync();
        }

        private async void refreshToken_Click(object sender, RoutedEventArgs e)
        {
            resultTextBox.Text = "Refreshing access_token" + Environment.NewLine;

            PasswordVault vault = new PasswordVault();

            var storedCredential = vault.Retrieve("My App", "testUser");

            var client = GetAuth0Client();
            var newToken = await client.RefreshTokenAsync(storedCredential.Password);
            AccessToken = newToken.AccessToken;

            resultTextBox.Text += $"New access token received: {AccessToken}";
        }
    }
}
