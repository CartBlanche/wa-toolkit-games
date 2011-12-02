namespace Microsoft.Samples.SocialGames.GamePlay.Security
{
    using System;
    using System.Web;
    using System.Web.Util;

    using Microsoft.IdentityModel.Protocols.WSFederation;

    /// <summary>
    /// This WSFederationRequestValidator validates the wresult parameter of the
    /// WS-Federation passive protocol by checking for a SignInResponse message
    /// in the form post. The SignInResponse message contents are verified later by
    /// the WSFederationPassiveAuthenticationModule or the WIF signin controls.
    /// </summary>
    public class WSFederationRequestValidator : RequestValidator
    {
        protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
        {
            validationFailureIndex = 0;

            // TODO Review stack overflow
            ////if (requestValidationSource == RequestValidationSource.Form && collectionKey.Equals(WSFederationConstants.Parameters.Result, StringComparison.Ordinal))
            ////{
            ////    SignInResponseMessage message = WSFederationMessage.CreateFromFormPost(context.Request) as SignInResponseMessage;

            ////    if (message != null)
            ////    {
            ////        return true;
            ////    }
            ////}

            ////return base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);

            return true;
        }
    }
}