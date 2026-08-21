using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DpgShowcaseCart.Wpf.Models;
using Kassyi.Generators.DependencyProperty;

namespace DpgShowcaseCart.Wpf.Controls;

// 1. Core DPG Features: TwoWay Binding, XML Documentation (Description), and Category metadata injection
[DependencyProperty<string>("CardNumber", 
    DefaultValue = "•••• •••• •••• ••••", 
    DefaultBindingMode = DefaultBindingMode.TwoWay, 
    Category = "Payment", 
    Description = "16-digit credit card number with real-time validation.")]

[DependencyProperty<string>("CardHolder", 
    DefaultValue = "CARDHOLDER NAME", 
    Category = "Payment", 
    Description = "Name of the cardholder.")]

[DependencyProperty<string>("ExpiryDate", 
    DefaultValue = "MM/YY", 
    Category = "Payment", 
    Description = "Card expiration date in MM/YY format.")]

// 2. Data Validation: ValidateValueCallback and CoerceValueCallback generation
[DependencyProperty<decimal>("Amount", 
    DefaultValueExpression = "128.50m", 
    Coerce = true, 
    Validate = true, 
    Category = "Payment", 
    Description = "Payment transaction amount.")]


[DependencyProperty<PaymentStatus>("Status", 
    DefaultValue = PaymentStatus.Idle, 
    DefaultBindingMode = DefaultBindingMode.TwoWay, 
    Category = "Payment", 
    Description = "Current payment processing status.")]

// 3. Encapsulation: ReadOnly Dependency Properties (Generates internal setter, public getter, and RegisterReadOnly)
[DependencyProperty<CardBrand>("Brand", 
    DefaultValue = CardBrand.Visa, 
    IsReadOnly = true, 
    Category = "Payment", 
    Description = "Detected credit card brand (Visa/Mastercard).")]

[DependencyProperty<bool>("IsProcessing", 
    DefaultValue = false, 
    IsReadOnly = true, 
    Category = "Payment", 
    Description = "Indicates whether payment is currently in progress.")]

[DependencyProperty<bool>("IsValidCardNumber", 
    DefaultValue = false, 
    IsReadOnly = true, 
    Category = "Validation", 
    Description = "Indicates whether the card number passes the Luhn checksum algorithm.")]
// --- DPG0004 Demo Section ---
// Using DefaultValueExpression = "new(...)" for a reference type (Brush)
// causes a WPF-specific memory sharing bug, which triggers a DPG0004 static analysis error.
// [DependencyProperty<Brush>("CardBackground", DefaultValueExpression = "new LinearGradientBrush(Color.FromRgb(30, 30, 48), Color.FromRgb(15, 15, 25), 45)")]
// Solution: Set CreateDefaultValueCallback = true and implement the factory method

// 4. Memory Safety & Rendering: CreateDefaultValueCallback (DPG0004 fix) + FrameworkPropertyMetadataOptions (AffectsRender)
[DependencyProperty<Brush>("CardBackground", 
    CreateDefaultValueCallback = true, 
    AffectsRender = true, 
    Category = "Appearance", 
    Description = "Dynamic background gradient brush of the card.")]

// 5. Routed Events: Boilerplate-free event registration with Strategy (Bubble/Tunnel/Direct)
[RoutedEvent<RoutedEventHandler>("CardValidated", 
    RoutedEventStrategy.Bubble, 
    Description = "Occurs when the credit card number checksum validation is evaluated.")]

[RoutedEvent<RoutedEventHandler>("PaymentCompleted", 
    RoutedEventStrategy.Bubble, 
    Description = "Occurs when the payment transaction completes (Approved or Failed).")]
public partial class PaymentCardView : Control
{
    static PaymentCardView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PaymentCardView),
            new FrameworkPropertyMetadata(typeof(PaymentCardView)));
    }

    // DPG0004 fix: Factory method for reference-type brush default value
    private static partial Brush GetCardBackgroundDefaultValue() => 
        new LinearGradientBrush(Color.FromRgb(30, 30, 48), Color.FromRgb(15, 15, 25), 45);

    // ValidateValueCallback: Rejects invalid/negative values at the dependency property system level
    private static partial bool IsAmountValid(decimal? value) => value is null or >= 0m;

    // CoerceValueCallback: Clamps negative inputs
    private partial decimal CoerceAmount(decimal? value)
    {
        if (value < 0)
        {
            return 0m;
        }
        return value ?? 128.50m;
    }

    // OnChanged Callback: Automatically switches brand, gradient, and raises routed event
    partial void OnCardNumberChanged(string oldValue, string newValue)
    {
        // Showcase: Using oldValue to optimize performance
        // Only recreate the background brush (which allocates memory) if the brand actually changed.
        var oldBrand = PaymentModel.DetectBrand(oldValue);
        var newBrand = PaymentModel.DetectBrand(newValue);

        if (oldBrand != newBrand)
        {
            SetValue(BrandPropertyKey, newBrand);
            
            CardBackground = newBrand switch
            {
                CardBrand.Visa => new LinearGradientBrush(Color.FromRgb(26, 42, 108), Color.FromRgb(178, 31, 102), 45),
                CardBrand.Mastercard => new LinearGradientBrush(Color.FromRgb(235, 87, 87), Color.FromRgb(0, 0, 0), 45),
                _ => GetCardBackgroundDefaultValue(),
            };
        }

        var isValid = PaymentModel.ValidateLuhn(newValue);
        SetValue(IsValidCardNumberPropertyKey, isValid);
        OnCardValidated();
    }

    // OnChanged Callback: Handles payment status changes and raises completed event
    partial void OnStatusChanged(PaymentStatus oldValue, PaymentStatus newValue)
    {
        SetValue(IsProcessingPropertyKey, newValue == PaymentStatus.Processing);

        // Showcase: Using oldValue for State Machine transition validation
        // Only raise PaymentCompleted if we successfully transitioned FROM Processing.
        if (oldValue == PaymentStatus.Processing && 
           (newValue == PaymentStatus.Approved || newValue == PaymentStatus.Failed))
        {
            OnPaymentCompleted();
        }
    }
}
