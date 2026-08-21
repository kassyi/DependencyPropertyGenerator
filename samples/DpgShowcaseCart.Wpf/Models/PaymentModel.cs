namespace DpgShowcaseCart.Wpf.Models;

public enum PaymentStatus
{
    Idle,
    Processing,
    Approved,
    Failed,
}

public enum CardBrand
{
    Unknown,
    Visa,
    Mastercard,
}

public static class PaymentModel
{
    /// <summary>
    /// Determines the card brand based on card number prefix.
    /// </summary>
    public static CardBrand DetectBrand(string? cardNumber) =>
        cardNumber?.Replace(" ", "") switch
        {
            ['4', ..] => CardBrand.Visa,
            ['5', ..] => CardBrand.Mastercard,
            _ => CardBrand.Unknown,
        };

    /// <summary>
    /// Calculates the Luhn algorithm (Mod 10 checksum) for a credit card number.
    /// </summary>
    public static bool ValidateLuhn(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return false;
        }

        ReadOnlySpan<char> span = cardNumber;
        var sum = 0;
        var digitCount = 0;
        var isSecond = false;

        for (var i = span.Length - 1; i >= 0; i--)
        {
            var c = span[i];
            if (!char.IsDigit(c))
            {
                if (c == ' ')
                {
                    continue;
                }

                return false;
            }

            var d = c - '0';
            digitCount++;

            if (isSecond)
            {
                d *= 2;
                if (d > 9)
                {
                    d -= 9;
                }
            }

            sum += d;
            isSecond = !isSecond;
        }

        return digitCount is >= 13 and <= 19 && sum % 10 == 0;
    }

    /// <summary>
    /// Simulates payment processing asynchronously.
    /// </summary>
    public static async Task<PaymentStatus> ProcessPaymentAsync(string? cardNumber, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken);
        return ValidateLuhn(cardNumber) ? PaymentStatus.Approved : PaymentStatus.Failed;
    }
}
