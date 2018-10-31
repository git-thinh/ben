using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace DrEdit.Models
{

    /// <summary>
    /// Exception thrown when an error occurred while retrieving credentials.
    /// </summary>
    public class GetCredentialsException : Exception
    {
        public String AuthorizationUrl { get; set; }

        /// <summary>
        /// Construct a GetCredentialsException.
        /// </summary>
        /// @param authorizationUrl The authorization URL to redirect the user to.
        public GetCredentialsException(String authorizationUrl)
        {
            this.AuthorizationUrl = authorizationUrl;
        }
    }

    /// <summary>
    /// Exception thrown when no refresh token has been found.
    /// </summary>
    public class NoRefreshTokenException : GetCredentialsException
    {
        /// <summary>
        /// Construct a NoRefreshTokenException.
        /// </summary>
        /// @param authorizationUrl The authorization URL to redirect the user to.
        public NoRefreshTokenException(String authorizationUrl) : base(authorizationUrl)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a code exchange has failed.
    /// </summary>
    public class CodeExchangeException : GetCredentialsException
    {
        /// <summary>
        /// Construct a CodeExchangeException.
        /// </summary>
        /// @param authorizationUrl The authorization URL to redirect the user to.
        public CodeExchangeException(String authorizationUrl) : base(authorizationUrl)
        {
        }
    }

    /// <summary>
    /// Exception thrown when no user ID could be retrieved.
    /// </summary>
    public class NoUserIdException : Exception
    {
    }

    /// <summary>
    /// Extends the NativeApplicationClient class to allow setting of a custom IAuthorizationState.
    /// </summary>
    public class StoredStateClient : NativeApplicationClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoredStateClient"/> class.
        /// </summary>
        /// <param name="authorizationServer">The token issuer.</param>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        public StoredStateClient(AuthorizationServerDescription authorizationServer,
            String clientIdentifier,
            String clientSecret,
            IAuthorizationState state)
            : base(authorizationServer, clientIdentifier, clientSecret)
        {
            this.State = state;
        }

        public IAuthorizationState State { get; private set; }

        /// <summary>
        /// Returns the IAuthorizationState stored in the StoredStateClient instance.
        /// </summary>
        /// <param name="provider">OAuth2 client.</param>
        /// <returns>The stored authorization state.</returns>
        static public IAuthorizationState GetState(StoredStateClient provider)
        {
            return provider.State;
        }
    }
}
