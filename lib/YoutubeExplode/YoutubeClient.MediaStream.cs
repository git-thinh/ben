using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode.Internal;
using YoutubeExplode.Models.MediaStreams;

namespace YoutubeExplode
{
    public partial class YoutubeClient
    {
        /// <inheritdoc />
        public MediaStream GetMediaStreamAsync(MediaStreamInfo info)
        {
            info.GuardNotNull(nameof(info));

            // Get stream
            var stream = _httpClient.GetStreamAsync(info.Url).Result; //.ConfigureAwait(false);

            return new MediaStream(info, stream);
        }

        /// <inheritdoc />
        public void DownloadMediaStreamAsync(MediaStreamInfo info, Stream output,
            //IProgress<double> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            info.GuardNotNull(nameof(info));
            output.GuardNotNull(nameof(output));

            // Determine if stream is rate-limited
            var isRateLimited = !Regex.IsMatch(info.Url, @"ratebypass[=/]yes");

            // Download rate-limited streams in segments
            if (isRateLimited)
            {
                // Determine segment count
                //const long segmentSize = 9_898_989; // this number was carefully devised through research
                const long segmentSize = 9898989; // this number was carefully devised through research
                var segmentCount = (int)Math.Ceiling(1.0 * info.Size / segmentSize);

                // Keep track of bytes copied for progress reporting
                var totalBytesCopied = 0L;

                for (var i = 0; i < segmentCount; i++)
                {
                    // Determine segment range
                    var from = i * segmentSize;
                    var to = (i + 1) * segmentSize - 1;

                    // Download segment
                    using (var input = _httpClient.GetStreamAsync(info.Url, from, to))
                    {
                        int bytesCopied;
                        do
                        {
                            // Copy
                            bytesCopied = input.CopyChunkToAsync(output, cancellationToken);

                            // Report progress
                            totalBytesCopied += bytesCopied;
                            //progress?.Report(1.0 * totalBytesCopied / info.Size);
                        } while (bytesCopied > 0);
                    }
                }
            }
            // Download non-limited streams directly
            else
            {
                using (var input = GetMediaStreamAsync(info))
                    input.CopyToAsync(output, cancellationToken);
            }
        }

#if NETSTANDARD2_0 || NET45 || NETCOREAPP1_0

        /// <inheritdoc />
        public async Task DownloadMediaStreamAsync(MediaStreamInfo info, string filePath,
            IProgress<double> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            filePath.GuardNotNull(nameof(filePath));

            using (var output = File.Create(filePath))
                await DownloadMediaStreamAsync(info, output, progress, cancellationToken).ConfigureAwait(false);
        }

#endif
    }
}