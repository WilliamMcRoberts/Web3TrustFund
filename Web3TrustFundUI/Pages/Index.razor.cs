using Microsoft.AspNetCore.Components;
using MetaMask.Blazor;
using MetaMask.Blazor.Enums;
using Nethereum.Web3;
using Blazorise;

namespace Web3TrustFundUI.Pages
{
    public partial class Index : IDisposable
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
        public string SelectedTab { get; set; }

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
            }
            else
            {
                await ConnectMetaMask();
            }
        }

        


        public void OnSelectedTabChanged(string name)
        {
            SelectedTab = name;
            navManager.NavigateTo(SelectedTab);
        }

        



        public async Task ConnectMetaMask()
        {
            await MetaMaskService.ConnectMetaMask();
            await OnInitializedAsync();

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

        public async Task GetNonce()
        {
            var nonce = await MetaMaskService.GetTransactionCount();
            Nonce = nonce.ToString();
        }

        public async Task GetBalance()
        {
            var address = await MetaMaskService.GetSelectedAddress();
            var balance = await MetaMaskService.GetBalance(address);
            CurrentEthBalance = Web3.Convert.FromWei(balance);
        }

        public async Task GenericRpc()
        {
            var result = await MetaMaskService.RequestAccounts();
            RpcResult = result;
        }
        
        public void Dispose()
        {
            MetaMaskService.AccountChangedEvent -= MetaMaskService_AccountChangedEvent;
            MetaMaskService.ChainChangedEvent -= MetaMaskService_ChainChangedEvent;
        }

    }
}