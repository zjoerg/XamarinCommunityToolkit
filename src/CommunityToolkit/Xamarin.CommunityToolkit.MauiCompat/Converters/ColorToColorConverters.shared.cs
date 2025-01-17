using Xamarin.CommunityToolkit.Extensions;
using Microsoft.Maui; using Microsoft.Maui.Controls; using Microsoft.Maui.Graphics; using Microsoft.Maui.Controls.Compatibility;

namespace Xamarin.CommunityToolkit.Converters
{
	/// <summary>
	/// Converts the incoming value from <see cref="Color"/> and returns the object of a type <see cref="Color"/>.
	/// </summary>
	public class ColorToBlackOrWhiteConverter : BaseConverterOneWay<Color, Color>
	{
		public override Color ConvertFrom(Color value) => value.ToBlackOrWhite();
	}

	/// <summary>
	/// Converts the incoming value from <see cref="Color"/> and returns the object of a type <see cref="Color"/>.
	/// </summary>
	public class ColorToColorForTextConverter : BaseConverterOneWay<Color, Color>
	{
		public override Color ConvertFrom(Color value) => value.ToBlackOrWhiteForText();
	}

	/// <summary>
	/// Converts the incoming value from <see cref="Color"/> and returns the object of a type <see cref="Color"/>.
	/// </summary>
	public class ColorToGrayScaleColorConverter : BaseConverterOneWay<Color, Color>
	{
		public override Color ConvertFrom(Color value) => value.ToGrayScale();
	}

	/// <summary>
	/// Converts the incoming value from <see cref="Color"/> and returns the object of a type <see cref="Color"/>.
	/// </summary>
	public class ColorToInverseColorConverter : BaseConverterOneWay<Color, Color>
	{
		public override Color ConvertFrom(Color value) => value.ToInverseColor();
	}
}