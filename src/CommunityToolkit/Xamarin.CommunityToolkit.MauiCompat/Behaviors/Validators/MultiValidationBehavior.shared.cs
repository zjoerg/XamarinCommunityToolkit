﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Behaviors.Internals;
using Microsoft.Maui; using Microsoft.Maui.Controls; using Microsoft.Maui.Graphics; using Microsoft.Maui.Controls.Compatibility;

namespace Xamarin.CommunityToolkit.Behaviors
{
	/// <summary>
	/// The <see cref="MultiValidationBehavior"/> is a behavior that allows the user to combine multiple validators to validate text input depending on specified parameters. For example, an <see cref="Entry"/> control can be styled differently depending on whether a valid or an invalid text input is provided. By allowing the user to chain multiple existing validators together, it offers a high degree of customizability when it comes to validation. Additional properties handling validation are inherited from <see cref="ValidationBehavior"/>.
	/// </summary>
	[ContentProperty(nameof(Children))]
	public class MultiValidationBehavior : ValidationBehavior
	{
		/// <summary>
		/// Backing BindableProperty for the <see cref="Errors"/> property.
		/// </summary>
		public static readonly BindableProperty ErrorsProperty =
			BindableProperty.Create(nameof(Errors), typeof(List<object?>), typeof(MultiValidationBehavior), null, BindingMode.OneWayToSource);

		public static readonly BindableProperty ErrorProperty =
			BindableProperty.CreateAttached(nameof(GetError), typeof(object), typeof(MultiValidationBehavior), null);

		readonly ObservableCollection<ValidationBehavior> children = new ObservableCollection<ValidationBehavior>();

		/// <summary>
		/// Constructor for this behavior.
		/// </summary>
		public MultiValidationBehavior()
			=> children.CollectionChanged += OnChildrenCollectionChanged;

		/// <summary>
		/// Holds the errors from all of the nested invalid validators in <see cref="Children"/>. This is a bindable property.
		/// </summary>
		public List<object?>? Errors
		{
			get => (List<object?>?)GetValue(ErrorsProperty);
			set => SetValue(ErrorsProperty, value);
		}

		/// <summary>
		/// All child behaviors that are part of this <see cref="MultiValidationBehavior"/>. This is a bindable property.
		/// </summary>
		public IList<ValidationBehavior> Children => children;

		/// <summary>
		/// Method to extract the error from the attached property for a child behavior in <see cref="Children"/>.
		/// </summary>
		/// <param name="bindable">The <see cref="ValidationBehavior"/> that we extract the attached Error property</param>
		/// <returns>Object containing error information</returns>
		public static object? GetError(BindableObject bindable) => bindable.GetValue(ErrorProperty);

		/// <summary>
		/// Method to set the error on the attached property for a child behavior in <see cref="Children"/>.
		/// </summary>
		/// <param name="bindable">The <see cref="ValidationBehavior"/> on which we set the attached Error property value</param>
		/// <param name="value">The value to set</param>
		public static void SetError(BindableObject bindable, object value)
			=> bindable.SetValue(ErrorProperty, value);

		protected override async ValueTask<bool> ValidateAsync(object? value, CancellationToken token)
		{
			await Task.WhenAll(children.Select(c =>
			{
				c.Value = value;
				return c.ValidateNestedAsync(token).AsTask();
			})).ConfigureAwait(false);

			if (token.IsCancellationRequested)
				return IsValid;

			var errors = children.Where(c => c.IsNotValid).Select(c => GetError(c));

			if (!errors.Any())
			{
				Errors = null;
				return true;
			}

			if (!Errors?.SequenceEqual(errors) ?? true)
				Errors = (errors ?? Enumerable.Empty<object?>()).ToList();

			return false;
		}

		void OnChildrenCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (var child in e.NewItems.OfType<ValidationBehavior>())
				{
					child.TrySetBindingContext(new Binding
					{
						Path = BindingContextProperty.PropertyName,
						Source = this
					});
				}
			}

			if (e.OldItems != null)
			{
				foreach (var child in e.OldItems.OfType<ValidationBehavior>())
					child.TryRemoveBindingContext();
			}
		}
	}
}