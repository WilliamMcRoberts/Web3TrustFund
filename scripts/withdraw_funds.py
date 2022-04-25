from scripts.helpful_scipts import get_account, get_account2, get_account3
from brownie import Trust


def withdraw_funds(amount):
    trust = Trust[-1]
    account = get_account3()
    tx = trust.withdrawFunds(amount, {"from": account})
    tx.wait(1)
    return tx


def main():
    withdraw_funds()
