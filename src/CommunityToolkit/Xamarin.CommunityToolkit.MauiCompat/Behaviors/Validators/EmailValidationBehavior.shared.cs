﻿#if !NETSTANDARD1_0
using System;using Microsoft.Extensions.Logging;
using System.Globalization;
#endif
using System.Text.RegularExpressions;

namespace Xamarin.CommunityToolkit.Behaviors
{
	/// <summary>
	/// The <see cref="EmailValidationBehavior"/> is a behavior that allows users to determine whether or not text input is a valid e-mail address. For example, an <see cref="Forms.Entry"/> control can be styled differently depending on whether a valid or an invalid e-mail address is provided. The validation is achieved through a regular expression that is used to verify whether or not the text input is a valid e-mail address. It can be overridden to customize the validation through the properties it inherits from <see cref="Internals.ValidationBehavior"/>.
	/// </summary>
	public class EmailValidationBehavior : TextValidationBehavior
	{
#if !NETSTANDARD1_0
		readonly Regex normalizerRegex = new Regex(@"(@)(.+)$");
#endif

		protected override string DefaultRegexPattern
			=> @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
				@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

		protected override RegexOptions DefaultRegexOptions => RegexOptions.IgnoreCase;

		protected override object? Decorate(object? value)
		{
			var stringValue = base.Decorate(value)?.ToString();
#if NETSTANDARD1_0
			return stringValue;
#else
			if (string.IsNullOrWhiteSpace(stringValue))
				return stringValue;

			try
			{
				static string DomainMapper(Match match)
				{
					// Use IdnMapping class to convert Unicode domain names.
					var idn = new IdnMapping();

					// Pull out and process domain name (throws ArgumentException on invalid)
					var domainName = idn.GetAscii(match.Groups[2].Value);
					return match.Groups[1].Value + domainName;
				}

				// Normalize the domain
				return normalizerRegex.Replace(stringValue, DomainMapper);
			}
			catch (ArgumentException)
			{
				return stringValue;
			}
#endif
		}
	}
}