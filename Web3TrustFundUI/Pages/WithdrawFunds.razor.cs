using Microsoft.AspNetCore.Components;
using Blazorise;
using Nethereum.ABI.Model;
using Nethereum.ABI.FunctionEncoding;
using MetaMask.Blazor.Exceptions;
using MetaMask.Blazor;
using Nethereum.Web3;
using System.Numerics;
using MetaMask.Blazor.Enums;


namespace Web3TrustFundUI.Pages
{
    public partial class WithdrawFunds
    {
        [Inject]
        public MetaMaskService MetaMaskService { get; set; } = default !;

        

        
        public bool HasMetaMask { get; set; } = true;
        public string? SelectedAddress { get; set; }

        public string? SelectedChain { get; set; }

        public string? SelectedNetwork { get; set; }

        public string? RpcResult { get; set; }

        public bool IsSiteConnected { get; set; }

        public decimal AmountToWithdraw { get; set; }

        public string? FunctionResult { get; set; }

        public string SelectedTab { get; set; }

        public Chain? Chain { get; set; }



        private Theme theme = new Theme
        {
            IsRounded = false,
            IsGradient = true,
            ColorOptions = new ThemeColorOptions
            {
                Primary = "#DC143C",
                Secondary = "#FFF8DC",

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
            }
            else
            {
                await ConnectMetaMask();
            }
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

        public async void OnSubmitClicked()
        {
            await CallSmartContractFunctionWithdrawFunds(AmountToWithdraw);
        }

        public void OnSelectedTabChanged(string name)
        {
            SelectedTab = name;
            navManager.NavigateTo(SelectedTab);
        }

        public void OnAmountChanged(decimal amount)
        {
            AmountToWithdraw = amount;
        }

        private string GetEncodedFunctionWithdrawFunds(BigInteger amount)
        {
            FunctionABI function = new FunctionABI("withdrawFunds", false);
            var inputsParameters = new[]{new Parameter("uint256", "_amount"), };
            function.InputParameters = inputsParameters;
            var functionCallEncoder = new FunctionCallEncoder();
            var data = functionCallEncoder.EncodeRequest(function.Sha3Signature, inputsParameters, amount);
            return data;
        }

        public async Task CallSmartContractFunctionWithdrawFunds(decimal amount)
        {
            try
            {
                BigInteger amountToWithdraw = Web3.Convert.ToWei(amount);
                string contractAddress = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AddressSettings")["ContractAddress"];
                BigInteger weiValue = 0;
                string data = GetEncodedFunctionWithdrawFunds(amountToWithdraw);
                data = data[2..]; //Remove the 0x from the generated string
                var result = await MetaMaskService.SendTransaction(contractAddress, weiValue, data);
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