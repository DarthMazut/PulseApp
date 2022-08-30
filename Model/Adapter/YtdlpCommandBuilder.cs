using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Adapter
{
    /// <summary>
    /// Exposes fluent API for easy yt-dlp command creation.
    /// </summary>
    public class YtdlpCommandBuilder
    {
        private const string JSON_PROGRESS_TEMPLATE = @"[JSONDownload]{\""downloaded\"": %(progress.downloaded_bytes)s,\""total\"": %(progress.total_bytes)s,\""elapsed\"": %(progress.elapsed)s,\""speed\"": %(progress.speed)s,\""eta\"": %(progress.eta)s}";

        private string _url;
        private bool _getMetadata;
        private bool _printOutputInNewLine;
        private string? _progressTemplate;
        private string? _targetFormats;
        private string? _speedLimit;
        private string? _outputFolder;
        private string? _outputName;
        private string? _ffmpegPath;
        private string? _extractAudioFormat;
        private int? _extractAudioQuality;

        /// <summary>
        /// Initializes a new instance of the <see cref="YtdlpCommandBuilder"/> class.
        /// </summary>
        /// <param name="url">Url to be downloaded.</param>
        public YtdlpCommandBuilder(string url)
        {
            _url = url;
        }

        /// <summary>
        /// Specifies that building command is for downloading metadata.
        /// When this method is called any other parameters are ignored.
        /// </summary>
        public YtdlpCommandBuilder GetMetadata()
        {
            _getMetadata = true;
            return this;
        }

        /// <summary>
        /// Specifies that output from *yt-dlp* process will be written in separate lines.
        /// </summary>
        public YtdlpCommandBuilder PrintOutputInNewLine()
        {
            _printOutputInNewLine = true;
            return this;
        }

        /// <summary>
        /// Sets progress template to default one.
        /// </summary>
        public YtdlpCommandBuilder SetDefaultProgressTemplate()
        {
            _progressTemplate = JSON_PROGRESS_TEMPLATE;
            return this;
        }

        /// <summary>
        /// Sets progress template.
        /// </summary>
        /// <param name="downloadProgressTemplate">Progress template to be set.</param>
        public YtdlpCommandBuilder SetProgressTemplate(string downloadProgressTemplate)
        {
            _progressTemplate = downloadProgressTemplate;
            return this;
        }

        /// <summary>
        /// Sets the format(s) to be downloaded.
        /// </summary>
        /// <param name="targetFormats">A <see cref="string"/> representing format(s) to be downloaded.</param>
        public YtdlpCommandBuilder SetTargetFormats(string targetFormats)
        {
            _targetFormats = targetFormats;
            return this;
        }

        /// <summary>
        /// Sets the format(s) to be downloaded.
        /// </summary>
        /// <param name="targetFormats">Format(s) to be downloaded.</param>
        public YtdlpCommandBuilder SetTargetFormats(FormatSelection targetFormats)
        {
            _targetFormats = targetFormats.ToString();
            return this;
        }

        /// <summary>
        /// Set speed limit of download process.
        /// </summary>
        /// <param name="speedLimit">Speed limit.</param>
        public YtdlpCommandBuilder SetSpeedLimit(string speedLimit)
        {
            _speedLimit = speedLimit;
            return this;
        }

        /// <summary>
        /// Set speed limit of download process.
        /// </summary>
        /// <param name="speedLimit">Speed limit.</param>
        public YtdlpCommandBuilder SetSpeedLimit(MemorySpace speedLimit)
        {
            MemoryUnit memoryUnit = speedLimit.GetSizeWithMostSuitableUnit(out double sizeValue);
            _speedLimit = memoryUnit switch
            {
                MemoryUnit.Byte => $"{sizeValue}",
                MemoryUnit.Kilobyte => $"{sizeValue}K",
                MemoryUnit.Megabyte => $"{sizeValue}M",
                MemoryUnit.Gigabyte => $"{sizeValue}G",
                _ => throw new NotImplementedException()
            };

            return this;
        }

        /// <summary>
        /// Sets folder where video is to be downloaded.
        /// </summary>
        /// <param name="outputFolderPathFunc">A delegate that returns path to downloading video output folder.</param>
        /// <exception cref="ArgumentNullException"/>
        public YtdlpCommandBuilder SetOutputFolder(Func<YtdlpCommandBuilderFormatProvider, string> outputFolderPathFunc)
        {
            _ = outputFolderPathFunc ?? throw new ArgumentNullException(nameof(outputFolderPathFunc));

            _outputFolder = outputFolderPathFunc.Invoke(new YtdlpCommandBuilderFormatProvider());
            return this;
        }

        /// <summary>
        /// Sets the name of file to be downloaded.
        /// </summary>
        /// <param name="outputNameFunc">A delegate that returns name of output file.</param>
        /// <exception cref="ArgumentNullException"/>
        public YtdlpCommandBuilder SetOutputName(Func<YtdlpCommandBuilderFormatProvider, string> outputNameFunc)
        {
            _ = outputNameFunc ?? throw new ArgumentNullException(nameof(outputNameFunc));

            _outputName = outputNameFunc.Invoke(new YtdlpCommandBuilderFormatProvider());
            return this;
        }

        /// <summary>
        /// Specifies the path to *ffmpeg* executable.
        /// </summary>
        /// <param name="ffmpegPath">Path to *ffmpeg* executable.</param>
        public YtdlpCommandBuilder SetFfmpegPath(string ffmpegPath)
        {
            _ffmpegPath = ffmpegPath;
            return this;
        }

        /// <summary>
        /// Extracts audio from downloaded file and converts it to specified format.
        /// </summary>
        /// <param name="targetFormat">Target format of audio convertion.</param>
        public YtdlpCommandBuilder ExtractAudio(TargetAudioFormat targetFormat)
        {
            return ExtractAudio(targetFormat.ToString().ToLower(), null);
        }

        /// <summary>
        /// Extracts audio from downloaded file and converts it to specified format.
        /// </summary>
        /// <param name="targetFormat">Target format of audio convertion.</param>
        /// <param name="quality">Quality of convertion where 0 is best and 10 worst quality.</param>
        public YtdlpCommandBuilder ExtractAudio(TargetAudioFormat targetFormat, int quality)
        {
            return ExtractAudio(targetFormat.ToString().ToLower(), quality);
        }

        /// <summary>
        /// Extracts audio from downloaded file and converts it to specified format.
        /// </summary>
        /// <param name="targetFormat">Target format of audio convertion.</param>
        /// <param name="quality">Quality of convertion where 0 is best and 10 worst quality.</param>
        public YtdlpCommandBuilder ExtractAudio(string targetFormat, int? quality = default)
        {
            _extractAudioFormat = targetFormat;
            if (quality is not null)
            {
                _extractAudioQuality = ResolveQualityNumber((int)quality);
            }

            return this;
        }

        /// <summary>
        /// Creates command.
        /// </summary>
        public string AsCommand() => ToString();

        /// <inheritdoc/>
        public override string ToString()
        {
            // Should we guard here?
            //_ = _targetFormats ?? throw new InvalidOperationException($"Cannot create command because {_targetFormats} wasn't specified.");
            if (_getMetadata)
            {
                return $@"--print ""%()j"" -- {_url}";
            }


            StringBuilder sb = new();
            if (_targetFormats is not null)
            {
                sb.Append($"-f {_targetFormats} ");
            }
            if (_outputFolder is not null && _outputName is not null)
            {
                sb.Append($"-o \"{ResolveOutput()}\" ");
            }
            if (_printOutputInNewLine)
            {
                sb.Append("--newline ");
            }
            if (_progressTemplate is not null)
            {
                sb.Append($"--progress-template \"{_progressTemplate}\" ");
            }
            if (_ffmpegPath is not null)
            {
                sb.Append($"--ffmpeg-location \"{_ffmpegPath}\" ");
            }
            if (_extractAudioFormat is not null)
            {
                sb.Append($"-x --audio-format {_extractAudioFormat} ");
                if (_extractAudioQuality is not null)
                {
                    sb.Append($"--audio-quality {_extractAudioQuality} ");
                }
            }
            if (_speedLimit is not null)
            {
                sb.Append($"-r {_speedLimit} ");
            }

            sb.Append($"-- {_url}");
            return sb.ToString();
        }

        /// <summary>
        /// Creates a <see cref="YtdlpCommandBuilder"/> object with predefined typical setup.
        /// </summary>
        /// <param name="url">Url to target video.</param>
        public static YtdlpCommandBuilder Download(string url)
        {
            return new YtdlpCommandBuilder(url)
                .SetDefaultProgressTemplate()
                .PrintOutputInNewLine();
        }

        private string ResolveOutput() => Path.Combine(_outputFolder!, _outputName!);

        private int ResolveQualityNumber(int requestedQuality)
        {
            if (requestedQuality < 0)
            {
                return 0;
            }

            if (requestedQuality > 10)
            {
                return 10;
            }

            return requestedQuality;
        }
    }
}
