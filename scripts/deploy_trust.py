from brownie import Trust, network, config, accounts
from scripts.add_beneficiary import add_beneficiary
from scripts.helpful_scipts import get_account
import os

from scripts.withdraw_funds import withdraw_funds


def deploy_trust():
    account = get_account()
    trust = Trust.deploy(
        {"from": account},
        publish_source=config["networks"][network.show_active()].get("verify", False),
    )
    print("You've just deployed the Trust contract!")
    return trust


def main():
    deploy_trust()
