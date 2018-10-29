using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Internal;
using YoutubeExplode.Models;

namespace YoutubeExplode
{
    public partial class YoutubeClient
    {
        /// <inheritdoc />
        public Channel GetChannelAsync(string channelId)
        {
            channelId.GuardNotNull(nameof(channelId));

            if (!ValidateChannelId(channelId))
                throw new ArgumentException($"Invalid YouTube channel ID [{channelId}].", nameof(channelId));

            // This is a hack, it gets uploads and then gets uploader info of first video

            // Get channel uploads
            var uploads = GetChannelUploadsAsync(channelId, 1);

            // Get first video
            var video = uploads.FirstOrDefault();
            if (video == null)
                throw new ParseException("Channel does not have any videos.");

            // Get video channel
            return GetVideoAuthorChannelAsync(video.Id);
        }

        /// <inheritdoc />
        public IList<Video> GetChannelUploadsAsync(string channelId, int maxPages)
        {
            channelId.GuardNotNull(nameof(channelId));
            maxPages.GuardPositive(nameof(maxPages));

            if (!ValidateChannelId(channelId))
                throw new ArgumentException($"Invalid YouTube channel ID [{channelId}].", nameof(channelId));

            // Compose a playlist ID
            var playlistId = "UU" + channelId.SubstringAfter("UC");

            // Get playlist
            var playlist = GetPlaylistAsync(playlistId, maxPages);

            return playlist.Videos;
        }

        /// <inheritdoc />
        public IList<Video> GetChannelUploadsAsync(string channelId)
            => GetChannelUploadsAsync(channelId, int.MaxValue);
    }
}