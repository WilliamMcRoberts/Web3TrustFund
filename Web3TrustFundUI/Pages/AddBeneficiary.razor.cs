using Microsoft.AspNetCore.Components;
using MetaMask.Blazor;
using MetaMask.Blazor.Enums;
using System.Numerics;
using Nethereum.Web3;
using Nethereum.ABI.Model;
using Nethereum.ABI.FunctionEncoding;
using MetaMask.Blazor.Exceptions;
using Blazorise;
using Syncfusion.Blazor.Calendars;

namespace Web3TrustFundUI.Pages
{
    public partial class AddBeneficiary
    {

        [Inject]
        public MetaMaskService MetaMaskService { get; set; } = default!;

        public string? SelectedAddress { get; set; }


        public string? SelectedChain { get; set; }

        public string? SelectedNetwork { get; set; }

        public decimal CurrentEthBalance { get; set; }

        public string? FunctionResult { get; set; }

        public string? Nonce { get; set; }

        public Chain? Chain { get; set; }

        public bool HasMetaMask { get; set; } = true;

        public string? RpcResult { get; set; }

        public bool IsSiteConnected { get; set; }

        public int TimeUntilRelease { get; set; }
        public decimal AmountToLock { get; set; }

        public DateTime? ReleaseDate { get; set; }
        public string BeneficiaryAddress {get;set;}

        public string SelectedTab { get; set; }
        SfDatePicker<DateTime?> DateObj;

        

        

        DatePicker<DateTime?> datePicker;

        private Theme theme = new Theme
        {
            IsRounded = false,
            IsGradient = true,
            ColorOptions = new ThemeColorOptions
            {
                Primary = "#DC143C",
                Secondary = "#5F9EA0",

            }
        };






        protected override async Task OnInitializedAsync()
        {
            MetaMaskService.AccountChangedEvent += MetaMaskService_AccountChangedEvent;
            MetaMaskService.ChainChangedEvent += MetaMaskService_ChainChangedEvent;
            HasMetaMask = await MetaMaskService.HasMetaMask();
            if (!HasMetaMask)
            {
                await MetaMaskService.ListenToEvents();
            }

            IsSiteConnected = await MetaMaskService.IsSiteConnected();
            if (IsSiteConnected)
            {
                await GetSelectedAddress();
                await GetSelectedNetwork();
                //StateHasChanged();
            }
            else
            {
                await ConnectMetaMask();
            }

        }

        public void FocusHandler(Syncfusion.Blazor.Calendars.FocusEventArgs args)
        {
            this.DateObj.ShowPopupAsync();
        }

        public void OnSelectedTabChanged(string name)
        {
            SelectedTab = name;
            navManager.NavigateTo(SelectedTab);
        }

        public async void OnSubmitClicked()
        {
            var timeUntilRelease = ConvertToUnixTimestamp(ReleaseDate);
            await CallSmartContractFunctionAddBeneficiary(BeneficiaryAddress, timeUntilRelease, AmountToLock);
        }
        
        public static long ConvertToUnixTimestamp(DateTime? date)
        {
            DateTime newDate = (DateTime)date;
            long timeUntilRelease = newDate.Ticks;
            return timeUntilRelease;
        }



        public void OnAmountChanged(decimal amount)
        {
            AmountToLock = amount;
        }

        public void OnDateChanged(DateTime? date)
        {
            ReleaseDate = date;
        }

        public async Task MetaMaskService_ChainChangedEvent((long, Chain) arg)
        {
            await GetSelectedAddress();
            StateHasChanged();
        }

        public async Task MetaMaskService_AccountChangedEvent(string arg)
        {
            await GetSelectedAddress();
            StateHasChanged();
        }

        public async Task GetSelectedAddress()
        {
            SelectedAddress = await MetaMaskService.GetSelectedAddress();

        }

        public async Task GetSelectedNetwork()
        {
            var chainInfo = await MetaMaskService.GetSelectedChain();
            Chain = chainInfo.chain;

            SelectedChain = chainInfo.chainId.ToString();
            SelectedNetwork = chainInfo.chain.ToString();

        }

        public async Task ConnectMetaMask()
        {
            await MetaMaskService.ConnectMetaMask();

            await OnInitializedAsync();

        }

        private string GetEncodedFunctionAddBeneficiary(string beneficiaryAddress, long timeUntilRelease)
        {

            FunctionABI function = new FunctionABI("addBeneficiary", false);

            var inputsParameters = new[] {
                    new Parameter("address", "_beneficiary"),
                    new Parameter("uint256", "_timeUntilRelease"),
                };
            function.InputParameters = inputsParameters;

            var functionCallEncoder = new FunctionCallEncoder();

            var data = functionCallEncoder.EncodeRequest(function.Sha3Signature, inputsParameters,
                beneficiaryAddress,
                timeUntilRelease);
            return data;
        }

        public async Task CallSmartContractFunctionAddBeneficiary(string beneficiaryAddress, long timeUntilRelease, decimal amount)
        {
            try
            {
                BigInteger amountToLock = Web3.Convert.ToWei(amount);
                ConfigurationManager config = new();
                string contractAddress = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AddressSettings")["ContractAddress"];
                string data = GetEncodedFunctionAddBeneficiary(beneficiaryAddress, timeUntilRelease);

                data = data[2..]; //Remove the 0x from the generated string
                var result = await MetaMaskService.SendTransaction(contractAddress, amountToLock, data);
                FunctionResult = $"TX Hash: {result}";
            }
            catch (UserDeniedException)
            {
                FunctionResult = "User Denied";
            }
            catch (Exception ex)
            {
                FunctionResult = $"Exception: {ex}";
            }
        }

        
    }
}