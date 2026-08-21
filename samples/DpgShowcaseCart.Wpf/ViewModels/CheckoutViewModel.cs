using System.Windows.Input;
using DpgShowcaseCart.Wpf.Models;

namespace DpgShowcaseCart.Wpf.ViewModels;

public class CheckoutViewModel : ViewModelBase
{
    public CheckoutViewModel()
    {
        PayCommand = new AsyncRelayCommand(
            ExecutePayAsync,
            () => IsValidCardNumber && !IsProcessing);

        SelectVisaPresetCommand = new RelayCommand(() =>
        {
            CardNumber = "4242 4242 4242 4242";
            CardHolder = "NO BOILERPLATE";
            ExpiryDate = "08/29";
        });

        SelectMasterPresetCommand = new RelayCommand(() =>
        {
            CardNumber = "5555 5555 5555 4444";
            CardHolder = "PARTIAL CLASS";
            ExpiryDate = "11/28";
        });

        SelectInvalidPresetCommand = new RelayCommand(() =>
        {
            CardNumber = "4242 4242 4242 4241";
            CardHolder = "ASYNC VOID";
            ExpiryDate = "01/26";
        });

        ResetPaymentCommand = new RelayCommand(() =>
        {
            Status = PaymentStatus.Idle;
        });

        UpdateCardState();
    }

    public string CardNumber
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                UpdateCardState();
            }
        }
    } = "4242 4242 4242 4242";

    public string CardHolder
    {
        get;
        set => SetProperty(ref field, value);
    } = "NO BOILERPLATE";

    public string ExpiryDate
    {
        get;
        set => SetProperty(ref field, value);
    } = "08/29";

    public decimal Amount
    {
        get;
        set => SetProperty(ref field, value);
    } = 128.50m;

    public PaymentStatus Status
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
            {
                return;
            }

            OnPropertyChanged(nameof(IsProcessing));
            PayCommand.RaiseCanExecuteChanged();
        }
    } = PaymentStatus.Idle;

    public CardBrand Brand
    {
        get;
        private set => SetProperty(ref field, value);
    } = CardBrand.Visa;

    public bool IsValidCardNumber => PaymentModel.ValidateLuhn(CardNumber);

    public bool IsProcessing => Status == PaymentStatus.Processing;

    public AsyncRelayCommand PayCommand { get; }

    public ICommand SelectVisaPresetCommand { get; }

    public ICommand SelectMasterPresetCommand { get; }

    public ICommand SelectInvalidPresetCommand { get; }

    public ICommand ResetPaymentCommand { get; }

    private void UpdateCardState()
    {
        Brand = PaymentModel.DetectBrand(CardNumber);
        OnPropertyChanged(nameof(IsValidCardNumber));
        PayCommand.RaiseCanExecuteChanged();
    }

    private async Task ExecutePayAsync()
    {
        Status = PaymentStatus.Processing;
        try
        {
            Status = await PaymentModel.ProcessPaymentAsync(CardNumber);
        }
        catch
        {
            Status = PaymentStatus.Failed;
        }
    }
}
