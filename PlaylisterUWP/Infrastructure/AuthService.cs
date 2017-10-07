namespace PlaylisterUWP.Infrastructure
{
	using System;
	using System.Linq;
	using System.Net.Http;
	using System.Text;
	using System.Threading.Tasks;
	using Windows.Data.Json;
	using Windows.Storage;
	using Windows.System;
	using MetroLog;
	using Models;

	public class AuthService
	{
		private const string GoogleAuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
		private const string GoogleAuthorizationTokenExchangeEndpoint = "https://www.googleapis.com/oauth2/v4/token";
		private const string YouTubeReadOnlyScope = "https://www.googleapis.com/auth/youtube.readonly";
		private const string YouTubeUploadScope = "https://www.googleapis.com/auth/youtube.upload";
		private const string YouTubeApiClientId = "1088464525171-lt5bh3ilrbq3ps2lgiicllahkspjmok1.apps.googleusercontent.com";
		private const string AuthorizationRedirectUri = "com.playlister.app:/";

		public static async void LaunchGoogleAuthUri()
		{
			var state = UtilityService.GenerateRandomDataBase64Url(32);
			var codeVerifier = UtilityService.GenerateRandomDataBase64Url(32);
			var codeChallenge = UtilityService.EncodeBase64UrlNoPadding(UtilityService.ConvertToSha256(codeVerifier));
			const string codeChallengeMethod = "S256";
			var localSettings = ApplicationData.Current.LocalSettings;
			localSettings.Values["state"] = state;
			localSettings.Values["codeVerifier"] = codeVerifier;
			var authorizationRequest =
				$"{GoogleAuthorizationEndpoint}?response_type=code" +
				$"&scope=openid%20profile%20{YouTubeReadOnlyScope}%20{YouTubeUploadScope}" +
				$"&redirect_uri={Uri.EscapeDataString(AuthorizationRedirectUri)}" +
				$"&client_id={YouTubeApiClientId}" +
				$"&state={state}" +
				$"&code_challenge={codeChallenge}" +
				$"&code_challenge_method={codeChallengeMethod}";
			await Launcher.LaunchUriAsync(new Uri(authorizationRequest));
		}

		public static async Task<bool> PerformCodeExchangeAsync(string code, string codeVerifier)
		{
			var tokenRequestBody =
				$"code={code}&redirect_uri={System.Uri.EscapeDataString(AuthorizationRedirectUri)}" +
				$"&client_id={YouTubeApiClientId}&code_verifier={codeVerifier}" +
				$"&scope=" +
				$"&grant_type=authorization_code";
			var content = new StringContent(tokenRequestBody, Encoding.UTF8, "application/x-www-form-urlencoded");
			var handler = new HttpClientHandler {AllowAutoRedirect = true};
			var client = new HttpClient(handler);
			var response = await client.PostAsync(GoogleAuthorizationTokenExchangeEndpoint, content);
			var responseString = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				return false;
			}
			var tokens = JsonObject.Parse(responseString);
			var accessToken = tokens.GetNamedString("access_token");
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
			return true;
		}

		public static GoogleAuthorizationResponse ParseAuthorizationResponse(Uri uri, ILogger log)
		{
			var authorizationResponse = uri;
			var queryString = authorizationResponse.Query;
			var queryStringParams = queryString.Substring(1).Split('&')
				.ToDictionary(c => c.Split('=')[0], c => Uri.UnescapeDataString(c.Split('=')[1]));
			if (queryStringParams.ContainsKey("error"))
			{
				log.Error($"Error parsing authorization response: {queryStringParams["error"]}");
				return null;
			}
			if (!queryStringParams.ContainsKey("code") || !queryStringParams.ContainsKey("state"))
			{
				log.Error("Malformed authorization response. " + queryString);
				return null;
			}
			var code = queryStringParams["code"];
			var incoming_state = queryStringParams["state"];
			return new GoogleAuthorizationResponse
			{
				Code = code,
				State = incoming_state
			};
		}
	}
}
