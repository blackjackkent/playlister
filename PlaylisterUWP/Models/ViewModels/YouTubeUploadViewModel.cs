namespace PlaylisterUWP.Models.ViewModels
{
	using System;
	using Google.Apis.YouTube.v3.Data;

	public class YouTubeUploadViewModel
	{
		public string Title { get; set; }
		public DateTime UploadDateTime { get; set; }
		public string ImagePath { get; set; }
		public string Url { get; set; }

		public YouTubeUploadViewModel() { }

		public YouTubeUploadViewModel(PlaylistItem item)
		{
			Title = item.Snippet.Title;
			UploadDateTime = item.Snippet.PublishedAt.GetValueOrDefault();
			ImagePath = item.Snippet.Thumbnails.Standard?.Url;
			Url = "https://www.youtube.com/watch?v=" + item.Snippet.ResourceId.VideoId;
		}
	}
}
