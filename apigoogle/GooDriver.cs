using DotNetOpenAuth.OAuth2;
using DrEdit.Models;
using Google;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Text;

namespace apigoogle
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
        string f_get_userInfoOrCreateNewIfNotExist(OPEN_AUTH_CLIENT clientCredentials);
        string f_create_TokenNew(OPEN_AUTH_CLIENT clientCredentials);
        string f_get_retrieveAllFiles();
        string f_get_userInfo();

        string f_downloadFile(string file_id);
        string f_uploadFile(DriveFile file);
        string f_updateFile(DriveFile df_new);
    }

    public class GooDriver : IGooDriver
    {
        string _token = string.Empty;
        IAuthenticator _authenticator;
        DriveService _driveService;

        public string f_create_TokenNew(OPEN_AUTH_CLIENT clientCredentials)
        {
            IAuthenticator auth = null;
            DriveService service = null;

            try {
                auth = GetCredentials(clientCredentials);
                service = BuildService(auth);
            }
            catch (Exception e) { return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, Message = e.Message }); }

            if (auth == null || service == null)
                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, Message = "redirect user to authentication" });
            else
            {
                this._authenticator = auth;
                this._driveService = service;
                return this.f_get_userInfo();
            }
        }

        public string f_get_userInfoOrCreateNewIfNotExist(OPEN_AUTH_CLIENT clientCredentials)
        {
            if ((this._authenticator == null || this._driveService == null) && (clientCredentials == null || string.IsNullOrEmpty(clientCredentials.CODE_AUTH_CLIENT)))
                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, Message = "CODE_AUTH_CLIENT must be not null" });

            IAuthenticator auth = null;
            DriveService service = null;

            if (this._authenticator == null || this._driveService == null)
            {
                try
                {
                    auth = GetCredentials(clientCredentials);
                    service = BuildService(auth);
                }
                catch (Exception e) { return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, Message = e.Message }); }
            }
            else
                return f_get_userInfo();

            if (auth == null || service == null)
                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, Message = "Redirect user to authentication" });
            else
            {
                this._authenticator = auth;
                this._driveService = service;
                return this.f_get_userInfo();
            }
        }

        public string f_get_retrieveAllFiles()
        {
            if (this._authenticator == null || this._driveService == null)
                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, Message = "redirect user to authentication" });

            List<Google.Apis.Drive.v2.Data.File> result = new List<Google.Apis.Drive.v2.Data.File>();
            FilesResource.ListRequest request = this._driveService.Files.List();
            do
            {
                try
                {
                    FileList files = request.Fetch();

                    result.AddRange(files.Items);
                    request.PageToken = files.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            return JsonConvert.SerializeObject(new { Ok = true, Data = result });
        }

        public string f_get_userInfo()
        {
            if (this._authenticator == null || this._driveService == null)
                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, Message = "redirect user to authentication" });
            try
            {
                Userinfo userInfo = GetUserInfo(this._authenticator);
                Google.Apis.Drive.v2.Data.About about = this._driveService.About.Get().Fetch();

                return JsonConvert.SerializeObject(new
                {
                    Ok = true,
                    userInfo = new { tokenAccess = this._token, email = userInfo.Email, link = userInfo.Link, picture = userInfo.Picture },
                    quotaBytesTotal = about.QuotaBytesTotal,
                    quotaBytesUsed = about.QuotaBytesUsed
                });
            }
            catch (Exception e) { return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, Message = e.Message }); }
        }

        #region [ TOKEN ]
        
        /// <summary>
        /// Build a Drive service object.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials.</param>
        /// <returns>Drive service object.</returns>
        Google.Apis.Drive.v2.DriveService BuildService(IAuthenticator credentials)
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
        IAuthenticator GetCredentials(OPEN_AUTH_CLIENT clientCredentials)
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

        IAuthorizationState ExchangeCode(OPEN_AUTH_CLIENT clientCredentials)
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, clientCredentials.CLIENT_ID, clientCredentials.CLIENT_SECRET);
            IAuthorizationState state = new AuthorizationState();
            state.Callback = new Uri(clientCredentials.REDIRECT_URI);
            try
            {
                state = provider.ProcessUserAuthorization(clientCredentials.CODE_AUTH_CLIENT, state);
                this._token = state.AccessToken;
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
        IAuthenticator GetAuthenticatorFromState(IAuthorizationState credentials, OPEN_AUTH_CLIENT clientCredentials)
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
        Userinfo GetUserInfo(IAuthorizationState credentials, OPEN_AUTH_CLIENT clientCredentials)
        {
            return GetUserInfo(GetAuthenticatorFromState(credentials, clientCredentials));
        }

        /// <summary>
        /// Send a request to the UserInfo API to retrieve the user's information.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials to authorize the request.</param>
        /// <returns>User's information.</returns>
        /// <exception cref="NoUserIdException">An error occurred.</exception>
        Userinfo GetUserInfo(IAuthenticator credentials)
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
        IAuthorizationState GetStoredCredentials(String userId)
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
        void StoreCredentials(String userId, IAuthorizationState credentials)
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

        public string f_downloadFile(string file_id)
        {
            if (string.IsNullOrEmpty(file_id))
                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, Message = "FILE_ID must be not empty or null" });

            if (this._authenticator == null || this._driveService == null)
                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, Message = "redirect user to authentication" });

            Google.Apis.Drive.v2.Data.File file = this._driveService.Files.Get(file_id).Fetch();
            string downloadUrl = file.DownloadUrl;

            string data = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(downloadUrl));
                this._authenticator.ApplyAuthenticationToRequest(request);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                System.IO.Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                data = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new
                {
                    Ok = false,
                    Message = "An error occurred: " + e.Message,
                    File = new DriveFile(file, data)
                });
            }

            DriveFile df = new DriveFile(file, data);
            return JsonConvert.SerializeObject(new { Ok = true, File = df });
        }

        public string f_uploadFile(DriveFile df)
        {
            if (this._authenticator == null || this._driveService == null)
                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, File = df, Message = "redirect user to authentication" });

            try
            {
                // File's metadata.
                Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
                body.Title = df.title;
                body.Description = df.description;
                body.MimeType = df.mimeType;

                byte[] byteArray = Encoding.ASCII.GetBytes(df.content);
                MemoryStream stream = new MemoryStream(byteArray);

                FilesResource.InsertMediaUpload request = this._driveService.Files.Insert(body, stream, df.mimeType);
                request.Upload();
                Google.Apis.Drive.v2.Data.File file = request.ResponseBody;

                Permission newPermission = new Permission();
                newPermission.Type = "anyone";
                newPermission.Role = "reader";
                newPermission.Value = "";
                newPermission.WithLink = true;
                this._driveService.Permissions.Insert(newPermission, file.Id).Fetch();
                
                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, File = new DriveFile(file, df.content) });
            }
            catch(Exception e) {
                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, Message = e.Message, File = df });
            }
        }

        public string f_updateFile(DriveFile df_new)
        {
            if (this._authenticator == null || this._driveService == null)
                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, File = df_new, Message = "redirect user to authentication" });

            try
            {
                // First retrieve the file from the API.
                Google.Apis.Drive.v2.Data.File body = this._driveService.Files.Get(df_new.id).Fetch();

                body.Title = df_new.title;
                body.Description = df_new.description;
                body.MimeType = df_new.mimeType;

                //byte[] byteArray = Encoding.ASCII.GetBytes(content);
                byte[] byteArray = Encoding.UTF8.GetBytes(df_new.content);
                MemoryStream stream = new MemoryStream(byteArray);

                Google.Apis.Drive.v2.FilesResource.UpdateMediaUpload request = this._driveService.Files.Update(body, df_new.id, stream, df_new.mimeType);
                request.Upload();
                
                Permission newPermission = new Permission();
                newPermission.Type = "anyone";
                newPermission.Role = "reader";
                newPermission.Value = "";
                newPermission.WithLink = true;
                this._driveService.Permissions.Insert(newPermission, df_new.id).Fetch();

                Google.Apis.Drive.v2.Data.File file = request.ResponseBody;

                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, File = new DriveFile(file, df_new.content) });
            }
            catch(Exception e) {
                return JsonConvert.SerializeObject(new { Ok = false, tokenAccess = this._token, Message = e.Message, File = df_new });
            }
        }

    }
}
