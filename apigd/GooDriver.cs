using DotNetOpenAuth.OAuth2;
using DrEdit.Models;
using Google;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Drive.v2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace apigd
{

    public class OPEN_AUTH_CLIENT
    {
        public string CLIENT_ID { set; get; }
        public string CLIENT_SECRET { set; get; }
        public string[] SCOPES { set; get; }
        public string REDIRECT_URI { set; get; }
        public string CODE_AUTH_CLIENT { set; get; }
    }

    public interface IGooDriver
    {
        IAuthenticator f_get_Authenticator(IAuthenticator aut);
        DriveService f_get_DriveService(DriveService sv);
        string f_get_userInfoOrCreateNewIfNotExist(OPEN_AUTH_CLIENT clientCredentials);
        string f_create_TokenNew(OPEN_AUTH_CLIENT clientCredentials);
    }

    public class GooDriver : IGooDriver
    {
        IAuthenticator _authenticator;
        DriveService _driveService;

        public string f_create_TokenNew(OPEN_AUTH_CLIENT clientCredentials)
        {
            string result = string.Empty;
            IAuthenticator auth = null;
            DriveService service = null;

            auth = GetCredentials(clientCredentials);
            service = BuildService(auth);

            if (auth == null || service == null)
                result = JsonConvert.SerializeObject(new { Ok = false, Message = "redirect user to authentication" });
            else
            {
                this._authenticator = auth;
                this._driveService = service;
                result = this.f_get_userInfo();
            }

            return result;
        }

        public string f_get_userInfoOrCreateNewIfNotExist(OPEN_AUTH_CLIENT clientCredentials)
        {
            if(clientCredentials == null || string.IsNullOrEmpty(clientCredentials.CODE_AUTH_CLIENT))
                return JsonConvert.SerializeObject(new { Ok = false, Message = "CODE_AUTH_CLIENT must be not null" });

            IAuthenticator auth = null;
            DriveService service = null;


            if (this._authenticator == null || this._driveService == null)
            {
                auth = GetCredentials(clientCredentials);
                service = BuildService(auth);
            }
            else
                return f_get_userInfo();

            if (auth == null || service == null)
                return JsonConvert.SerializeObject(new { Ok = false, Message = "Redirect user to authentication" });
            else
            {
                this._authenticator = auth;
                this._driveService = service;
                return this.f_get_userInfo();
            }
        }

        public IAuthenticator f_get_Authenticator(IAuthenticator aut)
        {
            return this._authenticator;
        }

        public DriveService f_get_DriveService(DriveService sv)
        {
            return this._driveService;
        }

        string f_get_userInfo()
        {
            string result = string.Empty;

            if (this._authenticator == null || this._driveService == null)
                return JsonConvert.SerializeObject(new { Ok = false, Message = "redirect user to authentication" });

            Userinfo userInfo = GetUserInfo(this._authenticator);
            Google.Apis.Drive.v2.Data.About about = this._driveService.About.Get().Fetch();

            result = JsonConvert.SerializeObject(new
            {
                Ok = true,
                userInfo = new { email = userInfo.Email, link = userInfo.Link, picture = userInfo.Picture },
                quotaBytesTotal = about.QuotaBytesTotal,
                quotaBytesUsed = about.QuotaBytesUsed
            });

            return result;
        }

        #region

        /// <summary>
        /// Build a Drive service object.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials.</param>
        /// <returns>Drive service object.</returns>
        static Google.Apis.Drive.v2.DriveService BuildService(IAuthenticator credentials)
        {
            return new Google.Apis.Drive.v2.DriveService(credentials);
        }

        /// <summary>
        /// Retrieve credentials using the provided authorization code.
        ///
        /// This function exchanges the authorization code for an access token and
        /// queries the UserInfo API to retrieve the user's e-mail address. If a
        /// refresh token has been retrieved along with an access token, it is stored
        /// in the application database using the user's e-mail address as key. If no
        /// refresh token has been retrieved, the function checks in the application
        /// database for one and returns it if found or throws a NoRefreshTokenException
        /// with the authorization URL to redirect the user to.
        /// </summary>
        /// <param name="authorizationCode">Authorization code to use to retrieve an access token.</param>
        /// <param name="state">State to set to the authorization URL in case of error.</param>
        /// <returns>OAuth 2.0 credentials instance containing an access and refresh token.</returns>
        /// <exception cref="CodeExchangeException">
        /// An error occurred while exchanging the authorization code.
        /// </exception>
        /// <exception cref="NoRefreshTokenException">
        /// No refresh token could be retrieved from the available sources.
        /// </exception>
        static IAuthenticator GetCredentials(OPEN_AUTH_CLIENT clientCredentials)
        {
            String emailAddress = "";
            try
            {
                IAuthorizationState credentials = ExchangeCode(clientCredentials);
                Userinfo userInfo = GetUserInfo(credentials, clientCredentials);
                String userId = userInfo.Id;
                emailAddress = userInfo.Email;
                ////if (!String.IsNullOrEmpty(credentials.RefreshToken))
                ////{
                ////    StoreCredentials(userId, credentials);
                ////    return GetAuthenticatorFromState(credentials, clientCredentials);
                ////}
                ////else
                ////{
                ////    credentials = GetStoredCredentials(userId);
                ////    if (credentials != null && !String.IsNullOrEmpty(credentials.RefreshToken))
                ////    {
                ////        return GetAuthenticatorFromState(credentials, clientCredentials);
                ////    }
                ////}

                IAuthenticator au = GetAuthenticatorFromState(credentials, clientCredentials);
                return au;
            }
            catch { }

            //catch (CodeExchangeException e)
            //{
            //    Console.WriteLine("An error occurred during code exchange. " + e.Message);
            //    ////// Drive apps should try to retrieve the user and credentials for the current
            //    ////// session.
            //    ////// If none is available, redirect the user to the authorization URL.
            //    ////e.AuthorizationUrl = GetAuthorizationUrl(emailAddress, state);
            //    ////throw e;
            //}
            //catch (NoUserIdException eu)
            //{
            //    Console.WriteLine("No user ID could be retrieved. " + eu.Message);
            //}
            ////////// No refresh token has been retrieved.
            ////////String authorizationUrl = GetAuthorizationUrl(emailAddress, state);
            ////////throw new NoRefreshTokenException(authorizationUrl);

            return null;
        }

        static IAuthorizationState ExchangeCode(OPEN_AUTH_CLIENT clientCredentials)
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, clientCredentials.CLIENT_ID, clientCredentials.CLIENT_SECRET);
            IAuthorizationState state = new AuthorizationState();
            state.Callback = new Uri(clientCredentials.REDIRECT_URI);
            try
            {
                state = provider.ProcessUserAuthorization(clientCredentials.CODE_AUTH_CLIENT, state);
                return state;
            }
            catch (ProtocolException)
            {
                throw new CodeExchangeException(null);
            }
        }




        /// <summary>
        /// Retrieve an IAuthenticator instance using the provided state.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials to use.</param>
        /// <returns>Authenticator using the provided OAuth 2.0 credentials</returns>
        static IAuthenticator GetAuthenticatorFromState(IAuthorizationState credentials, OPEN_AUTH_CLIENT clientCredentials)
        {
            var provider = new StoredStateClient(GoogleAuthenticationServer.Description, clientCredentials.CLIENT_ID, clientCredentials.CLIENT_SECRET, credentials);
            var auth = new OAuth2Authenticator<StoredStateClient>(provider, StoredStateClient.GetState);
            auth.LoadAccessToken();
            return auth;
        }

        /// <summary>
        /// Send a request to the UserInfo API to retrieve the user's information.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials to authorize the request.</param>
        /// <returns>User's information.</returns>
        /// <exception cref="NoUserIdException">An error occurred.</exception>
        static Userinfo GetUserInfo(IAuthorizationState credentials, OPEN_AUTH_CLIENT clientCredentials)
        {
            return GetUserInfo(GetAuthenticatorFromState(credentials, clientCredentials));
        }

        /// <summary>
        /// Send a request to the UserInfo API to retrieve the user's information.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials to authorize the request.</param>
        /// <returns>User's information.</returns>
        /// <exception cref="NoUserIdException">An error occurred.</exception>
        static Userinfo GetUserInfo(IAuthenticator credentials)
        {
            Oauth2Service userInfoService = new Oauth2Service(credentials);
            Userinfo userInfo = null;
            try
            {
                userInfo = userInfoService.Userinfo.Get().Fetch();
            }
            catch (GoogleApiRequestException e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            if (userInfo != null && !String.IsNullOrEmpty(userInfo.Id))
            {
                return userInfo;
            }
            else
            {
                throw new NoUserIdException();
            }
        }



        /// <summary>
        /// Retrieved stored credentials for the provided user ID.
        /// </summary>
        /// <param name="userId">User's ID.</param>
        /// <returns>Stored GoogleAccessProtectedResource if found, null otherwise.</returns>
        static IAuthorizationState GetStoredCredentials(String userId)
        {
            if (!System.IO.File.Exists("AuthorizationState.json")) return null;

            string json = System.IO.File.ReadAllText("AuthorizationState.json");
            AuthorizationState o = JsonConvert.DeserializeObject<AuthorizationState>(json);
            return o;

            //StoredCredentialsDBContext db = new StoredCredentialsDBContext();
            //StoredCredentials sc = db.StoredCredentialSet.FirstOrDefault(x => x.UserId == userId);
            //if (sc != null)
            //{
            //    return new AuthorizationState() { AccessToken = sc.AccessToken, RefreshToken = sc.RefreshToken };
            //}
            //return null;
        }

        /// <summary>
        /// Store OAuth 2.0 credentials in the application's database.
        /// </summary>
        /// <param name="userId">User's ID.</param>
        /// <param name="credentials">The OAuth 2.0 credentials to store.</param>
        static void StoreCredentials(String userId, IAuthorizationState credentials)
        {
            var o = new StoredCredentials { UserId = userId, AccessToken = credentials.AccessToken, RefreshToken = credentials.RefreshToken };
            string json = JsonConvert.SerializeObject(o);
            System.IO.File.WriteAllText("AuthorizationState.json", json);

            //StoredCredentialsDBContext db = new StoredCredentialsDBContext();
            //StoredCredentials sc = db.StoredCredentialSet.FirstOrDefault(x => x.UserId == userId);
            //if (sc != null)
            //{
            //    sc.AccessToken = credentials.AccessToken;
            //    sc.RefreshToken = credentials.RefreshToken;
            //}
            //else
            //{
            //    db.StoredCredentialSet.Add(new StoredCredentials { UserId = userId, AccessToken = credentials.AccessToken, RefreshToken = credentials.RefreshToken });
            //}
            //db.SaveChanges();
        }



        #endregion

    }
}
