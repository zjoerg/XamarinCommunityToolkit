﻿using System;using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Behaviors.Internals;
using Microsoft.Maui; using Microsoft.Maui.Controls; using Microsoft.Maui.Graphics; using Microsoft.Maui.Controls.Compatibility;

namespace Xamarin.CommunityToolkit.Behaviors
{
	/// <summary>
	/// The <see cref="NumericValidationBehavior"/> is a behavior that allows the user to determine if text input is a valid numeric value. For example, an <see cref="Entry"/> control can be styled differently depending on whether a valid or an invalid numeric input is provided. Additional properties handling validation are inherited from <see cref="ValidationBehavior"/>.
	/// </summary>
	public class NumericValidationBehavior : ValidationBehavior
	{
		/// <summary>
		/// Backing BindableProperty for the <see cref="MinimumValue"/> property.
		/// </summary>
		public static readonly BindableProperty MinimumValueProperty =
			BindableProperty.Create(nameof(MinimumValue), typeof(double), typeof(NumericValidationBehavior), double.NegativeInfinity, propertyChanged: OnValidationPropertyChanged);

		/// <summary>
		/// Backing BindableProperty for the <see cref="MaximumValue"/> property.
		/// </summary>
		public static readonly BindableProperty MaximumValueProperty =
			BindableProperty.Create(nameof(MaximumValue), typeof(double), typeof(NumericValidationBehavior), double.PositiveInfinity, propertyChanged: OnValidationPropertyChanged);

		/// <summary>
		/// Backing BindableProperty for the <see cref="MinimumDecimalPlaces"/> property.
		/// </summary>
		public static readonly BindableProperty MinimumDecimalPlacesProperty =
			BindableProperty.Create(nameof(MinimumDecimalPlaces), typeof(int), typeof(NumericValidationBehavior), 0, propertyChanged: OnValidationPropertyChanged);

		/// <summary>
		/// Backing BindableProperty for the <see cref="MaximumDecimalPlaces"/> property.
		/// </summary>
		public static readonly BindableProperty MaximumDecimalPlacesProperty =
			BindableProperty.Create(nameof(MaximumDecimalPlaces), typeof(int), typeof(NumericValidationBehavior), int.MaxValue, propertyChanged: OnValidationPropertyChanged);

		/// <summary>
		/// The minimum numeric value that will be allowed. This is a bindable property.
		/// </summary>
		public double MinimumValue
		{
			get => (double)GetValue(MinimumValueProperty);
			set => SetValue(MinimumValueProperty, value);
		}

		/// <summary>
		/// The maximum numeric value that will be allowed. This is a bindable property.
		/// </summary>
		public double MaximumValue
		{
			get => (double)GetValue(MaximumValueProperty);
			set => SetValue(MaximumValueProperty, value);
		}

		/// <summary>
		/// The minimum number of decimal places that will be allowed. This is a bindable property.
		/// </summary>
		public int MinimumDecimalPlaces
		{
			get => (int)GetValue(MinimumDecimalPlacesProperty);
			set => SetValue(MinimumDecimalPlacesProperty, value);
		}

		/// <summary>
		/// The maximum number of decimal places that will be allowed. This is a bindable property.
		/// </summary>
		public int MaximumDecimalPlaces
		{
			get => (int)GetValue(MaximumDecimalPlacesProperty);
			set => SetValue(MaximumDecimalPlacesProperty, value);
		}

		protected override object? Decorate(object? value)
			=> base.Decorate(value)?.ToString()?.Trim();

		protected override ValueTask<bool> ValidateAsync(object? value, CancellationToken token)
		{
			if (value is not string valueString)
				return new ValueTask<bool>(false);

			if (!(double.TryParse(valueString, out var numeric)
					&& numeric >= MinimumValue
					&& numeric <= MaximumValue))
			{
				return new ValueTask<bool>(false);
			}

			var decimalDelimeterIndex = valueString.IndexOf(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			var hasDecimalDelimeter = decimalDelimeterIndex >= 0;

			// If MaximumDecimalPlaces equals zero, ".5" or "14." should be considered as invalid inputs.
			if (hasDecimalDelimeter && MaximumDecimalPlaces == 0)
				return new ValueTask<bool>(false);

			var decimalPlaces = hasDecimalDelimeter
				? valueString.Substring(decimalDelimeterIndex + 1, valueString.Length - decimalDelimeterIndex - 1).Length
				: 0;

			return new ValueTask<bool>(
				decimalPlaces >= MinimumDecimalPlaces &&
				decimalPlaces <= MaximumDecimalPlaces);
		}
	}
}