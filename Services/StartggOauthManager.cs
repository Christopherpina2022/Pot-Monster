using Microsoft.Security.Authentication.OAuth;
using Microsoft.UI;
using WinRT.Interop;
using System.Windows.Interop;
using System.Windows;
using Windows.UI.WebUI;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Http;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public class StartGgOAuthService
    {
        private static readonly string ClientId = "543";
        private static readonly Uri AuthorizationEndpoint = new("https://start.gg/oauth/authorize");
        private static readonly Uri AuthorizationCallback = new("http://localhost:8000/oauth/callback");
        private static readonly Uri TokenUrl = new("https://api.start.gg/oauth/access_token");
        private readonly Window _owner;

        public StartGgOAuthService(Window owner)
        {
            _owner = owner;
        }

        private static string GenerateCodeVerifier()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);
            return Base64UrlEncode(bytes);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static string GenerateCodeChallenge(string codeVerifier)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
            return Base64UrlEncode(bytes);
        }

        public async Task AuthenticateAsync()
        {
            // Get the window handle
            IntPtr hwnd = new WindowInteropHelper(_owner).Handle;
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hwnd);

            // Generate the code challenge and verifier
            string codeVerifier = GenerateCodeVerifier();
            string codeChallenge = GenerateCodeChallenge(codeVerifier);

            // Setup Authentication parameters
            var authParams = AuthRequestParams.CreateForAuthorizationCodeRequest(ClientId, AuthorizationCallback);
            authParams.Scope = "user.identity";
            authParams.CodeChallenge = codeChallenge;
            authParams.CodeChallengeMethod = CodeChallengeMethodKind.S256;

            // Start the callback listener
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8000/oauth/callback/");
            listener.Start();

            // Send the request
            var authTask = OAuth2Manager.RequestAuthWithParamsAsync(windowId, AuthorizationEndpoint, authParams);
            HttpListenerContext context = await listener.GetContextAsync();
            Uri responseUri = context.Request.Url!;

            // Give the browser a response
            const string responseHtml = 
            """
            <html>
                <body>
                    <h2>Authorization successful.</h2>
                    <p>You can close this window and return to the application.</p>
                </body>
            </html>
            """;

            // Send response to callback
            byte[] buffer = Encoding.UTF8.GetBytes(responseHtml);
            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer);
            context.Response.Close();

            OAuth2Manager.CompleteAuthRequest(responseUri);

            AuthRequestResult authResult = await authTask;

            if (authResult.Response is AuthResponse authResponse)
            {
                using var client = new HttpClient();

                var parameters = new Dictionary<string, string>
                {
                    ["grant_type"] = "authorization_code",
                    ["code"] = authResponse.Code,
                    ["client_id"] = "543",
                    ["redirect_uri"] = AuthorizationCallback.ToString(),
                    ["code_verifier"] = codeVerifier,
                    ["scope"] = "user.identity"
                };

                using var content = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = await client.PostAsync(TokenUrl, content);

                string responseBody = await response.Content.ReadAsStringAsync();

                MessageBox.Show( $"Status: {(int)response.StatusCode} {response.StatusCode}\n\n" + responseBody);

                //TokenRequestParams tokenRequestParams = TokenRequestParams.CreateForAuthorizationCodeRequest(authResponse);
                //TokenRequestResult tokenResult = await OAuth2Manager.RequestTokenAsync(TokenUrl, tokenRequestParams);

                //if (tokenResult.Response is TokenResponse tokenResponse)
                //{
                 //   MessageBox.Show(
                //        $"Success!\n\nAccess Token:\n{tokenResponse.AccessToken}");
                //}
                //else
                //{
                //    TokenFailure failure = tokenResult.Failure;

                 //   MessageBox.Show(
                //        $"Token request failed:\n{failure.Error}\n{failure.ErrorDescription}");
                //}
            }
            else
            {
                AuthFailure failure = authResult.Failure;

                MessageBox.Show(
                    $"Authorization failed:\n{failure.Error}\n{failure.ErrorDescription}");
            }

            listener.Stop();
        }
    }
}
