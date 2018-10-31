/*
 * Copyright (c) 2012 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
 * in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License
 * is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 * or implied. See the License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using Google.Apis.Requests;
using System.Text;
using DotNetOpenAuth.Messaging;
using Google.Apis.Drive.v2.Data;
using Newtonsoft.Json;

namespace DrEdit.Models
{
    public class Utils
    {
        /// <summary>
        /// Build a Drive service object.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials.</param>
        /// <returns>Drive service object.</returns>
        internal static Google.Apis.Drive.v2.DriveService BuildService(IAuthenticator credentials)
        {
            return new Google.Apis.Drive.v2.DriveService(credentials);
        }

        /// <summary>
        /// Retrieve an IAuthenticator instance using the provided state.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials to use.</param>
        /// <returns>Authenticator using the provided OAuth 2.0 credentials</returns>
        public static IAuthenticator GetAuthenticatorFromState(IAuthorizationState credentials, OPEN_AUTH_CLIENT ClientCredentials)
        {
            var provider = new StoredStateClient(GoogleAuthenticationServer.Description, ClientCredentials.CLIENT_ID, ClientCredentials.CLIENT_SECRET, credentials);
            var auth = new OAuth2Authenticator<StoredStateClient>(provider, StoredStateClient.GetState);
            auth.LoadAccessToken();
            return auth;
        }

        /// <summary>
        /// Retrieved stored credentials for the provided user ID.
        /// </summary>
        /// <param name="userId">User's ID.</param>
        /// <returns>Stored GoogleAccessProtectedResource if found, null otherwise.</returns>
        static IAuthorizationState GetStoredCredentials(String userId)
        {
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

        /// <summary>
        /// Exchange an authorization code for OAuth 2.0 credentials.
        /// </summary>
        /// <param name="authorizationCode">Authorization code to exchange for OAuth 2.0 credentials.</param>
        /// <returns>OAuth 2.0 credentials.</returns>
        /// <exception cref="CodeExchangeException">An error occurred.</exception>
        static IAuthorizationState ExchangeCode(String authorizationCode,OPEN_AUTH_CLIENT clientCredentials)
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, clientCredentials.CLIENT_ID, clientCredentials.CLIENT_SECRET);
            IAuthorizationState state = new AuthorizationState();
            state.Callback = new Uri(clientCredentials.REDIRECT_URI);
            try
            {
                state = provider.ProcessUserAuthorization(authorizationCode, state);
                return state;
            }
            catch (ProtocolException)
            {
                throw new CodeExchangeException(null);
            }
        }

        /// <summary>
        /// Send a request to the UserInfo API to retrieve the user's information.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials to authorize the request.</param>
        /// <returns>User's information.</returns>
        /// <exception cref="NoUserIdException">An error occurred.</exception>
        public static Userinfo GetUserInfo(IAuthenticator credentials) 
        {
            Oauth2Service userInfoService = new Oauth2Service(credentials);
            Userinfo userInfo = null;
            try {
                userInfo = userInfoService.Userinfo.Get().Fetch();
            } catch (GoogleApiRequestException e) {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            if (userInfo != null && !String.IsNullOrEmpty(userInfo.Id)) {
                return userInfo;
            } else {
                throw new NoUserIdException();
            }
        }

        /// <summary>
        /// Send a request to the UserInfo API to retrieve the user's information.
        /// </summary>
        /// <param name="credentials">OAuth 2.0 credentials to authorize the request.</param>
        /// <returns>User's information.</returns>
        /// <exception cref="NoUserIdException">An error occurred.</exception>
        public static Userinfo GetUserInfo(IAuthorizationState credentials, OPEN_AUTH_CLIENT clientCredentials)
        {
            return GetUserInfo(GetAuthenticatorFromState(credentials, clientCredentials));
        }

        /// <summary>
        /// Retrieve the authorization URL.
        /// </summary>
        /// <param name="emailAddress">User's e-mail address.</param>
        /// <param name="state">State for the authorization URL.</param>
        /// <returns>Authorization URL to redirect the user to.</returns>
        public static String GetAuthorizationUrl(String emailAddress, String state,OPEN_AUTH_CLIENT clientCredentials)
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = clientCredentials.CLIENT_ID;

            IAuthorizationState authorizationState = new AuthorizationState(clientCredentials.SCOPES);
            authorizationState.Callback = new Uri(clientCredentials.REDIRECT_URI);

            UriBuilder builder = new UriBuilder(provider.RequestUserAuthorization(authorizationState));
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(builder.Query);

            queryParameters.Set("access_type", "offline");
            queryParameters.Set("approval_prompt", "force");
            queryParameters.Set("user_id", emailAddress);
            queryParameters.Set("state", state);

            builder.Query = queryParameters.ToString();
            return builder.Uri.ToString();
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
        public static IAuthenticator GetCredentials(String authorizationCode, String state, OPEN_AUTH_CLIENT clientCredentials)
        {
            String emailAddress = "";
            try
            {
                IAuthorizationState credentials = ExchangeCode(authorizationCode, clientCredentials);
                Userinfo userInfo = GetUserInfo(credentials, clientCredentials);
                String userId = userInfo.Id;
                emailAddress = userInfo.Email;
                if (!String.IsNullOrEmpty(credentials.RefreshToken))
                {
                    StoreCredentials(userId, credentials);
                    return GetAuthenticatorFromState(credentials, clientCredentials);
                }
                else
                {
                    credentials = GetStoredCredentials(userId);
                    if (credentials != null && !String.IsNullOrEmpty(credentials.RefreshToken))
                    {
                        return GetAuthenticatorFromState(credentials, clientCredentials);
                    }
                }
            }
            catch (CodeExchangeException e)
            {
                Console.WriteLine("An error occurred during code exchange.");
                // Drive apps should try to retrieve the user and credentials for the current
                // session.
                // If none is available, redirect the user to the authorization URL.
                e.AuthorizationUrl = GetAuthorizationUrl(emailAddress, state, clientCredentials);
                throw e;
            }
            catch (NoUserIdException)
            {
                Console.WriteLine("No user ID could be retrieved.");
            }
            // No refresh token has been retrieved.
            String authorizationUrl = GetAuthorizationUrl(emailAddress, state, clientCredentials);
            throw new NoRefreshTokenException(authorizationUrl);
        }

        /// <summary>
        /// Download a file and return a string with its content.
        /// </summary>
        /// <param name="auth">Authenticator responsible for creating web requests.</param>
        /// <param name="downloadUrl">Url to be used to download the resource.</param>
        public static string DownloadFile(IAuthenticator auth, String downloadUrl)
        {
            string result = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(downloadUrl));
                auth.ApplyAuthenticationToRequest(request);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                System.IO.Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            return result;
        }

        /// <summary>
        /// Update both metadata and content of a file and return the updated file.
        /// </summary>
        public static Google.Apis.Drive.v2.Data.File UpdateResource(Google.Apis.Drive.v2.DriveService service, IAuthenticator auth, String fileId, String newTitle,
            String newDescription, String newMimeType, String content, bool newRevision)
        {
            // First retrieve the file from the API.
            Google.Apis.Drive.v2.Data.File body = service.Files.Get(fileId).Fetch();

            body.Title = newTitle;
            body.Description = newDescription;
            body.MimeType = newMimeType;

            //byte[] byteArray = Encoding.ASCII.GetBytes(content);
            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            MemoryStream stream = new MemoryStream(byteArray);

            Google.Apis.Drive.v2.FilesResource.UpdateMediaUpload request = service.Files.Update(body, fileId, stream, newMimeType);
            request.Upload();


            Permission newPermission = new Permission();
            newPermission.Type = "anyone";
            newPermission.Role = "reader";
            newPermission.Value = "";
            newPermission.WithLink = true;
            service.Permissions.Insert(newPermission, fileId).Fetch();


            return request.ResponseBody;
        }

        /// <summary>
        /// Create a new file and return it.
        /// </summary>
        public static Google.Apis.Drive.v2.Data.File InsertResource(Google.Apis.Drive.v2.DriveService service, IAuthenticator auth, String title,
            String description, String mimeType, String content)
        {
            // File's metadata.
            Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
            body.Title = title;
            body.Description = description;
            body.MimeType = mimeType;

            byte[] byteArray = Encoding.ASCII.GetBytes(content);
            MemoryStream stream = new MemoryStream(byteArray);

            Google.Apis.Drive.v2.FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, mimeType);
            request.Upload();

            return request.ResponseBody;
        }
    }

}
