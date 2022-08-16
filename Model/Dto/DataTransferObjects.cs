﻿using System;

namespace Model.Dto
{
    public record FormatDto(
        string format_id,
        string format_note,
        string ext,
        string protocol,
        string? acodec,
        string? vcodec,
        string url,
        int? width,
        int? height,
        string audio_ext,
        string video_ext,
        string format,
        string? resolution,
        int? asr,
        long? filesize,
        int? source_preference,
        double? fps,
        int? quality,
        bool? has_drm,
        double? tbr,
        string language,
        int? language_preference,
        int? preference,
        string dynamic_range,
        double? abr,
        string container,
        double? vbr,
        double? filesize_approx);

    public record RequestedFormatDto(
        int? asr,
        long? filesize,
        string format_id,
        string format_note,
        int source_preference,
        double? fps,
        int? height,
        int quality,
        bool has_drm,
        double tbr,
        string url,
        int? width,
        string language,
        int language_preference,
        object preference,
        string ext,
        string vcodec,
        string acodec,
        string dynamic_range,
        double vbr,
        string container,
        string protocol,
        string video_ext,
        string audio_ext,
        string format,
        string resolution,
        double? abr);

    public record MetadataDto(
        string id,
        string title,
        IReadOnlyList<FormatDto> formats,
        IReadOnlyList<ThumbnailDto> thumbnails,
        string thumbnail,
        string description,
        string uploader,
        string uploader_id,
        string uploader_url,
        string channel_id,
        string channel_url,
        double duration,
        long view_count,
        object average_rating,
        int age_limit,
        string webpage_url,
        IReadOnlyList<string> categories,
        IReadOnlyList<object> tags,
        bool playable_in_embed,
        bool? is_live,
        bool was_live,
        string live_status,
        object release_timestamp,
        object chapters,
        int like_count,
        string channel,
        int channel_follower_count,
        string? upload_date,
        string availability,
        string original_url,
        string webpage_url_basename,
        string webpage_url_domain,
        string extractor,
        string extractor_key,
        object playlist,
        object playlist_index,
        string display_id,
        string fulltitle,
        string duration_string,
        object requested_subtitles,
        object _has_drm,
        IReadOnlyList<RequestedFormatDto> requested_formats,
        string format,
        string format_id,
        string ext,
        string protocol,
        object language,
        string format_note,
        double filesize_approx,
        double tbr,
        int width,
        int height,
        string resolution,
        double? fps,
        string dynamic_range,
        string vcodec,
        double vbr,
        object stretched_ratio,
        string acodec,
        double abr,
        int asr,
        int epoch,
        string _filename,
        string filename,
        string urls,
        string _type);

    public record ThumbnailDto(
        string url,
        int preference,
        string id,
        int? height,
        int? width,
        string resolution);

    public record DownloadProgressDto(
        long downloaded,
        long? total,
        double elapsed,
        double? speed,
        int? eta);

}