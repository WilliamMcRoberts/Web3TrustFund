from brownie import Trust, config, accounts, network
from scripts.helpful_scipts import LOCAL_BLOCKCHAIN_ENVIRONMENTS, get_account
from web3 import Web3

amount = Web3.toWei(0.005, "ether")
time = 0.001


def add_beneficiary_test():
    trust = Trust[-1]
    account = get_account()
    if network.show_active() in LOCAL_BLOCKCHAIN_ENVIRONMENTS:
        beneficiary = accounts[1]
    else:
        beneficiary = accounts.add(config["wallets"]["from_key2"])
    add_tx = trust.addBeneficiary(
        beneficiary,
        time,
        {"from": account, "value": amount},
    )
    add_tx.wait(1)
    print(
        "You've added beneficiary with account address: ",
        beneficiary,
        " amount: ",
        Web3.fromWei(amount, "ether"),
        "ETH",
        " The time until release in seconds is: ",
        time,
        "seconds",
    )
    return add_tx


def main():
    add_beneficiary_test()
