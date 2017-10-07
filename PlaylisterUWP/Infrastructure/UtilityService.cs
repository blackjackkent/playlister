namespace PlaylisterUWP.Infrastructure
{
	using Windows.Security.Cryptography;
	using Windows.Security.Cryptography.Core;
	using Windows.Storage.Streams;

	public class UtilityService
	{
		/// <summary>
		/// Base64url no-padding encodes the given input buffer.
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		public static string EncodeBase64UrlNoPadding(IBuffer buffer)
		{
			string base64 = CryptographicBuffer.EncodeToBase64String(buffer);
			// Converts base64 to base64url.
			base64 = base64.Replace("+", "-");
			base64 = base64.Replace("/", "_");
			// Strips padding.
			base64 = base64.Replace("=", "");
			return base64;
		}

		/// <summary>
		/// Returns URI-safe data with a given input length.
		/// </summary>
		/// <param name="length">Input length (nb. output will be longer)</param>
		/// <returns></returns>
		public static string GenerateRandomDataBase64Url(uint length)
		{
			IBuffer buffer = CryptographicBuffer.GenerateRandom(length);
			return UtilityService.EncodeBase64UrlNoPadding(buffer);
		}

		/// <summary>
		/// Returns the SHA256 hash of the input string.
		/// </summary>
		/// <param name="inputString"></param>
		/// <returns></returns>
		public static IBuffer ConvertToSha256(string inputString)
		{
			HashAlgorithmProvider sha = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
			IBuffer buff = CryptographicBuffer.ConvertStringToBinary(inputString, BinaryStringEncoding.Utf8);
			return sha.HashData(buff);
		}
	}
}
