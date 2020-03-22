using System;

#if NETSTANDARD2_0
using gfoidl.Base64;
#endif

namespace JWT
{
    /// <summary>
    /// Base64 encoding/decoding implementation according to the JWT spec
    /// </summary>
    public sealed class JwtBase64UrlEncoder : IBase64UrlEncoder
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        public string Encode(byte[] input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));
            if (input.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(input));

#if NETSTANDARD2_0
            return Base64.Url.Encode(input);
#else
            var output = Convert.ToBase64String(input);

            // Remove any trailing '='s
            var idx = output.IndexOf('=');
            if (idx > 0)
            {
                output = output.Substring(0, idx);
            }

            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
#endif
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException" />
        /// <exception cref="FormatException" />
        public byte[] Decode(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
                throw new ArgumentException(nameof(input));

#if NETSTANDARD2_0
            return Base64.Url.Decode(input.AsSpan());
#else
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0:
                    break; // No pad chars in this case
                case 2:
                    output += "==";
                    break; // Two pad chars
                case 3:
                    output += "=";
                    break; // One pad char
                default:
                    throw new FormatException("Illegal base64url string.");
            }

             // Standard base64 decoder
            return Convert.FromBase64String(output);
#endif
        }
    }
}